"""Anteponer la caratula original y anexar las capturas historicas sin modificar sus pixeles."""
from pathlib import Path
from html import escape
import json

from pypdf import PdfReader, PdfWriter
from reportlab.lib import colors
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import SimpleDocTemplate, Paragraph, Image, Spacer, PageBreak

ROOT = Path(__file__).resolve().parent.parent
AUTHOR = 'Anrrango Benavides Mario Santiago'
SUFFIX = 'Anrrango_Benavides_Mario_Santiago'
BASE = ROOT / 'output/pdf/Informe_Tecnico_GeoLab2D.pdf'
COVER = ROOT / 'docs/recursos/Caratula_ESPE.pdf'
OUTPUT = BASE.with_name('Informe_Tecnico_GeoLab2D_' + SUFFIX + '.pdf')
STEPS = [
    ('Creación del proyecto',
     'Se crea la solución en Visual Studio con la plantilla de Windows Forms para C# y .NET Framework. '
     'La captura muestra el formulario inicial y la estructura del proyecto antes de incorporar las figuras. '
     'Este es el punto de partida de la aplicación.'),
    ('Preparación del lienzo y los parámetros',
     'Se incorpora una superficie de dibujo, el selector de figura y el control de lado. '
     'Esta versión inicial prepara la entrada de parámetros y el espacio donde se representarán las construcciones. '
     'La interfaz de esta captura todavía utiliza el diseño claro anterior al panel lateral.'),
    ('Construcción del hexágono de seis colores',
     'Se generan los seis vértices con seno y coseno y se conectan con el centro para formar seis triángulos equiláteros. '
     'La captura muestra un lado de 280 píxeles, perímetro de 1680 píxeles y área aproximada de 203689.17 píxeles cuadrados. '
     'El control del lado permite comprobar que el dibujo y sus medidas se actualizan juntos.'),
    ('Construcción de la margarita',
     'Se añaden cinco pentágonos regulares distribuidos simétricamente, conservando el hueco central. '
     'El parámetro representa el lado del pentágono exterior y de él se obtiene el lado de cada pétalo. '
     'La captura corresponde a una dimensión exterior de 280 píxeles y a la versión con cinco pétalos rojos.'),
    ('Construcción de polígonos estrellados',
     'Se incorpora el recorrido de vértices por saltos. En esta evidencia se utilizan ocho puntas, salto tres y radio exterior de 150 píxeles. '
     'El recorrido se cierra sin coordenadas fijas y su longitud total es aproximadamente 2217.31 píxeles. '
     'La versión final mantiene esta construcción y amplía el máximo de la interfaz a 64 puntas.'),
    ('Operaciones vectoriales',
     'Se añade la entrada de componentes para dos vectores y su representación mediante flechas desde el origen. '
     'El panel presenta magnitudes, direcciones, producto punto y determinante. '
     'Esta etapa permite contrastar los valores numéricos con la dirección y la longitud de los vectores dibujados.'),
    ('Análisis geométrico y proyección',
     'Se integran los puntos A, B y P, el segmento AB y su punto Q más cercano a P. '
     'La evidencia muestra el análisis sobre la margarita, junto con los controles de transformación y la matriz. '
     'Posteriormente se reorganizó esta información en secciones del panel lateral y se añadió arrastre, selección por clic y ayuda contextual.'),
]


def main():
    for file in [BASE, COVER] + [ROOT / f'docs/imagenes/procedimientos/paso-{i:02}.png' for i in range(1, 8)]:
        if not file.is_file(): raise FileNotFoundError(file)
    fonts = Path('C:/Windows/Fonts')
    pdfmetrics.registerFont(TTFont('Procedure', str(fonts / 'arial.ttf')))
    pdfmetrics.registerFont(TTFont('ProcedureBold', str(fonts / 'arialbd.ttf')))
    title = ParagraphStyle('ProcedureTitle', fontName='ProcedureBold', fontSize=18, leading=23,
        textColor=colors.HexColor('#17394e'), spaceAfter=12)
    body = ParagraphStyle('ProcedureBody', fontName='Procedure', fontSize=10.5, leading=15, spaceAfter=12)
    caption = ParagraphStyle('ProcedureCaption', fontName='Procedure', fontSize=9, leading=13,
        textColor=colors.HexColor('#536574'), spaceAfter=10)
    temporary = ROOT / 'entrega/bin/anexo-procedimientos.pdf'
    temporary.parent.mkdir(parents=True, exist_ok=True)
    doc = SimpleDocTemplate(str(temporary), pagesize=A4, leftMargin=45, rightMargin=45, topMargin=43, bottomMargin=42)
    story = []
    markdown = ['# Desarrollo paso a paso', '',
        'Capturas proporcionadas por el estudiante. Corresponden a las etapas anteriores al rediseño de la interfaz.', '']
    for number, (heading, description) in enumerate(STEPS, 1):
        if number > 1: story.append(PageBreak())
        story.append(Paragraph('Anexo A Desarrollo paso a paso', title))
        story.append(Paragraph(f'Paso {number} {escape(heading)}', ParagraphStyle('StepTitle', parent=title, fontSize=13, leading=17)))
        story.append(Paragraph(escape(description), body))
        file = ROOT / f'docs/imagenes/procedimientos/paso-{number:02}.png'
        picture = Image(str(file))
        ratio = min(doc.width / picture.imageWidth, 440 / picture.imageHeight)
        picture.drawWidth = picture.imageWidth * ratio
        picture.drawHeight = picture.imageHeight * ratio
        picture.hAlign = 'CENTER'
        story.extend([Spacer(1, 8), picture, Spacer(1, 10),
            Paragraph(f'Figura A.{number}. {escape(heading)}. Captura histórica anterior al rediseño visual.', caption)])
        markdown.extend([f'## Paso {number} {heading}', '', description, '',
            f'![Figura A.{number}. {heading}](imagenes/procedimientos/paso-{number:02}.png)', ''])
    story.append(PageBreak())
    declaration = ROOT / 'docs/Declaracion_IA_y_Bibliografia.md'
    for line in declaration.read_text(encoding='utf-8').splitlines():
        if not line.strip():
            continue
        if line.startswith('# '):
            story.append(Paragraph(escape(line[2:]), title))
        elif line.startswith('## '):
            story.append(Paragraph(escape(line[3:]), ParagraphStyle('DeclarationHeading', parent=title, fontSize=13, leading=17, spaceBefore=12)))
        else:
            story.append(Paragraph(escape(line), body))
    base_pages = len(PdfReader(BASE).pages)

    def footer(canvas, document):
        canvas.saveState()
        canvas.setFont('Procedure', 8)
        canvas.setFillColor(colors.HexColor('#536574'))
        canvas.drawString(45, 23, 'GeoLab2D | Documentación complementaria')
        canvas.drawRightString(A4[0] - 45, 23, str(base_pages + document.page))
        canvas.restoreState()

    doc.build(story, onFirstPage=footer, onLaterPages=footer)
    writer = PdfWriter()
    writer.append(str(COVER))
    writer.append(str(BASE))
    writer.append(str(temporary))
    writer.add_metadata({'/Title': 'GeoLab2D Informe técnico y desarrollo paso a paso', '/Author': AUTHOR})
    with OUTPUT.open('wb') as stream: writer.write(stream)
    (ROOT / 'docs/Procedimiento_Paso_a_Paso.md').write_text('\n'.join(markdown), encoding='utf-8')
    (ROOT / 'entrega/informe-actual.json').write_text(json.dumps({
        'ruta': OUTPUT.relative_to(ROOT).as_posix(), 'estudiante': AUTHOR,
        'paginas': len(PdfReader(OUTPUT).pages), 'caratula': 'docs/recursos/Caratula_ESPE.pdf',
        'anexo': 'docs/Procedimiento_Paso_a_Paso.md',
    }, ensure_ascii=False, indent=2), encoding='utf-8')
    print(OUTPUT)
    print('Paginas:', len(PdfReader(OUTPUT).pages))


if __name__ == '__main__': main()
