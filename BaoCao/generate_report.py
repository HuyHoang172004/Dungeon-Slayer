from pathlib import Path
from datetime import date

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.style import WD_STYLE_TYPE
from docx.enum.table import WD_CELL_VERTICAL_ALIGNMENT, WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_BREAK, WD_LINE_SPACING
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Inches, Pt, RGBColor


ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "BaoCao" / "Bao_cao_mon_hoc_Game_2D_3D.docx"
IMAGES = {
    "map1": Path(r"C:\Users\HUY HOANG\Pictures\Screenshots\Screenshot 2026-10-13 000544.png"),
    "map2": Path(r"C:\Users\HUY HOANG\Pictures\Screenshots\Screenshot 2026-10-13 060441.png"),
    "final": Path(r"C:\Users\HUY HOANG\Pictures\Screenshots\Screenshot (659).png"),
    "meteor": ROOT / "Temp" / "CodexCaptures" / "meteor-visible-test.png",
}


def set_cell_shading(cell, fill):
    tc_pr = cell._tc.get_or_add_tcPr()
    shd = tc_pr.find(qn("w:shd"))
    if shd is None:
        shd = OxmlElement("w:shd")
        tc_pr.append(shd)
    shd.set(qn("w:fill"), fill)


def set_repeat_table_header(row):
    tr_pr = row._tr.get_or_add_trPr()
    tbl_header = OxmlElement("w:tblHeader")
    tbl_header.set(qn("w:val"), "true")
    tr_pr.append(tbl_header)


def set_cell_text(cell, text, bold=False, center=False, size=11):
    cell.text = ""
    p = cell.paragraphs[0]
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER if center else WD_ALIGN_PARAGRAPH.LEFT
    p.paragraph_format.space_after = Pt(0)
    p.paragraph_format.line_spacing = 1.15
    run = p.add_run(str(text))
    run.bold = bold
    run.font.name = "Times New Roman"
    run._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
    run.font.size = Pt(size)
    cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER


def add_field(paragraph, instruction):
    run = paragraph.add_run()
    fld_char = OxmlElement("w:fldChar")
    fld_char.set(qn("w:fldCharType"), "begin")
    instr = OxmlElement("w:instrText")
    instr.set(qn("xml:space"), "preserve")
    instr.text = instruction
    separate = OxmlElement("w:fldChar")
    separate.set(qn("w:fldCharType"), "separate")
    placeholder = OxmlElement("w:t")
    placeholder.text = "Nhấn Ctrl+A, F9 để cập nhật"
    end = OxmlElement("w:fldChar")
    end.set(qn("w:fldCharType"), "end")
    run._r.extend([fld_char, instr, separate, placeholder, end])


def add_page_number(paragraph):
    paragraph.alignment = WD_ALIGN_PARAGRAPH.CENTER
    add_field(paragraph, "PAGE")


def add_table(doc, headers, rows, widths=None, font_size=10.5):
    table = doc.add_table(rows=1, cols=len(headers))
    table.style = "Table Grid"
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = True
    hdr = table.rows[0]
    set_repeat_table_header(hdr)
    for i, value in enumerate(headers):
        set_cell_text(hdr.cells[i], value, bold=True, center=True, size=font_size)
        set_cell_shading(hdr.cells[i], "D9EAF7")
        if widths and i < len(widths):
            hdr.cells[i].width = Cm(widths[i])
    for row_data in rows:
        cells = table.add_row().cells
        for i, value in enumerate(row_data):
            set_cell_text(cells[i], value, center=(i == 0), size=font_size)
            if widths and i < len(widths):
                cells[i].width = Cm(widths[i])
    doc.add_paragraph().paragraph_format.space_after = Pt(0)
    return table


def add_para(doc, text="", bold_prefix=None, italic=False, align=None, keep=False):
    p = doc.add_paragraph()
    p.paragraph_format.first_line_indent = Cm(1)
    p.paragraph_format.alignment = align or WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.keep_together = keep
    if bold_prefix and text.startswith(bold_prefix):
        r1 = p.add_run(bold_prefix)
        r1.bold = True
        r2 = p.add_run(text[len(bold_prefix):])
        r2.italic = italic
    else:
        run = p.add_run(text)
        run.italic = italic
    return p


def add_bullets(doc, items):
    for item in items:
        p = doc.add_paragraph(style="List Bullet")
        p.paragraph_format.left_indent = Cm(1)
        p.paragraph_format.first_line_indent = Cm(0)
        p.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        p.add_run(item)


def add_numbered(doc, items):
    for item in items:
        p = doc.add_paragraph(style="List Number")
        p.paragraph_format.left_indent = Cm(1)
        p.paragraph_format.first_line_indent = Cm(0)
        p.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        p.add_run(item)


def add_placeholder(doc, text):
    table = doc.add_table(rows=1, cols=1)
    table.style = "Table Grid"
    cell = table.cell(0, 0)
    set_cell_shading(cell, "FFF2CC")
    set_cell_text(cell, "[PHẦN CẦN BỔ SUNG] " + text, bold=True, center=False, size=11)
    doc.add_paragraph().paragraph_format.space_after = Pt(0)


def add_flow(doc, lines, caption=None):
    table = doc.add_table(rows=1, cols=1)
    table.style = "Table Grid"
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    cell = table.cell(0, 0)
    set_cell_shading(cell, "F2F2F2")
    cell.text = ""
    for index, line in enumerate(lines):
        p = cell.add_paragraph() if index else cell.paragraphs[0]
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_after = Pt(3)
        run = p.add_run(line)
        run.bold = "↓" not in line and "→" not in line
        run.font.name = "Consolas"
        run.font.size = Pt(10.5)
    if caption:
        p = doc.add_paragraph(caption)
        p.style = "Caption"
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER


def add_figure(doc, path, caption, width=6.2):
    if not path.exists():
        add_placeholder(doc, f"Chèn hình: {caption}")
        return
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run().add_picture(str(path), width=Inches(width))
    cp = doc.add_paragraph(caption)
    cp.style = "Caption"
    cp.alignment = WD_ALIGN_PARAGRAPH.CENTER


def add_heading(doc, text, level=1):
    return doc.add_heading(text, level=level)


doc = Document()
section = doc.sections[0]
section.top_margin = Cm(2)
section.bottom_margin = Cm(2)
section.left_margin = Cm(3)
section.right_margin = Cm(2)
section.header_distance = Cm(1)
section.footer_distance = Cm(1)

styles = doc.styles
normal = styles["Normal"]
normal.font.name = "Times New Roman"
normal._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
normal.font.size = Pt(13)
normal.paragraph_format.line_spacing_rule = WD_LINE_SPACING.ONE_POINT_FIVE
normal.paragraph_format.space_after = Pt(6)
normal.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY

for style_name, size, color in [
    ("Title", 20, "1F4E78"),
    ("Heading 1", 16, "1F4E78"),
    ("Heading 2", 14, "2F5597"),
    ("Heading 3", 13, "000000"),
]:
    style = styles[style_name]
    style.font.name = "Times New Roman"
    style._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
    style.font.size = Pt(size)
    style.font.bold = True
    style.font.color.rgb = RGBColor.from_string(color)
    style.paragraph_format.space_before = Pt(12)
    style.paragraph_format.space_after = Pt(6)
    style.paragraph_format.keep_with_next = True

caption = styles["Caption"]
caption.font.name = "Times New Roman"
caption._element.rPr.rFonts.set(qn("w:eastAsia"), "Times New Roman")
caption.font.size = Pt(11)
caption.font.italic = True

header = section.header.paragraphs[0]
header.alignment = WD_ALIGN_PARAGRAPH.RIGHT
run = header.add_run("Báo cáo môn học – Phát triển Game 2D và 3D")
run.font.name = "Times New Roman"
run.font.size = Pt(9)
run.font.italic = True
add_page_number(section.footer.paragraphs[0])

settings = doc.settings.element
update_fields = OxmlElement("w:updateFields")
update_fields.set(qn("w:val"), "true")
settings.append(update_fields)

# Trang bìa
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
p.paragraph_format.space_after = Pt(4)
r = p.add_run("[TÊN TRƯỜNG ĐẠI HỌC/Cao đẳng]")
r.bold = True
r.font.size = Pt(14)
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("[KHOA/BỘ MÔN]")
r.bold = True
r.font.size = Pt(14)

doc.add_paragraph("\n")
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("BÁO CÁO MÔN HỌC")
r.bold = True
r.font.size = Pt(22)
r.font.color.rgb = RGBColor(31, 78, 120)
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("PHÁT TRIỂN GAME 2D VÀ GAME 3D")
r.bold = True
r.font.size = Pt(18)
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("SẢN PHẨM 2D: DUNGEON SLAYER – BA CHÌA KHÓA PHONG ẤN")
r.bold = True
r.font.size = Pt(16)
r.font.color.rgb = RGBColor(47, 85, 151)
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("SẢN PHẨM 3D: [TÊN GAME BẮN SÚNG 3D]")
r.bold = True
r.font.size = Pt(14)

doc.add_paragraph("\n")
cover_rows = [
    ("Môn học", "[TÊN MÔN HỌC]"),
    ("Giảng viên hướng dẫn", "[HỌ VÀ TÊN GIẢNG VIÊN]"),
    ("Nhóm thực hiện", "[TÊN/SỐ NHÓM]"),
    ("Thành viên", "1. [Họ tên – MSSV]\n2. [Họ tên – MSSV]\n3. [Họ tên – MSSV]"),
    ("Lớp", "[TÊN LỚP]"),
]
table = doc.add_table(rows=0, cols=2)
table.alignment = WD_TABLE_ALIGNMENT.CENTER
for left, right in cover_rows:
    cells = table.add_row().cells
    set_cell_text(cells[0], left, bold=True, size=13)
    set_cell_text(cells[1], right, size=13)
    cells[0].width = Cm(5)
    cells[1].width = Cm(10)

doc.add_paragraph("\n\n")
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
p.add_run("[ĐỊA DANH], 2026").bold = True
doc.add_page_break()

# Nhận xét và phân công
add_heading(doc, "NHẬN XÉT CỦA GIẢNG VIÊN", 1)
for _ in range(12):
    doc.add_paragraph("................................................................................................................................................")
doc.add_page_break()

add_heading(doc, "PHÂN CÔNG NHIỆM VỤ", 1)
add_para(doc, "Bảng phân công dưới đây là khung tham khảo. Nhóm cần điền đúng họ tên, mã sinh viên, nhiệm vụ thực tế và tỷ lệ đóng góp trước khi nộp báo cáo.")
add_table(
    doc,
    ["STT", "Thành viên", "Nhiệm vụ chính", "Sản phẩm phụ trách", "Tỷ lệ"],
    [
        ("1", "[Họ tên – MSSV]", "Phân tích, lập trình gameplay, AI boss, tích hợp và kiểm thử", "Game 2D Dungeon Slayer", "[..]%"),
        ("2", "[Họ tên – MSSV]", "Thiết kế level, giao diện, tài liệu và kiểm thử", "Game 2D Dungeon Slayer", "[..]%"),
        ("3", "[Họ tên – MSSV]", "[Bổ sung nhiệm vụ]", "Game bắn súng 3D", "[..]%"),
    ],
    widths=[1, 4, 6, 4, 2],
)
add_placeholder(doc, "Điền thông tin thành viên và điều chỉnh nhiệm vụ theo đóng góp thực tế.")
doc.add_page_break()

add_heading(doc, "LỜI MỞ ĐẦU", 1)
add_para(doc, "Trong những năm gần đây, ngành công nghiệp trò chơi điện tử phát triển mạnh mẽ và trở thành một lĩnh vực giao thoa giữa công nghệ phần mềm, mỹ thuật số, âm thanh và thiết kế trải nghiệm người dùng. Việc xây dựng một trò chơi hoàn chỉnh đòi hỏi người phát triển phải vận dụng đồng thời kiến thức về lập trình hướng đối tượng, xử lý sự kiện, vật lý, trí tuệ nhân tạo, quản lý dữ liệu và tổ chức dự án. Vì vậy, phát triển game là một đề tài phù hợp để nhóm áp dụng kiến thức môn học vào một sản phẩm có khả năng chạy và kiểm thử trực tiếp.")
add_para(doc, "Trong khuôn khổ đồ án, nhóm lựa chọn xây dựng hai sản phẩm nhằm tiếp cận cả hai hướng phát triển phổ biến. Sản phẩm thứ nhất là trò chơi hành động đi cảnh 2D Dungeon Slayer, tập trung vào di chuyển, chiến đấu cận chiến và tầm xa, thu thập vật phẩm, vượt màn bằng chìa khóa và đối đầu với hệ thống miniboss, boss nhiều giai đoạn. Sản phẩm thứ hai là một trò chơi bắn súng 3D do thành viên khác phụ trách. Báo cáo hiện tại trình bày đầy đủ phần cơ sở lý thuyết dùng chung và toàn bộ quá trình phân tích, thiết kế, triển khai, kiểm thử của game 2D; các mục dành cho game 3D được bố trí sẵn để thành viên phụ trách tiếp tục hoàn thiện.")
add_para(doc, "Dungeon Slayer được phát triển trên Unity 6 bằng ngôn ngữ C#. Nhóm tiến hành phân tích yêu cầu, sửa lỗi, thiết kế cảnh quan, bổ sung cơ chế chìa khóa, lưu tiến trình, điều chỉnh giao diện, xây dựng cốt truyện và phát triển trí tuệ nhân tạo cho các trận đánh boss. Mục tiêu của nhóm không chỉ là tạo ra một bản chơi được mà còn xây dựng sản phẩm theo quy trình phần mềm gồm khảo sát, xác định yêu cầu, thiết kế, cài đặt, kiểm thử và đánh giá.")
add_para(doc, "Nhóm xin chân thành cảm ơn giảng viên đã hướng dẫn và góp ý trong quá trình thực hiện. Do thời gian và kinh nghiệm còn hạn chế, sản phẩm vẫn có thể tồn tại một số thiếu sót. Nhóm mong nhận được nhận xét để tiếp tục hoàn thiện trò chơi và nâng cao kỹ năng phát triển phần mềm.")
doc.add_page_break()

add_heading(doc, "MỤC LỤC", 1)
p = doc.add_paragraph()
add_field(p, 'TOC \\o "1-3" \\h \\z \\u')
doc.add_page_break()

add_heading(doc, "DANH MỤC TỪ VIẾT TẮT", 1)
add_table(doc, ["Từ viết tắt", "Nội dung"], [
    ("AI", "Artificial Intelligence – Trí tuệ nhân tạo"),
    ("API", "Application Programming Interface – Giao diện lập trình ứng dụng"),
    ("FPS", "First-Person Shooter – Trò chơi bắn súng góc nhìn thứ nhất"),
    ("FSM", "Finite State Machine – Máy trạng thái hữu hạn"),
    ("HP", "Health Point – Điểm sinh lực"),
    ("MP/Power", "Năng lượng sử dụng kỹ năng hoặc trạng thái cường hóa"),
    ("NPC", "Non-Player Character – Nhân vật không do người chơi điều khiển"),
    ("OOP", "Object-Oriented Programming – Lập trình hướng đối tượng"),
    ("RPG", "Role-Playing Game – Trò chơi nhập vai"),
    ("UI", "User Interface – Giao diện người dùng"),
    ("UX", "User Experience – Trải nghiệm người dùng"),
    ("URP", "Universal Render Pipeline – Hệ thống kết xuất đa nền tảng của Unity"),
], widths=[4, 12])

add_heading(doc, "DANH MỤC HÌNH VÀ BẢNG", 1)
add_para(doc, "Các hình và bảng trong báo cáo được đánh số theo chương. Khi bổ sung hoặc xóa hình, người biên tập cần cập nhật lại số thứ tự và mục lục bằng chức năng Update Field của Microsoft Word.")
add_placeholder(doc, "Trong Word, chọn toàn bộ tài liệu bằng Ctrl+A rồi nhấn F9 để cập nhật mục lục và số trang.")
doc.add_page_break()

add_heading(doc, "GIỚI THIỆU ĐỀ TÀI", 1)
add_heading(doc, "1. Lý do chọn đề tài", 2)
add_para(doc, "Trò chơi điện tử là một loại phần mềm tương tác có yêu cầu cao về phản hồi thời gian thực. Một thao tác của người chơi phải được tiếp nhận, xử lý và phản ánh tức thời bằng chuyển động, hoạt ảnh, âm thanh hoặc thay đổi trạng thái. Đề tài phát triển game cho phép nhóm vận dụng kiến thức lập trình theo cách trực quan, đồng thời tạo điều kiện nghiên cứu thêm về thiết kế trải nghiệm, cân bằng độ khó và tổ chức mã nguồn.")
add_para(doc, "Nhóm lựa chọn thực hiện song song game 2D và game 3D để so sánh hai quy trình phát triển. Game 2D phù hợp để tập trung vào cơ chế, nhịp độ chiến đấu và thiết kế màn chơi; game 3D giúp tiếp cận không gian ba chiều, camera phối cảnh và cơ chế bắn súng. Trong phạm vi tài liệu này, Dungeon Slayer là sản phẩm được mô tả và đánh giá chính.")

add_heading(doc, "2. Mục tiêu", 2)
add_bullets(doc, [
    "Xây dựng một game hành động đi cảnh 2D có thể chơi từ đầu đến cuối trên Unity.",
    "Thiết kế luồng gồm menu, ba màn thu thập chìa khóa và một màn boss cuối.",
    "Tạo hệ thống chiến đấu kết hợp kiếm, cung, nhảy, lướt và trạng thái cường hóa.",
    "Xây dựng miniboss và boss có nhiều phase, phản ứng khác nhau trước hành động của người chơi.",
    "Lưu màn và vị trí gần nhất để hỗ trợ Resume; cho phép New Game đặt lại tiến trình.",
    "Thiết kế giao diện thích hợp cho bàn phím, tay cầm và điều khiển cảm ứng Android.",
    "Thực hiện kiểm thử chức năng, kiểm thử luồng và đánh giá khả năng mở rộng.",
    "Tạo khung báo cáo chung để tích hợp phần game bắn súng 3D của nhóm.",
])

add_heading(doc, "3. Phạm vi", 2)
add_para(doc, "Game 2D bao gồm năm scene: Menu, Scene_1, Scene_2, Scene_3 và Scene_Final_1. Phạm vi chức năng gồm điều khiển nhân vật, hệ thống chỉ số, chiến đấu, vật phẩm, hội thoại, checkpoint, cổng dịch chuyển, miniboss, boss cuối và màn hình thắng/thua. Dữ liệu được lưu cục bộ bằng PlayerPrefs; sản phẩm chưa triển khai tài khoản trực tuyến, máy chủ, bảng xếp hạng mạng hoặc chế độ nhiều người chơi.")
add_placeholder(doc, "Thành viên phụ trách game 3D bổ sung phạm vi game bắn súng: số màn, chế độ chơi, nền tảng và các chức năng chính.")

add_heading(doc, "4. Phương pháp thực hiện", 2)
add_numbered(doc, [
    "Khảo sát yêu cầu, xác định tài nguyên, scene, prefab và cấu trúc script cần thiết.",
    "Phân tích lỗi và yêu cầu cải tiến; xây dựng backlog theo mức độ ảnh hưởng đến luồng chơi.",
    "Thiết kế cốt truyện, cơ chế chìa khóa, đặc điểm từng màn và độ khó boss tăng dần.",
    "Cài đặt theo từng mô-đun; kiểm tra biên dịch và chạy Play Mode sau mỗi thay đổi quan trọng.",
    "Kiểm thử xuyên suốt từ New Game đến chiến thắng, kiểm tra Resume, Replay và lưu dữ liệu.",
    "Tổng hợp kết quả, hạn chế và định hướng phát triển trong báo cáo.",
])
doc.add_page_break()

# CHƯƠNG 1
add_heading(doc, "CHƯƠNG 1. CƠ SỞ LÝ THUYẾT", 1)
add_heading(doc, "1.1. Tổng quan về trò chơi điện tử", 2)
add_para(doc, "Trò chơi điện tử là hệ thống phần mềm nhận đầu vào từ người chơi, cập nhật trạng thái thế giới theo bộ luật và trình bày kết quả thông qua hình ảnh, âm thanh hoặc phản hồi rung. Khác với phần mềm quản lý dữ liệu thông thường, vòng lặp game được thực hiện liên tục theo khung hình. Mỗi khung hình thường gồm ba nhóm công việc: đọc đầu vào, cập nhật logic và kết xuất.")
add_flow(doc, ["Đọc đầu vào", "↓", "Cập nhật trạng thái và vật lý", "↓", "Tính toán AI – va chạm – điểm số", "↓", "Kết xuất hình ảnh, UI và âm thanh", "↓", "Khung hình tiếp theo"], "Hình 1.1. Vòng lặp xử lý cơ bản của trò chơi")
add_para(doc, "Một sản phẩm game hoàn chỉnh thường có bốn thành phần: gameplay, nội dung, trình bày và hạ tầng kỹ thuật. Gameplay quy định người chơi có thể làm gì và thắng bằng cách nào. Nội dung gồm cốt truyện, màn chơi, nhân vật và thử thách. Trình bày bao gồm đồ họa, giao diện và âm thanh. Hạ tầng kỹ thuật xử lý lưu dữ liệu, chuyển scene, quản lý tài nguyên và khả năng chạy trên nhiều nền tảng.")

add_heading(doc, "1.2. Game 2D và game 3D", 2)
add_para(doc, "Game 2D biểu diễn đối tượng chủ yếu trên hai trục X và Y. Đồ họa thường được tạo từ sprite, sprite sheet, tilemap và các lớp nền. Camera thường sử dụng phép chiếu trực giao, giúp kích thước vật thể không thay đổi theo chiều sâu. Game 3D bổ sung trục Z, sử dụng mesh, vật liệu, ánh sáng, camera phối cảnh và hệ thống va chạm không gian. Sự khác nhau này ảnh hưởng trực tiếp đến thiết kế level, điều khiển, xử lý camera và chi phí sản xuất tài nguyên.")
add_table(doc, ["Tiêu chí", "Game 2D", "Game 3D"], [
    ("Không gian", "Hai chiều X–Y; chiều sâu thường dùng để sắp lớp", "Ba chiều X–Y–Z"),
    ("Tài nguyên", "Sprite, tilemap, animation 2D", "Mesh, material, texture, rig và animation 3D"),
    ("Camera", "Thường trực giao, đi ngang hoặc từ trên xuống", "Phối cảnh, góc nhìn thứ nhất hoặc thứ ba"),
    ("Vật lý", "Collider2D, Rigidbody2D", "Collider, Rigidbody và raycast 3D"),
    ("Thiết kế level", "Chú trọng nền tảng, khoảng nhảy và đường đi", "Chú trọng không gian, tầm nhìn, che chắn và định hướng"),
    ("Sản phẩm nhóm", "Dungeon Slayer – hành động đi cảnh", "[Game bắn súng 3D – bổ sung]"),
], widths=[3, 6.5, 6.5])

add_heading(doc, "1.3. Thể loại game và chủ đề của nhóm", 2)
add_para(doc, "Dungeon Slayer kết hợp ba nhóm thể loại. Thứ nhất, đây là platformer vì người chơi di chuyển ngang, nhảy, nhảy kép và vượt qua địa hình. Thứ hai, trò chơi mang tính action RPG thông qua chỉ số HP, sức tấn công, phòng thủ, vật phẩm tăng chỉ số và các trận boss. Thứ ba, cấu trúc dungeon crawler được thể hiện ở việc khám phá từng khu vực, tiêu diệt quái, thu thập chiến lợi phẩm và mở khóa cổng sang khu vực tiếp theo.")
add_para(doc, "Chủ đề của game 2D là giả tưởng trung cổ pha yếu tố ma thuật, kể về hành trình thu hồi ba chìa khóa phong ấn và đánh bại pháp sư Malakar. Phong cách pixel art giúp tài nguyên có tính thống nhất, phù hợp với game 2D và giảm chi phí dựng hình so với đồ họa 3D. Game bắn súng 3D thuộc nhóm FPS hoặc TPS tùy góc nhìn do thành viên phụ trách lựa chọn.")

add_heading(doc, "1.4. Thiết kế gameplay", 2)
add_para(doc, "Gameplay được hình thành từ mục tiêu, luật chơi, hành động và phản hồi. Mục tiêu trả lời người chơi cần đạt điều gì; luật quy định điều kiện hợp lệ; hành động là tập thao tác mà người chơi có thể thực hiện; phản hồi giúp người chơi hiểu kết quả. Trong Dungeon Slayer, mục tiêu của mỗi màn là hạ miniboss, nhặt chìa khóa và đến cổng. Luật ngăn cổng hoạt động nếu chưa có chìa khóa. Hành động gồm di chuyển, nhảy, chém, bắn cung, lướt và cường hóa. Phản hồi được thể hiện bằng hoạt ảnh, đổi màu khi trúng đòn, rung camera, thanh máu và thông báo mục tiêu.")
add_para(doc, "Khái niệm core loop mô tả chuỗi hành động lặp lại tạo nên trải nghiệm chính. Core loop của Dungeon Slayer là khám phá – chiến đấu – nhận phần thưởng – tăng sức mạnh – vượt thử thách lớn hơn. Ngoài core loop, trò chơi có meta progression thông qua lưu điểm, sức tấn công, phòng thủ, màn đã mở và checkpoint.")

add_heading(doc, "1.5. Cân bằng độ khó và thiết kế boss", 2)
add_para(doc, "Độ khó tốt không chỉ đến từ việc tăng HP hoặc sát thương mà còn từ việc bổ sung mẫu tấn công mới, giảm vùng an toàn và yêu cầu người chơi thay đổi chiến thuật. Boss nhiều phase là một phương pháp phổ biến: ngưỡng máu đóng vai trò điều kiện chuyển trạng thái, mỗi phase thay đổi tốc độ, kích thước, kỹ năng hoặc hành vi phản ứng.")
add_para(doc, "Một kỹ năng công bằng cần có ba giai đoạn: báo hiệu, thực thi và hồi phục. Báo hiệu cho phép người chơi nhận biết; thực thi tạo nguy hiểm; hồi phục mở ra cơ hội phản công. Cơ chế lao của miniboss Bee sử dụng nhấp nháy cảnh báo trước khi lao. Mưa đá của boss cuối hiển thị dấu cảnh báo, chờ 0,8 giây, tạo đá rơi và dành hai làn an toàn để người chơi né.")

add_heading(doc, "1.6. Trí tuệ nhân tạo trong game", 2)
add_para(doc, "AI trong game không nhất thiết phải sử dụng học máy. Phần lớn hành vi được xây dựng bằng luật, máy trạng thái hữu hạn và lựa chọn theo điều kiện. Một FSM gồm tập trạng thái, điều kiện chuyển trạng thái và hành động tương ứng. Ví dụ, kẻ địch có thể tuần tra, phát hiện người chơi, tấn công, bị choáng và chết. Cách tiếp cận này dễ kiểm soát, dễ cân bằng và phù hợp với quy mô đồ án.")
add_flow(doc, ["PATROL", "→ phát hiện người chơi →", "CHASE/ATTACK", "→ HP dưới ngưỡng →", "ENRAGED / PHASE 2", "→ HP rất thấp →", "PHASE 3", "→ HP ≤ 0 →", "DEAD"], "Hình 1.2. Mô hình trạng thái tổng quát của boss")

add_heading(doc, "1.7. Công nghệ Unity và C#", 2)
add_para(doc, "Unity là game engine hỗ trợ tổ chức thế giới theo Scene và GameObject. Mỗi GameObject được mở rộng bằng Component như Transform, SpriteRenderer, Animator, Rigidbody2D, Collider2D và các MonoBehaviour viết bằng C#. Cách tổ chức theo thành phần cho phép tái sử dụng prefab, tách trách nhiệm và cấu hình dữ liệu bằng Inspector.")
add_para(doc, "Dự án sử dụng Unity 6000.6.0f1, Universal Render Pipeline 17.6.0, hệ thống 2D Sprite, 2D Tilemap, Physics 2D, Animator, TextMeshPro/uGUI và Input System. Ngôn ngữ C# được dùng để lập trình gameplay, AI, chuyển scene và lưu tiến trình. Các khái niệm OOP được áp dụng qua lớp, đóng gói trạng thái, tái sử dụng component và phân tách mô-đun.")

add_heading(doc, "1.8. Sprite, Tilemap, Animation và Physics 2D", 2)
add_para(doc, "Sprite là ảnh hai chiều được Unity nhập thành tài nguyên có pivot, Pixels Per Unit và bộ lọc. Với pixel art, Point Filter giúp giữ cạnh ảnh sắc nét. Tilemap cho phép ghép các tile thành nền đất, tường và platform, đồng thời có thể kết hợp TilemapCollider2D để tạo va chạm. Animator điều khiển các trạng thái đứng, chạy, nhảy, tấn công và chết thông qua parameter và transition.")
add_para(doc, "Physics 2D sử dụng Rigidbody2D và Collider2D. Rigidbody2D quyết định đối tượng tham gia mô phỏng; collider mô tả vùng va chạm; trigger dùng để phát hiện giao nhau mà không tạo lực đẩy. Trong game, vật phẩm và cổng dùng trigger, nhân vật và nền dùng collider vật lý, còn đá thiên thạch dùng Rigidbody2D Kinematic cùng CircleCollider2D để phát hiện người chơi hoặc mặt đất.")

add_heading(doc, "1.9. Giao diện và trải nghiệm người dùng", 2)
add_para(doc, "UI phải truyền đạt trạng thái quan trọng mà không che khu vực chiến đấu. HUD của game hiển thị HP, năng lượng, tiền, ngọc, sao, sức tấn công và phòng thủ. Trên thiết bị di động, joystick được đặt bên trái và các nút hành động ở bên phải. Thông báo mục tiêu chỉ xuất hiện khi người chơi đứng gần NPC và được đặt trong khung nền có độ tương phản để dễ đọc.")

add_heading(doc, "1.10. Lưu tiến trình", 2)
add_para(doc, "Đối với game đơn người chơi quy mô nhỏ, PlayerPrefs có thể lưu các giá trị đơn giản theo cặp khóa–giá trị. Dungeon Slayer lưu scene checkpoint, tọa độ nhân vật, màn đã mở, điểm, tiền, ngọc, sao, sức tấn công và phòng thủ. Resume đọc checkpoint để tiếp tục; New Game xóa hoặc đặt lại các khóa liên quan. PlayerPrefs phù hợp với prototype nhưng chưa có mã hóa và không thích hợp cho dữ liệu phức tạp hoặc chống gian lận.")

add_heading(doc, "1.11. Kiểm thử game", 2)
add_para(doc, "Kiểm thử game kết hợp kiểm thử phần mềm và kiểm thử trải nghiệm. Kiểm thử chức năng xác nhận nút, cổng, vật phẩm và sát thương hoạt động đúng. Kiểm thử luồng xác nhận người chơi có thể đi từ đầu đến cuối. Kiểm thử cân bằng xem thử thách có tăng hợp lý và còn khả năng né. Kiểm thử hồi quy bảo đảm một thay đổi ở boss không làm mất sát thương của người chơi hoặc ảnh hưởng các màn khác.")

add_heading(doc, "1.12. Định hướng game bắn súng 3D", 2)
add_para(doc, "Về lý thuyết, game bắn súng 3D sử dụng camera phối cảnh, raycast hoặc projectile, hệ thống tâm ngắm, quản lý đạn, phản hồi khi bắn, AI di chuyển trong không gian và bố trí vật cản. Nếu là FPS, camera đặt tại vị trí mắt nhân vật; nếu là TPS, camera theo sau nhân vật. Hệ thống NavMesh có thể hỗ trợ kẻ địch tìm đường.")
add_placeholder(doc, "Bổ sung tên game 3D, thể loại FPS/TPS, chủ đề, công nghệ, cơ chế bắn và định hướng AI của sản phẩm thứ hai.")

add_heading(doc, "1.13. Kết luận chương", 2)
add_para(doc, "Chương 1 đã trình bày cơ sở lý thuyết cho phát triển game 2D và 3D, đồng thời làm rõ các khái niệm được áp dụng trong Dungeon Slayer: vòng lặp game, platformer, action RPG, sprite, tilemap, Physics 2D, FSM, UI, lưu tiến trình và kiểm thử. Đây là nền tảng để phân tích và thiết kế hệ thống ở chương tiếp theo.")
doc.add_page_break()

# CHƯƠNG 2
add_heading(doc, "CHƯƠNG 2. PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG", 1)
add_heading(doc, "2.1. Mô tả bài toán", 2)
add_para(doc, "Bài toán đặt ra là xây dựng một trò chơi đi cảnh 2D có cốt truyện và luồng hoàn chỉnh. Người chơi phải có khả năng bắt đầu mới, tiếp tục tiến trình, điều khiển nhân vật, chiến đấu với nhiều loại quái, nâng cấp chỉ số, vượt qua ba người canh giữ và đánh bại boss cuối. Hệ thống phải tạo sự khác biệt giữa các màn, cung cấp phản hồi rõ ràng và không cho phép bỏ qua thử thách chính bằng cách chạy thẳng đến cổng.")
add_para(doc, "Trong quá trình phân tích, nhóm xác định các vấn đề trọng tâm cần giải quyết gồm: gắn điều kiện qua màn với mục tiêu cụ thể, tạo hành vi khác biệt cho từng boss, hoàn thiện Resume và lưu tiến trình, đồng bộ cảnh quan, bố trí thông báo không che HUD, đồng thời xử lý lỗi va chạm và sát thương. Các yêu cầu này được ưu tiên triển khai để tạo thành một luồng chơi hoàn chỉnh.")

add_heading(doc, "2.2. Tác nhân và nhu cầu", 2)
add_table(doc, ["Tác nhân", "Nhu cầu"], [
    ("Người chơi", "Bắt đầu mới, tiếp tục, điều khiển nhân vật, chiến đấu, thu thập, vượt màn, tạm dừng và xem kết quả."),
    ("Nhà phát triển", "Cấu hình scene, prefab, chỉ số, AI, tài nguyên; theo dõi Console và mở rộng màn chơi."),
    ("Hệ thống", "Cập nhật vật lý, phát hiện va chạm, quản lý trạng thái, lưu dữ liệu và chuyển scene."),
    ("Giảng viên/người đánh giá", "Quan sát được chức năng, phần cải tiến, quy trình thiết kế và kết quả kiểm thử."),
], widths=[4, 12])

add_heading(doc, "2.3. Yêu cầu chức năng", 2)
functional_rows = [
    ("FR-01", "Menu", "Hiển thị Resume, New Game, Options và Exit theo bốn hàng."),
    ("FR-02", "Tiến trình", "Resume từ menu tải scene và vị trí được lưu gần nhất."),
    ("FR-03", "Tiến trình", "New Game đặt lại checkpoint, điểm và chỉ số nâng cấp."),
    ("FR-04", "Điều khiển", "Di chuyển trái/phải bằng bàn phím, tay cầm hoặc joystick cảm ứng."),
    ("FR-05", "Điều khiển", "Nhảy và nhảy kép; hỗ trợ lướt nhanh có ảnh dư."),
    ("FR-06", "Chiến đấu", "Tấn công cận chiến bằng kiếm và tầm xa bằng cung."),
    ("FR-07", "Chỉ số", "Quản lý HP, Power, Attack và Defense; cập nhật HUD."),
    ("FR-08", "Cường hóa", "Khi đủ Power, kích hoạt Awakening trong thời gian giới hạn."),
    ("FR-09", "Vật phẩm", "Thu thập coin, gem, star, heart, sword và shield."),
    ("FR-10", "Nhiệm vụ", "Miniboss chết làm xuất hiện chìa khóa tại vị trí chết."),
    ("FR-11", "Cổng", "Cổng chỉ chuyển scene khi người chơi đã nhặt chìa khóa."),
    ("FR-12", "Hội thoại", "NPC hiển thị hội thoại và mục tiêu khi người chơi ở gần."),
    ("FR-13", "AI", "Mỗi miniboss có hành vi phase riêng; độ khó tăng theo màn."),
    ("FR-14", "Boss cuối", "Boss có ba phase, dịch chuyển, bắn phép và mưa đá."),
    ("FR-15", "Kết quả", "Hỗ trợ Game Over, Replay/Resume và màn hình chiến thắng."),
]
add_table(doc, ["Mã", "Nhóm", "Yêu cầu"], functional_rows, widths=[2, 3, 11])

add_heading(doc, "2.4. Yêu cầu phi chức năng", 2)
add_table(doc, ["Mã", "Yêu cầu", "Tiêu chí"], [
    ("NFR-01", "Khả dụng", "Phản hồi điều khiển rõ ràng; HUD không che khu vực chiến đấu quan trọng."),
    ("NFR-02", "Hiệu năng", "Duy trì tốc độ khung hình ổn định trên máy tính phổ thông và thiết bị Android mục tiêu."),
    ("NFR-03", "Tương thích", "Hoạt động với bàn phím, gamepad và điều khiển cảm ứng."),
    ("NFR-04", "Bảo trì", "Mã nguồn chia theo Camera, Menu, Player, Enemies, Portal, Scores và Dialogues."),
    ("NFR-05", "Tin cậy", "Không xuất hiện lỗi biên dịch; checkpoint không trỏ ra ngoài Build Settings."),
    ("NFR-06", "Nhất quán", "Sprite, background và hiệu ứng sử dụng phong cách pixel art thống nhất."),
    ("NFR-07", "Công bằng", "Kỹ năng mạnh có cảnh báo và khoảng hồi phục; sát thương phòng thủ vẫn có mức tối thiểu."),
], widths=[2, 5, 9])

add_heading(doc, "2.5. Cốt truyện: Ba Chìa Khóa Phong Ấn", 2)
add_heading(doc, "2.5.1. Bối cảnh và nhân vật", 3)
add_para(doc, "Vương quốc Eldoria từng được bảo vệ bởi Trái Tim Ánh Sáng, nguồn năng lượng cổ đại nằm sâu trong một hang động pha lê. Pháp sư hoàng gia Malakar được giao nhiệm vụ bảo vệ nguồn sức mạnh này. Vì muốn vượt qua cái chết, hắn thực hiện nghi lễ cấm và làm năng lượng biến thành lời nguyền. Các chiến binh cổ đại phong ấn Malakar trong hang động, chia chìa khóa thành ba phần và trao cho ba người canh giữ.")
add_para(doc, "Qua nhiều năm, lời nguyền rò rỉ ra ngoài, biến sinh vật thành quái vật và tha hóa chính những người canh giữ. Aren, hậu duệ cuối cùng của đội quân phong ấn, nhận thanh kiếm cổ từ người thầy và lên đường tìm ba chìa khóa. Mục tiêu của Aren là mở cánh cổng vực sâu, đánh bại Malakar và thanh lọc Trái Tim Ánh Sáng.")

add_heading(doc, "2.5.2. Diễn biến", 3)
story_rows = [
    ("Mở đầu", "Làng của Aren bị quái vật tấn công. Người thầy giao nhiệm vụ thu hồi ba chìa khóa và hoàn thành phong ấn."),
    ("Màn 1 – Đồng cỏ", "Aren giải thoát chiến binh canh giữ bị nguyền. Phase 2 tạo bốn phân thân. Nhận Chìa khóa Lòng Dũng Cảm."),
    ("Màn 2 – Rừng bị nguyền", "Aren đối đầu sinh vật bay bảo vệ khu rừng. Nó nổi giận, lao xuống vị trí người chơi. Nhận Chìa khóa Sự Sống."),
    ("Màn 3 – Tàn tích bóng tối", "Người canh giữ cuối thay đổi kích thước và dịch chuyển. Trước khi tan biến, ông cảnh báo Malakar muốn chiếm Trái Tim Ánh Sáng. Aren nhận Chìa khóa Ý Chí."),
    ("Màn cuối", "Ba chìa khóa mở hang động. Malakar tiết lộ các guardian vừa bảo vệ chìa khóa vừa ngăn hắn thoát ra. Trận chiến ba phase bắt đầu."),
    ("Kết thúc", "Malakar bị đánh bại và hối hận. Aren đặt ba chìa khóa vào Trái Tim Ánh Sáng, thanh lọc lời nguyền và giải thoát các guardian."),
]
add_table(doc, ["Giai đoạn", "Nội dung"], story_rows, widths=[4, 12])

add_heading(doc, "2.5.3. Cao trào và kết thúc", 3)
add_para(doc, "Ở phase cuối, Malakar mất kiểm soát và tuyên bố nếu hắn không thể sở hữu Trái Tim Ánh Sáng thì không ai có thể. Hắn dịch chuyển liên tục và triệu hồi mưa thiên thạch. Sau khi bị đánh bại, Malakar trở lại hình dạng con người trong giây lát, thừa nhận chính nỗi sợ cái chết đã biến hắn thành quái vật. Aren sửa chữa phong ấn, ánh sáng lan khắp vương quốc và trò chơi kết thúc bằng màn hình Victory cùng thống kê thành tích.")

add_heading(doc, "2.6. Luồng tổng thể", 2)
add_flow(doc, ["Khởi động", "↓", "MENU", "↓", "RESUME hoặc NEW GAME", "↓", "SCENE 1 → SCENE 2 → SCENE 3", "↓", "Hạ miniboss → nhặt chìa khóa → mở cổng", "↓", "SCENE FINAL – BOSS 3 PHASE", "↓", "VICTORY → MAIN MENU"], "Hình 2.1. Luồng tổng thể của game 2D")
add_para(doc, "Nếu nhân vật hết HP, hệ thống chuyển sang trạng thái Game Over. Người chơi có thể chơi lại và tiếp tục từ checkpoint của scene. Khi ứng dụng bị tạm dừng hoặc đóng, vị trí hiện tại được lưu để Resume từ menu có thể đưa nhân vật trở lại khu vực gần nhất.")

add_heading(doc, "2.7. Core loop và luồng mỗi màn", 2)
add_flow(doc, ["Khám phá khu vực", "↓", "Đánh quái – né bẫy – thu thập vật phẩm", "↓", "Gặp NPC và đọc mục tiêu", "↓", "Đối đầu miniboss", "↓", "Miniboss chết và rơi chìa khóa", "↓", "Nhặt chìa khóa", "↓", "Đi tới cổng để qua màn"], "Hình 2.2. Vòng lặp của một màn chơi")

add_heading(doc, "2.8. Thiết kế điều khiển và chiến đấu", 2)
add_table(doc, ["Hành động", "Bàn phím", "Gamepad", "Mobile", "Mô tả"], [
    ("Di chuyển", "A/D hoặc mũi tên", "Analog/D-Pad", "Joystick trái", "Di chuyển ngang, đổi hướng sprite."),
    ("Nhảy", "Space", "A/Cross", "Nút mũi tên", "Nhảy và nhảy kép."),
    ("Chém", "F", "X/Square", "Nút kiếm", "OverlapCircle tại attack point; lấy damage từ Stats."),
    ("Bắn cung", "C", "Y/Triangle", "Nút cung", "Animation Event tạo Arrow bay theo hướng nhân vật."),
    ("Lướt", "Left Shift", "RB", "Nút kỹ năng", "Tăng vận tốc trong 0,2 giây và tạo afterimage."),
    ("Awakening", "R", "B/Circle", "Nút kỹ năng", "Gấp đôi damage và tăng 50 defense trong 10 giây."),
    ("Tạm dừng", "Esc", "Start", "Nút menu", "Dừng Time.timeScale và mở menu trong game."),
], widths=[2.5, 3, 3, 3, 5])

add_heading(doc, "2.9. Thiết kế chỉ số và sát thương", 2)
add_para(doc, "Nhân vật bắt đầu với 200 HP và 100 Attack. Kiếm sử dụng Attack hiện tại; cung gây khoảng 140% Attack. Defense giảm sát thương nhưng không thể tạo trạng thái bất tử. Công thức bảo đảm tối thiểu 20% sát thương gốc vẫn xuyên qua phòng thủ:")
p = doc.add_paragraph()
p.alignment = WD_ALIGN_PARAGRAPH.CENTER
r = p.add_run("MinimumDamage = max(1, ceil(Damage × 0,20))\nActualDamage = max(MinimumDamage, Damage − Defense)")
r.bold = True
r.font.name = "Cambria Math"
add_para(doc, "Cách tính này giữ giá trị của vật phẩm Shield nhưng tránh lỗi người chơi không mất HP khi Defense quá cao. Heart hồi máu, Sword tăng Attack, Shield tăng Defense, còn Star/Power phục vụ trạng thái Awakening.")

add_heading(doc, "2.10. Thiết kế màn chơi", 2)
add_table(doc, ["Scene", "Bối cảnh", "Guardian/Boss", "Điểm nhấn gameplay", "Mục tiêu"], [
    ("Scene_1", "Đồng cỏ và núi", "Knight – 1.500 HP", "Làm quen điều khiển; phase 2 tạo bốn clone", "Chìa khóa Lòng Dũng Cảm"),
    ("Scene_2", "Rừng nhiệt đới và hầm", "Bee – 1.800 HP", "Bay tuần tra; phase 2 khóa tọa độ X/Y rồi lao xuống", "Chìa khóa Sự Sống"),
    ("Scene_3", "Tàn tích bóng tối", "Vampire – 3.600 HP", "Ba phase; phóng to, thu nhỏ, né đòn và dịch chuyển sau lưng", "Chìa khóa Ý Chí"),
    ("Scene_Final_1", "Hang động pha lê tím", "Wizard – 5.000 HP", "Ba phase; bắn phép, blink và mưa đá có vùng an toàn", "Phá lời nguyền"),
], widths=[2.5, 3.5, 3.5, 5.5, 3])

add_figure(doc, IMAGES["map1"], "Hình 2.3. Giao diện gameplay và bối cảnh màn 1")
add_figure(doc, IMAGES["map2"], "Hình 2.4. Bối cảnh rừng và khu vực màn 2")
add_figure(doc, IMAGES["final"], "Hình 2.5. Trận chiến với boss cuối trong hang động pha lê")

add_heading(doc, "2.11. Thiết kế AI miniboss và boss", 2)
add_heading(doc, "2.11.1. Miniboss Knight", 3)
add_para(doc, "Knight là thử thách đầu tiên. Khi HP còn 45%, damage gốc được tăng và boss tạo bốn phân thân ở các vị trí xung quanh. Mỗi clone có lượng máu bằng khoảng một phần tư máu ban đầu, không rơi vật phẩm và chỉ gây 35% damage kế thừa. Boss chính được giảm 50% sát thương nhận vào trong bốn giây đầu sau khi gọi clone để tạo khoảnh khắc chuyển phase nhưng không kéo dài vô hạn.")

add_heading(doc, "2.11.2. Miniboss Bee", 3)
add_para(doc, "Bee không sử dụng phân thân để tránh lặp lại màn 1. Khi còn 50% HP, tốc độ tuần tra tăng 2,8 lần, sát thương tiếp xúc tăng và kích thước tăng 1,3 lần. Mỗi đợt lao có 0,45 giây nhấp nháy cảnh báo, 0,18 giây di chuyển đến đúng vị trí X/Y đã khóa của người chơi, 0,5 giây hồi phục và 2,7 giây hồi chiêu. Sau khi lao, Bee lùi về độ cao sân đấu.")

add_heading(doc, "2.11.3. Miniboss Vampire", 3)
add_para(doc, "Vampire có ba phase. Phase 1 sử dụng hành vi cơ bản. Ở 60% HP, boss phóng lớn 1,45 lần, tăng damage và có 55% xác suất dịch chuyển né đòn với cooldown một giây. Ở 25% HP, boss thu nhỏ còn 68%, tốc độ di chuyển đạt 3,2 lần, vùng tấn công tăng và xác suất né đạt 70%. Dịch chuyển áp sát sau lưng được giới hạn khoảng hai giây một lần để tránh gây cảm giác bất công.")

add_heading(doc, "2.11.4. Boss cuối Malakar", 3)
add_table(doc, ["Phase", "Ngưỡng", "Hình thái", "Hành vi"], [
    ("1", "100%–60%", "Bình thường", "Di chuyển và bắn phép cơ bản; giúp người chơi đọc nhịp."),
    ("2", "60%–30%", "Phóng lớn, màu tím", "Phản ứng khi trúng đòn bằng BlinkAway; bắn quạt ba viên phép mỗi 3,2 giây; dịch chuyển sau lưng."),
    ("3", "Dưới 30%", "Thu nhỏ, màu xanh", "Chuỗi mưa đá → hồi phục → bắn quạt → dịch chuyển. Khi bị đánh có thể phản ứng sau cooldown 2,2 giây."),
], widths=[2, 3, 4, 8])
add_para(doc, "Mưa đá chia khu vực quanh người chơi thành chín làn, bỏ trống hai làn liền nhau và thả đá ở bảy làn còn lại. Dấu chấm than cảnh báo tồn tại 0,8 giây. Đá sử dụng sprite pixel art riêng, rơi với tốc độ 5,5 đơn vị/giây, gây 35 damage, biến mất khi chạm người hoặc địa hình và tự hủy sau bốn giây. Object đá được đặt cùng layer với boss để camera hiển thị đúng.")
add_figure(doc, IMAGES["meteor"], "Hình 2.6. Hiệu ứng mưa đá của boss cuối sau khi hoàn thiện", width=6.3)

add_heading(doc, "2.12. Thiết kế chìa khóa, cổng và mục tiêu", 2)
add_para(doc, "Mỗi scene có một KeyPickup riêng. Khi bắt đầu scene, Portal đặt chìa khóa ở trạng thái ẩn, tìm guardian phù hợp theo tên Knight, Bee hoặc Vampire và theo dõi Enemy.health. Khi guardian bị hạ, chìa khóa được đặt đúng vị trí chết và kích hoạt. Khi người chơi chạm chìa khóa, biến tĩnh HasKey chuyển thành true. Portal đổi từ màu xám sang màu bình thường và chỉ chuyển sang scene kế tiếp nếu HasKey bằng true.")
add_para(doc, "ObjectivePanel chỉ bật khi nhân vật đứng gần NPC. Nội dung thay đổi theo trạng thái: hạ guardian để lấy chìa khóa; tìm chìa khóa; hoặc đi tới cổng khi đã có chìa khóa. Thiết kế này giảm việc thông báo che HUD trong suốt quá trình chơi.")

add_heading(doc, "2.13. Kiến trúc phần mềm", 2)
add_flow(doc, ["LỚP GIAO DIỆN", "Menu – HUD – ObjectivePanel – DialogueManager – Mobile Controls", "↓", "LỚP GAMEPLAY", "PlayerController – Stats – Combat – Bow – Dash – Enemy AI", "↓", "LỚP TIẾN TRÌNH", "Portal – KeyPickup – LevelManager – GameProgress – ScoreManager", "↓", "UNITY ENGINE", "SceneManager – Physics2D – Animator – Audio – PlayerPrefs"], "Hình 2.7. Kiến trúc phân lớp logic của game")
add_table(doc, ["Mô-đun", "Script tiêu biểu", "Trách nhiệm"], [
    ("Player", "PlayerController, Stats, PlayerCombat, Bow, Arrow, DashSkill", "Điều khiển, chiến đấu, chỉ số và kỹ năng."),
    ("Enemies", "Enemy, Patrol, BossWeapon, MinibossController", "Máu, di chuyển, damage và phase miniboss."),
    ("Final Boss", "FinalWizardBossAI, MageWeapon, MageBall, MeteorRock", "AI ba phase và các đòn phép đặc biệt."),
    ("Progress", "LevelManager, GameProgress, Portal, KeyPickup", "Checkpoint, lưu vị trí, điều kiện qua màn."),
    ("UI", "Menu, ObjectivePanel, DialogueManager, HealthBar", "Menu, HUD, hội thoại và thông báo."),
    ("Scores", "ScoreManager, Coin, Gem, Star, Heart, Shield, Attack", "Điểm, vật phẩm và nâng cấp."),
], widths=[3, 6, 7])

add_heading(doc, "2.14. Thiết kế dữ liệu lưu", 2)
add_table(doc, ["Khóa PlayerPrefs", "Kiểu", "Ý nghĩa"], [
    ("CheckpointScene", "int", "Build index của scene tiếp tục."),
    ("CheckpointPositionScene", "int", "Scene chứa vị trí đã lưu."),
    ("CheckpointPositionX/Y", "float", "Tọa độ nhân vật khi tạm dừng/thoát."),
    ("MaxUnlockedLevel", "int", "Màn cao nhất đã mở."),
    ("AttackDamage", "int", "Sức tấn công đã nâng cấp."),
    ("Defense", "int", "Phòng thủ đã nâng cấp."),
    ("Score", "int", "Tổng điểm."),
    ("ScoreCoins/Gems/Stars", "int", "Số lượng vật phẩm tích lũy."),
    ("AudioVolume", "float", "Âm lượng do người chơi thiết lập."),
], widths=[6, 2, 8])

add_heading(doc, "2.15. Thiết kế giao diện", 2)
add_para(doc, "Menu chính sử dụng bốn hàng: Resume, New Game, Options và Exit. Resume chỉ tải checkpoint khi đang ở scene Menu; Resume trong pause menu chỉ đóng menu và tiếp tục scene hiện tại. HUD đặt chân dung, HP, Power và thống kê ở góc trái; joystick ở dưới trái; nút cung, kiếm và nhảy ở bên phải. Thanh máu boss được đặt phía trên để người chơi theo dõi phase.")
add_para(doc, "Màu sắc được sử dụng như phản hồi trạng thái: cổng xám khi khóa; guardian đỏ để phân biệt miniboss; phase mạnh chuyển tím/cam/xanh; nhân vật lóe đỏ khi bị đánh. Các background và middleground được điều chỉnh theo từng khu vực nhưng vẫn giữ độ phân giải và phong cách pixel art thống nhất của toàn bộ trò chơi.")

add_heading(doc, "2.16. Các use case chính", 2)
add_table(doc, ["Use case", "Tiền điều kiện", "Luồng chính", "Ngoại lệ"], [
    ("UC-01 New Game", "Đang ở Menu", "Xóa tiến trình → tải Scene_1", "Nếu chưa có dữ liệu vẫn khởi tạo mặc định."),
    ("UC-02 Resume", "Có/không có checkpoint", "Đọc scene hợp lệ → tải scene → đặt vị trí", "Checkpoint sai được đưa về Scene_1."),
    ("UC-03 Đánh miniboss", "Đã vào khu vực guardian", "Tấn công → chuyển phase → HP về 0 → rơi chìa khóa", "Người chơi chết thì Game Over."),
    ("UC-04 Qua màn", "Đã nhặt chìa khóa", "Chạm cổng → lưu checkpoint scene sau → tải scene", "Nếu chưa có chìa khóa, cổng không hoạt động."),
    ("UC-05 Awakening", "Power = 4", "Nhấn kỹ năng → tăng damage/defense 10 giây → hoàn nguyên", "Không đủ Power thì bỏ qua."),
    ("UC-06 Boss cuối", "Đến Scene_Final_1", "Giảm HP qua ba ngưỡng → né kỹ năng → hạ boss → Victory", "Nếu hết HP thì Game Over."),
], widths=[3, 4, 6, 4])

add_heading(doc, "2.17. Phân tích sản phẩm game 3D", 2)
add_placeholder(doc, "Thành viên phụ trách viết: mô tả bài toán game bắn súng 3D, cốt truyện/chế độ chơi, tác nhân, yêu cầu chức năng và phi chức năng.")
add_placeholder(doc, "Bổ sung sơ đồ luồng: Menu → chọn chế độ/màn → gameplay → thắng/thua → kết quả.")
add_placeholder(doc, "Bổ sung thiết kế điều khiển, súng, đạn, máu, AI địch, map 3D, UI và kiến trúc script.")

add_heading(doc, "2.18. Kết luận chương", 2)
add_para(doc, "Chương 2 đã chuyển yêu cầu của đề tài thành thiết kế cụ thể: cốt truyện, luồng màn, cơ chế chiến đấu, dữ liệu lưu, giao diện và AI boss. Thiết kế theo mô-đun giúp nhóm triển khai từng chức năng độc lập và kiểm thử theo use case. Chương 3 trình bày quá trình cài đặt và kết quả kiểm thử thực tế.")
doc.add_page_break()

# CHƯƠNG 3
add_heading(doc, "CHƯƠNG 3. TRIỂN KHAI, KIỂM THỬ VÀ ĐÁNH GIÁ", 1)
add_heading(doc, "3.1. Môi trường phát triển", 2)
add_table(doc, ["Thành phần", "Thông tin"], [
    ("Game Engine", "Unity 6000.6.0f1"),
    ("Ngôn ngữ", "C#"),
    ("Render Pipeline", "Universal Render Pipeline 17.6.0"),
    ("Gói chính", "2D Sprite, 2D Tilemap, Physics 2D, uGUI/TextMeshPro, Input System"),
    ("Nền tảng thử nghiệm", "Windows Editor; định hướng build Android"),
    ("Quản lý mã nguồn", "Git/Unity Version Control tùy cấu hình nhóm"),
    ("Quy mô hiện tại", "5 scene, 51 script C#, 58 prefab, 346 ảnh PNG, 23 audio, 86 animation/controller"),
], widths=[5, 11])

add_heading(doc, "3.2. Tổ chức thư mục", 2)
add_flow(doc, ["Assets/", "├─ Scenes/        Menu và các màn chơi", "├─ Scripts/       Camera, Player, Enemies, Menu, Portal, Scores", "├─ Prefabs/       Player, Enemies, Objects, UI, Sounds", "├─ Sprites/       Nhân vật, vật phẩm, background, UI", "├─ Resources/Boss Asset đá thiên thạch runtime", "└─ Sounds/        Nhạc nền và hiệu ứng"], "Hình 3.1. Cấu trúc tài nguyên chính")

add_heading(doc, "3.3. Triển khai nhân vật", 2)
add_para(doc, "PlayerController đọc trục Horizontal để điều khiển Rigidbody2D, đồng thời không ghi đè vận tốc khi joystick mobile đang thao tác. PlayerControllerUP quản lý quyền nhảy và nhảy kép. SpriteRenderer.flipX và object chứa attack point được xoay theo hướng nhân vật. Menu trong game thay đổi Time.timeScale giữa 0 và 1.")
add_para(doc, "PlayerCombat sử dụng Physics2D.OverlapCircleAll quanh attackPoint để tìm Enemy thuộc enemyLayers. Khi đánh trúng, hệ thống lấy AttackDamage từ Stats, gọi Enemy.TakeDamage và tăng Power. Bow phát hoạt ảnh, sau đó Animation Event gọi ShootArrow để tạo Arrow. Arrow sử dụng Rigidbody2D, bay với vận tốc 20, tồn tại tối đa ba giây và gây 140% Attack khi chạm Enemy hoặc Boss.")
add_para(doc, "DashSkill tạm thời tắt trọng lực, đặt vận tốc ngang bằng dashForce trong 0,2 giây và tạo afterimage mỗi 0,05 giây. Cooldown một giây giới hạn tần suất sử dụng. Awakening yêu cầu bốn Power, tăng gấp đôi damage, cộng 50 Defense và đổi màu nhân vật trong mười giây.")

add_heading(doc, "3.4. Triển khai chỉ số và vật phẩm", 2)
add_para(doc, "Stats quản lý HP, Power, Attack và Defense. UI máu sử dụng tập sprite theo từng khoảng 10 HP. Khi nhận sát thương, nhân vật lóe đỏ trong 0,12 giây và phát âm thanh nếu được gán. Khi HP về 0, cờ chết được đặt một lần, animation die chạy, HUD tắt và giao diện Game Over bật.")
add_para(doc, "Các component Coin, Gem, Star, Heart, Attack và Shield xử lý trigger với Player, cập nhật ScoreManager hoặc Stats rồi tự hủy. AttackDamage và Defense được lưu ngay khi tăng để duy trì qua scene. ScoreManager đọc và ghi tổng điểm, số coin, gem và star bằng PlayerPrefs.")

add_heading(doc, "3.5. Triển khai checkpoint và menu", 2)
add_para(doc, "GameProgress.SaveCheckpoint kiểm tra build index hợp lệ trước khi lưu. SavePlayerPosition ghi scene và tọa độ X/Y. PlayerController gọi hàm này khi ứng dụng pause, quit hoặc khi respawn. Khi scene bắt đầu, TryGetSavedPlayerPosition chỉ trả về vị trí nếu scene hiện tại trùng scene đã lưu, tránh đưa nhân vật đến tọa độ của màn khác.")
add_para(doc, "Trong scene Menu, script Menu tìm nút Play có sẵn, đổi thành Resume, sao chép để tạo New Game và sắp xếp Options, Exit thành bốn hàng. Resume tải checkpoint; New Game xóa checkpoint, vị trí, level mở, chỉ số và điểm rồi tải Scene_1. Trong gameplay, Resume không tải lại màn mà chỉ trả về trạng thái chơi hiện tại.")

add_heading(doc, "3.6. Triển khai chìa khóa và cổng", 2)
add_para(doc, "Portal.Awake đặt lại HasKey khi vào mỗi scene. Start tìm KeyPickup thuộc cùng scene, ẩn object chìa khóa và xác định guardian gần cổng nhất theo tên. Khi guardian hết máu, KeyPickup.PlaceAt cập nhật vị trí gốc để hiệu ứng bob hoạt động đúng tại nơi boss chết. Cổng chỉ gọi SceneManager.LoadScene sau khi HasKey bằng true; trước đó SpriteRenderer của cổng được tô xám.")

add_heading(doc, "3.7. Triển khai AI nhiều phase", 2)
add_para(doc, "MinibossController là component mở rộng Enemy thường thành guardian. Hàm ConfigureGuardian cache Enemy, BossWeapon, Patrol, SpriteRenderer, vị trí nhà và Transform người chơi. EvaluatePhase được gọi khi Enemy nhận damage. Tùy tag hoặc tên, controller chọn nhánh Knight, Bee hoặc Vampire.")
add_para(doc, "FinalWizardBossAI quản lý phase theo tỉ lệ máu bắt đầu. Ở phase 2, boss phản ứng với việc mất máu bằng BlinkAway, bắn quạt ba viên MageBall và đổi màu. Phase 3 chạy coroutine tuần tự nhằm tránh chồng nhiều kỹ năng lớn cùng lúc. MeteorRock di chuyển trong FixedUpdate bằng Rigidbody2D.MovePosition, dùng collision detection Continuous và xử lý trigger với Player hoặc nền.")

add_heading(doc, "3.8. Xử lý lỗi hiển thị mưa đá", 2)
add_para(doc, "Trong phiên bản thử nghiệm, Console báo đã tạo bảy object Wizard Meteor Rock nhưng người chơi không nhìn thấy. Kiểm tra runtime cho thấy sprite đã gán, tọa độ nằm trong viewport nhưng SpriteRenderer.isVisible bằng false. Nguyên nhân là GameObject tạo bằng mã mặc định ở layer Default, trong khi Culling Mask của Main Camera không render layer này. Giải pháp là đặt rock.layer, warning.layer và afterImage.layer bằng layer của boss. Sau sửa đổi, kiểm thử Play Mode ghi nhận 7/7 viên đá có isVisible bằng true và tự hủy đúng sau va chạm hoặc hết lifetime.")

add_heading(doc, "3.9. Triển khai giao diện, hội thoại và âm thanh", 2)
add_para(doc, "DialogueManager dùng Queue để lần lượt hiển thị sentence, coroutine tạo hiệu ứng gõ chữ và trigger để bật/tắt panel khi người chơi vào hoặc rời vùng NPC. ObjectivePanel được Portal điều khiển theo khoảng cách. UI mobile sử dụng các component IPointerDownHandler để chuyển thao tác chạm thành lệnh nhảy, kiếm hoặc cung.")
add_para(doc, "Âm thanh được tổ chức thành prefab: nhạc nền, tiếng cung, phép, va chạm, nhặt vật phẩm và âm thanh chết. VolumeController lưu AudioVolume. Hiệu ứng ImpactFX tạo hit-stop và camera shake để tăng cảm giác lực khi đòn đánh trúng.")

add_heading(doc, "3.10. Các hạng mục hoàn thiện nổi bật", 2)
add_table(doc, ["Hạng mục", "Yêu cầu đặt ra", "Kết quả triển khai"], [
    ("Luồng qua màn", "Tạo điều kiện hoàn thành rõ ràng", "Bắt buộc hạ guardian, nhặt chìa khóa mới mở cổng."),
    ("Miniboss", "Mỗi màn có thử thách riêng", "Ba guardian có phase riêng và độ khó tăng dần."),
    ("Boss cuối", "Tạo trận chiến cao trào", "Ba phase, phản ứng khi trúng đòn, bắn quạt, blink và mưa đá."),
    ("Tiến trình", "Cho phép chơi qua nhiều phiên", "Resume, New Game, checkpoint scene và vị trí."),
    ("Sát thương", "Defense hữu ích nhưng không gây bất tử", "Công thức damage tối thiểu 20% và phản hồi lóe đỏ."),
    ("Giao diện", "Dễ quan sát trên máy tính và mobile", "Bốn hàng menu; objective gần NPC; khung nền dễ đọc."),
    ("Đồ họa", "Tạo bản sắc riêng cho từng khu vực", "Hoàn thiện nhiều background theo đúng kích thước và pixel style."),
    ("Cốt truyện", "Có mở đầu, phát triển và kết thúc", "Ba chìa khóa phong ấn, guardian bị nguyền và trận chiến Malakar."),
    ("Kiểm thử", "Bảo đảm luồng chơi ổn định", "Kiểm tra biên dịch, Play Mode, va chạm, phase và hồi quy."),
], widths=[3, 5, 8])
add_para(doc, "Các hạng mục trên thể hiện quá trình nhóm chuyển yêu cầu gameplay thành chức năng cụ thể, đồng thời kết nối cốt truyện, giao diện, AI và tiến trình thành một sản phẩm có thể chơi xuyên suốt từ đầu đến cuối.")

add_heading(doc, "3.11. Kế hoạch và kết quả kiểm thử", 2)
test_rows = [
    ("TC-01", "New Game", "Chọn New Game", "Scene_1 mở; dữ liệu cũ được đặt lại", "Đạt"),
    ("TC-02", "Resume mặc định", "Chưa có save, chọn Resume", "Tải Scene_1", "Đạt"),
    ("TC-03", "Resume checkpoint", "Lưu ở Scene_2 rồi thoát", "Tải Scene_2 và vị trí đã lưu", "Đạt"),
    ("TC-04", "Pause Resume", "Mở pause trong gameplay rồi Resume", "Đóng menu, không tải scene khác", "Đạt"),
    ("TC-05", "Di chuyển", "Nhấn trái/phải", "Nhân vật di chuyển và quay đúng hướng", "Đạt"),
    ("TC-06", "Nhảy kép", "Nhấn nhảy hai lần", "Thực hiện tối đa hai lần trước khi chạm đất", "Đạt"),
    ("TC-07", "Kiếm", "Chém trong phạm vi", "Enemy mất HP, tăng Power, có hit impact", "Đạt"),
    ("TC-08", "Cung", "Bắn trúng enemy", "Arrow gây damage và biến mất", "Đạt"),
    ("TC-09", "Defense", "Nhận hit khi Defense cao", "Vẫn mất tối thiểu 20% damage", "Đạt"),
    ("TC-10", "Awakening", "Đủ 4 Power và kích hoạt", "Damage/Defense tăng 10 giây rồi trở lại", "Đạt"),
    ("TC-11", "Cổng khóa", "Chạm cổng khi chưa có key", "Không chuyển scene; cổng màu xám", "Đạt"),
    ("TC-12", "Rơi chìa khóa", "Hạ guardian", "Key xuất hiện đúng vị trí chết", "Đạt"),
    ("TC-13", "Nhặt chìa khóa", "Player chạm key", "HasKey=true; key biến mất; cổng sáng", "Đạt"),
    ("TC-14", "Knight phase 2", "Giảm HP dưới 45%", "Sinh bốn clone; clone damage giảm", "Đạt"),
    ("TC-15", "Bee lao", "Giảm HP dưới 50%", "Cảnh báo rồi lao đúng X/Y player", "Đạt"),
    ("TC-16", "Vampire phase 2", "HP dưới 60%", "Phóng lớn và né có cooldown", "Đạt"),
    ("TC-17", "Vampire phase 3", "HP dưới 25%", "Thu nhỏ, nhanh và dịch chuyển sau lưng có delay", "Đạt"),
    ("TC-18", "Wizard phase 2", "HP dưới 60%", "Đổi hình thái, blink và bắn ba phép", "Đạt"),
    ("TC-19", "Wizard phase 3", "HP dưới 30%", "Mưa đá, bắn quạt, blink tuần tự", "Đạt"),
    ("TC-20", "Đá hiển thị", "Boss gọi MeteorShower", "7 đá ở layer camera, sprite nhìn thấy", "Đạt"),
    ("TC-21", "Đá gây damage", "Đá chạm Player", "Mất 35 damage trước Defense và đá biến mất", "Đạt"),
    ("TC-22", "Đá chạm nền", "Đá rơi đến sàn", "Đá biến mất; không tích lũy object", "Đạt"),
    ("TC-23", "NPC", "Vào/rời vùng NPC", "Panel bật khi gần và tắt khi rời", "Đạt"),
    ("TC-24", "Game Over", "HP về 0", "Chạy death, tắt HUD và bật Game Over", "Đạt"),
    ("TC-25", "Biên dịch", "Build solution/Unity compile", "Không có lỗi C#", "Đạt"),
]
add_table(doc, ["Mã", "Chức năng", "Thao tác", "Kết quả mong đợi", "KQ"], test_rows, widths=[1.6, 3, 4, 7, 1.4], font_size=9.5)

add_heading(doc, "3.12. Đánh giá kết quả", 2)
add_heading(doc, "3.12.1. Kết quả đạt được", 3)
add_bullets(doc, [
    "Game có thể đi theo luồng Menu → ba màn guardian → boss cuối → chiến thắng.",
    "Cơ chế chìa khóa gắn thử thách chính với điều kiện qua màn.",
    "Ba miniboss và boss cuối có bản sắc chiến đấu khác nhau, không chỉ tăng HP.",
    "Độ khó tăng theo thứ tự 1.500 → 1.800 → 3.600 → 5.000 HP và tăng về độ phức tạp hành vi.",
    "Resume/New Game và checkpoint hoạt động phù hợp cho trải nghiệm chơi nhiều phiên.",
    "Điều khiển hỗ trợ bàn phím, gamepad và giao diện cảm ứng.",
    "Background được điều chỉnh để khác bản nền nhưng vẫn giữ phong cách pixel art.",
    "Các lỗi quan trọng về va chạm tường, mất damage và culling layer đã được phân tích và sửa.",
])

add_heading(doc, "3.12.2. Hạn chế", 3)
add_bullets(doc, [
    "Một số hội thoại và cutscene cốt truyện mới đang ở mức nội dung đề xuất, cần tích hợp đầy đủ vào scene trước bản phát hành chính thức.",
    "PlayerPrefs chưa mã hóa, có thể bị chỉnh sửa thủ công và khó lưu trạng thái phức tạp của quái vật.",
    "AI chủ yếu dựa trên luật và vị trí, chưa có tìm đường nâng cao cho mọi enemy.",
    "Chưa có hệ thống tùy chỉnh phím, lựa chọn độ khó và trợ năng đầy đủ.",
    "Cần kiểm thử thêm trên nhiều cấu hình điện thoại Android và nhiều tỉ lệ màn hình.",
    "Một số thông báo Console từ audio prefab chưa được gán cần tiếp tục dọn dẹp.",
])

add_heading(doc, "3.12.3. Hướng phát triển", 3)
add_bullets(doc, [
    "Tích hợp phần mở đầu, hội thoại giữa màn và ending đúng cốt truyện Ba Chìa Khóa Phong Ấn.",
    "Thay PlayerPrefs bằng JSON có phiên bản dữ liệu hoặc hệ thống save slot.",
    "Bổ sung bản đồ nhỏ, nhật ký nhiệm vụ, codex quái vật và lựa chọn độ khó.",
    "Mở rộng boss với animation telegraph riêng, particle impact và âm thanh theo phase.",
    "Tối ưu object pooling cho projectile, đá và damage popup.",
    "Thêm thống kê cuối game: thời gian, số quái, số lần chết và vật phẩm thu thập.",
    "Build APK, đo FPS, memory và thời gian tải trên thiết bị Android thật.",
])

add_heading(doc, "3.13. Triển khai và kiểm thử game bắn súng 3D", 2)
add_placeholder(doc, "Thành viên game 3D bổ sung môi trường phát triển, cấu trúc scene/prefab, player controller, camera, súng, projectile/raycast và AI enemy.")
add_placeholder(doc, "Chèn ảnh Menu, gameplay, map, kẻ địch, màn thắng/thua và các đoạn mã tiêu biểu của game 3D.")
add_placeholder(doc, "Lập bảng kiểm thử tối thiểu: di chuyển 3D, camera, bắn, nạp đạn, damage, AI, va chạm, thắng/thua, âm thanh và build.")

add_heading(doc, "3.14. Kết luận chương", 2)
add_para(doc, "Chương 3 đã trình bày cách các thiết kế được chuyển thành component Unity và script C#, từ điều khiển, combat, save đến AI boss nhiều phase. Ma trận kiểm thử cho thấy các chức năng chính của game 2D đã hoạt động. Các hạn chế còn lại chủ yếu nằm ở mức độ hoàn thiện nội dung cốt truyện, kiểm thử thiết bị và mở rộng hệ thống lưu.")
doc.add_page_break()

add_heading(doc, "KẾT LUẬN", 1)
add_para(doc, "Đồ án đã giúp nhóm tiếp cận quy trình phát triển trò chơi từ lý thuyết đến sản phẩm. Với Dungeon Slayer, nhóm đã phân tích yêu cầu, xác định các chức năng cần hoàn thiện và xây dựng một luồng gameplay có mục tiêu rõ ràng. Hệ thống ba chìa khóa kết nối từng màn với cốt truyện; AI nhiều phase tạo sự tăng tiến về thử thách; checkpoint và menu bảo đảm người chơi có thể bắt đầu mới hoặc tiếp tục; giao diện đa nền tảng hỗ trợ quá trình trình diễn và kiểm thử.")
add_para(doc, "Kết quả quan trọng không chỉ nằm ở số lượng chức năng mà còn ở việc nhóm xử lý các vấn đề thực tế của phát triển phần mềm: xung đột giữa UI và gameplay, cân bằng damage, va chạm, culling layer, quản lý trạng thái giữa scene và kiểm thử hồi quy. Cấu trúc script theo mô-đun giúp dự án tiếp tục được mở rộng mà không phải sửa toàn bộ hệ thống.")
add_para(doc, "Bên cạnh sản phẩm 2D, khung báo cáo đã dành vị trí cho game bắn súng 3D của nhóm. Sau khi thành viên phụ trách bổ sung nội dung, hai sản phẩm sẽ minh họa rõ sự khác nhau giữa phát triển game hai chiều và ba chiều, đồng thời thể hiện khả năng phối hợp và phân chia nhiệm vụ của nhóm.")

add_heading(doc, "TÀI LIỆU THAM KHẢO", 1)
references = [
    "[1] Unity Technologies, Unity Manual và Scripting API, phiên bản Unity 6, https://docs.unity3d.com/.",
    "[2] Unity Technologies, 2D Game Development và Tilemap documentation.",
    "[3] Robert Nystrom, Game Programming Patterns, Genever Benning, 2014.",
    "[4] Jesse Schell, The Art of Game Design: A Book of Lenses, CRC Press.",
    "[5] Ian Millington, Artificial Intelligence for Games, CRC Press.",
    "[6] Microsoft, C# Documentation, https://learn.microsoft.com/dotnet/csharp/.",
    "[7] Dungeon Slayer project resources and source acknowledgements, xem tệp README đi kèm dự án.",
    "[8] Tài liệu nội bộ, mã nguồn và kết quả kiểm thử của nhóm, 2026.",
]
for ref in references:
    p = doc.add_paragraph(ref)
    p.paragraph_format.left_indent = Cm(0.8)
    p.paragraph_format.first_line_indent = Cm(-0.8)

doc.add_page_break()
add_heading(doc, "PHỤ LỤC A. KỊCH BẢN HỘI THOẠI ĐỀ XUẤT", 1)
dialogue_rows = [
    ("Mở đầu", "Lời nguyền đã thức tỉnh. Hãy tìm ba mảnh chìa khóa, mở cánh cổng vực sâu và kết thúc sai lầm mà tổ tiên chúng ta chưa thể hoàn thành."),
    ("NPC màn 1", "Những sinh vật này từng bảo vệ chúng tôi. Kẻ canh giữ thứ nhất đang giữ một mảnh chìa khóa; lời nguyền có thể tạo ra bản sao của hắn."),
    ("Sau màn 1", "Sức mạnh điều khiển người canh giữ đã biến mất. Bạn nhận được Chìa khóa của Lòng Dũng Cảm."),
    ("NPC màn 2", "Kẻ canh giữ khu rừng không còn kiểm soát được bản thân. Đừng đứng yên khi nó bay lên cao; nó sẽ lao thẳng xuống vị trí của bạn."),
    ("Sau màn 2", "Sinh vật bảo vệ khu rừng đã được giải thoát. Bạn nhận được Chìa khóa của Sự Sống. Malakar đã cảm nhận được bạn."),
    ("NPC màn 3", "Người canh giữ cuối từng là anh hùng. Lời nguyền cho phép ông ấy thay đổi hình dạng, dịch chuyển và đọc hướng tấn công."),
    ("Guardian màn 3", "Cuối cùng ta cũng được giải thoát. Malakar không còn là con người. Đừng để hắn chạm vào Trái Tim Ánh Sáng."),
    ("Malakar", "Cuối cùng ngươi cũng mang cả ba chìa khóa đến cho ta. Những người canh giữ không chỉ ngăn ngươi vào đây; chúng đang ngăn ta thoát ra ngoài."),
    ("Aren", "Ta không đến để giải thoát ngươi. Ta đến để kết thúc lời nguyền."),
    ("Malakar phase 3", "Nếu ta không thể sở hữu Trái Tim Ánh Sáng, không ai có thể!"),
    ("Kết thúc", "Ta chỉ muốn chiến thắng cái chết, nhưng chính nỗi sợ đã biến ta thành quái vật. Hãy sửa chữa sai lầm của ta."),
]
add_table(doc, ["Thời điểm", "Nội dung"], dialogue_rows, widths=[4, 12])

add_heading(doc, "PHỤ LỤC B. CHECKLIST TRƯỚC KHI BÁO CÁO", 1)
add_bullets(doc, [
    "Điền đầy đủ tên trường, khoa, môn học, giảng viên, lớp và thành viên.",
    "Điều chỉnh bảng phân công đúng đóng góp thực tế.",
    "Thành viên game 3D hoàn thiện toàn bộ ô màu vàng và chèn ảnh sản phẩm.",
    "Tích hợp hoặc xác nhận nội dung hội thoại/cutscene thực tế rồi sửa các câu mô tả nếu cần.",
    "Chạy một lượt New Game từ Scene_1 đến Victory và quay video minh họa.",
    "Kiểm tra invincibleForBossTesting đã tắt trên Player prefab và scene.",
    "Dọn lỗi đỏ và các cảnh báo audio quan trọng trong Console.",
    "Build APK/Windows và chạy thử trên thiết bị dùng để trình bày.",
    "Trong Word, nhấn Ctrl+A rồi F9 để cập nhật mục lục và số trang.",
    "Xuất thêm bản PDF sau khi duyệt định dạng cuối cùng.",
])

add_heading(doc, "PHỤ LỤC C. THÔNG TIN MÃ NGUỒN", 1)
add_table(doc, ["Nội dung", "Đường dẫn"], [
    ("Scene", "Assets/Scenes"),
    ("Script Player", "Assets/Scripts/Player"),
    ("Script Enemy/Boss", "Assets/Scripts/Enemies"),
    ("Menu và tiến trình", "Assets/Scripts/Menu"),
    ("Chìa khóa và cổng", "Assets/Scripts/Portal"),
    ("Prefab", "Assets/Prefabs"),
    ("Asset mưa đá", "Assets/Resources/Boss/MeteorRock-v2.png"),
], widths=[6, 10])

# Làm sạch đoạn trống đầu ở các cell và thiết lập font mặc định cho toàn bộ run.
for paragraph in doc.paragraphs:
    for run in paragraph.runs:
        if run.font.name is None:
            run.font.name = "Times New Roman"
            run._element.get_or_add_rPr().rFonts.set(qn("w:eastAsia"), "Times New Roman")

OUT.parent.mkdir(parents=True, exist_ok=True)
doc.save(OUT)
print(OUT)
