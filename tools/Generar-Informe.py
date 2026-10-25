"""Genera el informe PDF desde Markdown y evidencias de pruebas aprobadas."""
from pathlib import Path
import csv
import json
import re
from html import escape

from reportlab.lib import colors
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, Preformatted, Image,
)

ROOT = Path(__file__).resolve().parent.parent
EVIDENCE = ROOT / 'entrega' / 'evidencias'
SOURCE = ROOT / 'docs' / 'InformeTecnico.md'
OUTPUT = ROOT / 'output' / 'pdf' / 'Informe_Tecnico_GeoLab2D.pdf'


def main():
    validation = json.loads((EVIDENCE / 'validacion.json').read_text(encoding='utf-8-sig'))
    if validation['codigoPruebas'] != 0 or validation['codigoCompilacion'] != 0:
        raise RuntimeError('Se requieren evidencias de compilacion y pruebas aprobadas.')
    with (EVIDENCE / 'casos-referencia.csv').open(encoding='utf-8-sig', newline='') as stream:
        cases = list(csv.DictReader(stream, delimiter=';'))
    if len(cases) != 16 or any(row['Resultado'] != 'APROBADO' for row in cases):
        raise RuntimeError('Los casos de referencia no estan completos o aprobados.')
    rows = ['| Caso | Esperado | Obtenido | Resultado |', '| --- | --- | --- | --- |']
    for case in cases:
        rows.append('| {} | {:.6f} | {:.6f} | {} |'.format(
            case['Caso'], float(case['Esperado']), float(case['Obtenido']), case['Resultado']))
    text = SOURCE.read_text(encoding='utf-8')
    text = re.sub(r'<!-- INICIO_CASOS -->.*?<!-- FIN_CASOS -->',
        '<!-- INICIO_CASOS -->\n' + '\n'.join(rows) + '\n<!-- FIN_CASOS -->', text, flags=re.S)
    SOURCE.write_text(text, encoding='utf-8')

    font_folder = Path('C:/Windows/Fonts')
    pdfmetrics.registerFont(TTFont('Report', str(font_folder / 'arial.ttf')))
    pdfmetrics.registerFont(TTFont('ReportBold', str(font_folder / 'arialbd.ttf')))
    pdfmetrics.registerFont(TTFont('ReportMono', str(font_folder / 'consola.ttf')))
    pdfmetrics.registerFontFamily('Report', normal='Report', bold='ReportBold', italic='Report', boldItalic='ReportBold')
    base = getSampleStyleSheet()
    styles = {
        'body': ParagraphStyle('BodyReport', fontName='Report', fontSize=10, leading=14, spaceAfter=7),
        'title': ParagraphStyle('TitleReport', fontName='ReportBold', fontSize=27, leading=31, spaceAfter=14),
        'h2': ParagraphStyle('HeadingReport', fontName='ReportBold', fontSize=17, leading=21,
            textColor=colors.HexColor('#17394e'), spaceBefore=4, spaceAfter=12, keepWithNext=True),
        'h3': ParagraphStyle('SubheadingReport', fontName='ReportBold', fontSize=11.5, leading=15,
            spaceBefore=10, spaceAfter=6, keepWithNext=True),
        'code': ParagraphStyle('CodeReport', fontName='ReportMono', fontSize=8.1, leading=11,
            backColor=colors.HexColor('#f1f5f8'), borderPadding=7, spaceBefore=4, spaceAfter=11),
        'cell': ParagraphStyle('CellReport', fontName='Report', fontSize=8.4, leading=11),
        'caption': ParagraphStyle('CaptionReport', fontName='Report', fontSize=8, leading=11,
            textColor=colors.HexColor('#536574'), spaceAfter=6),
    }
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    doc = SimpleDocTemplate(str(OUTPUT), pagesize=A4, rightMargin=45, leftMargin=45,
        topMargin=43, bottomMargin=42, title='GeoLab2D Informe tecnico', author='',
        subject='Construccion algoritmica, POO, transformaciones y validacion')
    story = []

    def inline(value):
        return re.sub(r'\*\*(.*?)\*\*', r'<b>\1</b>', escape(value))

    lines = text.splitlines()
    i = 0
    while i < len(lines):
        line = lines[i].strip()
        if not line or line.startswith('<!--'):
            i += 1
            continue
        if line == '---':
            story.append(PageBreak()); i += 1; continue
        if line.startswith('```'):
            code = []
            i += 1
            while i < len(lines) and not lines[i].startswith('```'):
                code.append(lines[i]); i += 1
            story.append(Preformatted('\n'.join(code), styles['code']))
            i += 1; continue
        if line.startswith('|'):
            table_rows = []
            while i < len(lines) and lines[i].strip().startswith('|'):
                cells = [c.strip() for c in lines[i].strip().strip('|').split('|')]
                if not all(re.fullmatch(r':?-+:?', c) for c in cells):
                    table_rows.append(cells)
                i += 1
            n = len(table_rows[0])
            if n == 4: widths = [doc.width * v for v in (0.40, 0.20, 0.20, 0.20)]
            elif n == 3:
                proporciones = (0.25, 0.57, 0.18)
                if table_rows[0][0] == 'Responsabilidad': proporciones = (0.23, 0.34, 0.43)
                if table_rows[0][0] == 'Parámetro': proporciones = (0.25, 0.42, 0.33)
                widths = [doc.width * v for v in proporciones]
            else: widths = [doc.width * 0.24, doc.width * 0.76]
            if n == 4 and table_rows[0][0] == 'N': widths = [doc.width * v for v in (0.10, 0.10, 0.18, 0.62)]
            data = [[Paragraph(('<b>' if r == 0 else '') + inline(c) + ('</b>' if r == 0 else ''), styles['cell'])
                for c in row] for r, row in enumerate(table_rows)]
            table = Table(data, colWidths=widths, repeatRows=1, hAlign='LEFT')
            table.setStyle(TableStyle([
                ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#e7eff5')),
                ('VALIGN', (0, 0), (-1, -1), 'TOP'),
                ('BOTTOMPADDING', (0, 0), (-1, -1), 5), ('TOPPADDING', (0, 0), (-1, -1), 5),
                ('LEFTPADDING', (0, 0), (-1, -1), 6), ('RIGHTPADDING', (0, 0), (-1, -1), 6),
                ('LINEBELOW', (0, 0), (-1, 0), 0.6, colors.HexColor('#a7b9c7')),
                ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f7f9fb')]),
            ]))
            story.extend([table, Spacer(1, 10)]); continue
        if line.startswith('!['):
            figures = re.findall(r'!\[(.*?)\]\((.*?)\)', line)
            slots = []
            width = (doc.width - 12 * (len(figures) - 1)) / len(figures)
            for caption, path in figures:
                file = (SOURCE.parent / path).resolve()
                if not file.is_file(): raise FileNotFoundError(file)
                picture = Image(str(file))
                max_height = 140 if len(figures) > 1 else 180
                scale = min(width / picture.imageWidth, max_height / picture.imageHeight)
                picture.drawWidth = picture.imageWidth * scale
                picture.drawHeight = picture.imageHeight * scale
                slots.append([picture, Paragraph(inline(caption), styles['caption'])])
            story.append(Table([slots], colWidths=[width] * len(slots), hAlign='CENTER'))
            i += 1; continue
        if line.startswith('# '):
            story.append(Paragraph(inline(line[2:]), styles['title'])); i += 1; continue
        if line.startswith('## '):
            story.append(Paragraph(inline(line[3:]), styles['h2'])); i += 1; continue
        if line.startswith('### '):
            story.append(Paragraph(inline(line[4:]), styles['h3'])); i += 1; continue
        paragraph = [line]
        i += 1
        while i < len(lines) and lines[i].strip() and not lines[i].startswith(('#', '|', '```', '![', '<!--')) and lines[i].strip() != '---':
            paragraph.append(lines[i].strip()); i += 1
        story.append(Paragraph(inline(' '.join(paragraph)), styles['body']))

    def footer(canvas, document):
        canvas.saveState()
        canvas.setFont('Report', 8)
        canvas.setFillColor(colors.HexColor('#536574'))
        canvas.drawString(45, 23, 'GeoLab2D | Informe tecnico | 24 octubre 2026')
        canvas.drawRightString(A4[0] - 45, 23, str(document.page))
        canvas.restoreState()

    doc.build(story, onFirstPage=footer, onLaterPages=footer)
    print(OUTPUT)


if __name__ == '__main__':
    main()
