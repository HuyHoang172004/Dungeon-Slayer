# Dungeon Slayer: Trái Tim Ánh Sáng

Dungeon Slayer là game hành động nhập vai 2D phong cách pixel art, được phát triển bằng Unity dành cho Windows và Android. Người chơi điều khiển chiến binh Aren khám phá ba vùng đất bị nguyền rủa, đánh bại các Kẻ Canh Giữ, thu hồi ba chìa khóa phong ấn và đối đầu với pháp sư Malakar trong trận chiến cuối cùng.

> Trạng thái dự án: phiên bản có thể chơi hoàn chỉnh từ mở đầu, ba màn chính, boss cuối đến hoạt cảnh kết thúc.

## Cốt truyện

Vương quốc Eldoria từng được bảo vệ bởi **Trái Tim Ánh Sáng**, nguồn sức mạnh duy trì sự sống và ngăn bóng tối xâm chiếm. Vì khao khát sự bất tử, pháp sư hoàng gia Malakar đã thực hiện một nghi lễ cấm. Nghi lễ thất bại, biến ông thành chúa tể bóng tối và gieo lời nguyền lên toàn vương quốc.

Để phong ấn Malakar, sức mạnh của Trái Tim Ánh Sáng được chia thành ba chìa khóa. Những người bảo vệ chìa khóa — Knight, Bee và Vampire — cũng bị lời nguyền kiểm soát. Aren phải giải thoát họ, thu hồi:

- Chìa khóa Lòng Dũng Cảm.
- Chìa khóa Sự Sống.
- Chìa khóa Ý Chí.

Khi ba chìa khóa hợp nhất, Aren có thể phá bỏ phong ấn bóng tối, giải phóng Trái Tim Ánh Sáng và khôi phục Eldoria.

## Luồng chơi

1. Chọn **NEW GAME** và xem phần dẫn truyện mở đầu.
2. Khám phá bản đồ, nói chuyện với NPC và thu thập vật phẩm nâng cấp.
3. Tìm và đánh bại miniboss đang giữ chìa khóa của màn.
4. Nhặt chìa khóa vừa xuất hiện tại vị trí miniboss bị hạ.
5. Đi tới cổng cuối màn để mở khóa khu vực tiếp theo.
6. Hoàn thành ba màn chính rồi đối đầu với Malakar.
7. Sau chiến thắng, Aren tự tiến đến Malakar, kích hoạt nghi lễ ba chìa khóa và giải phóng Trái Tim Ánh Sáng.
8. Xem phần kết truyện, điểm số và số sao đạt được.

## Tính năng nổi bật

### Chiến đấu và di chuyển

- Đánh cận chiến bằng kiếm và bắn cung tầm xa.
- Nhảy hai lần và lướt nhanh để vượt địa hình hoặc né đòn.
- Hiệu ứng hit-stop, rung camera, lóe sáng khi nhận sát thương và popup sát thương.
- Thanh HP, năng lượng, sức tấn công và phòng thủ.
- Hệ thống **Awakening**: tích đủ bốn năng lượng để tạm thời tăng gấp đôi sát thương và cộng thêm phòng thủ.
- Vật phẩm gồm xu, ngọc, tim hồi máu, kiếm tăng công, khiên tăng thủ và sao.

### Hệ thống màn chơi và chìa khóa

- Ba bản đồ chính có bối cảnh pixel art riêng.
- Mỗi màn có một miniboss và một chìa khóa mang màu sắc riêng.
- Người chơi bắt buộc phải đánh bại Kẻ Canh Giữ và nhặt chìa khóa trước khi qua cổng.
- Mục tiêu nhiệm vụ thay đổi theo tiến trình và chỉ hiện ở khu vực cần thiết.
- Mỗi màn chính có một ngôi sao duy nhất; sao đã nhặt không xuất hiện lại khi Replay, Resume hoặc mở lại game.
- Tổng số sao tối đa là ba và chỉ được đặt lại khi chọn New Game.

### Miniboss và boss

| Khu vực | Kẻ địch | Cơ chế nổi bật |
| --- | --- | --- |
| Màn 1 | Knight Guardian | Khi máu thấp bước sang Phase 2, tăng sức mạnh và triệu hồi bốn phân thân có sát thương cân bằng hơn bản chính. |
| Màn 2 | Bee Guardian | Bay tuần tra; ở Phase 2 chuyển sang trạng thái cuồng nộ, tăng tốc và lao xuống đúng vị trí của người chơi rồi rút lui. |
| Màn 3 | Vampire Guardian | Có ba phase; thay đổi kích thước, tăng tốc, dịch chuyển né đòn và xuất hiện sau lưng Aren để tấn công. |
| Màn cuối | Malakar | Boss ba phase có khả năng phản ứng khi trúng đòn, dịch chuyển, bắn chùm phép và triệu hồi bảy thiên thạch với vùng cảnh báo. |

Thiên thạch của Malakar tạo cảnh báo trước khi rơi, gây rung màn hình, gây sát thương khi chạm người chơi và biến mất khi va chạm với người hoặc mặt đất.

### Cốt truyện và giao diện

- Mở đầu bằng bốn tranh truyện giới thiệu Eldoria, Malakar, ba chìa khóa và nhiệm vụ của Aren.
- NPC hướng dẫn và hội thoại theo từng màn.
- Hộp mục tiêu, thông báo nhận chìa khóa và hội thoại được bố trí riêng để không che HUD.
- Hoạt cảnh kết thúc diễn ra trực tiếp trong đấu trường trước khi chuyển sang phần kết truyện và màn hình Victory.
- Hỗ trợ giao diện điều khiển cảm ứng trên Android.

### Lưu tiến trình

- **RESUME trong cùng phiên chơi:** trở lại đúng scene và giữ nguyên vị trí người chơi, trạng thái kẻ địch, HP và phase của boss.
- **RESUME sau khi mở lại game:** tiếp tục từ dữ liệu màn/vị trí đã lưu; các đối tượng chiến đấu trong scene được khởi tạo lại.
- **REPLAY:** tải lại màn hiện tại, xóa vị trí chết nhưng giữ các chỉ số nâng cấp lâu dài.
- **NEW GAME:** xóa tiến trình, điểm số, nâng cấp và trạng thái sao để bắt đầu lại từ đầu.
- Dữ liệu tiến trình được lưu bằng `PlayerPrefs`.

## Điều khiển

### Bàn phím

| Phím | Hành động |
| --- | --- |
| `A` / `D` hoặc phím mũi tên | Di chuyển trái/phải |
| `Space` | Nhảy / nhảy lần hai |
| `F` | Tấn công bằng kiếm |
| `C` | Bắn cung |
| `Left Shift` | Lướt nhanh |
| `R` | Kích hoạt Awakening khi đủ năng lượng |
| `Esc` | Mở/đóng menu tạm dừng |

### Tay cầm

| Nút | Hành động |
| --- | --- |
| Analog trái / D-Pad | Di chuyển |
| `A` / Cross | Nhảy |
| `X` / Square | Tấn công bằng kiếm |
| `Y` / Triangle | Bắn cung |
| Right Bumper | Lướt nhanh |
| `B` / Circle | Kích hoạt Awakening |
| Start | Mở/đóng menu tạm dừng |

### Android

- Joystick bên trái dùng để di chuyển.
- Các nút bên phải dùng để nhảy, đánh kiếm và bắn cung.
- Nút cài đặt dùng để tạm dừng hoặc quay lại menu.

## Yêu cầu phát triển

- Unity Editor **6000.6.0f1**.
- Unity Hub.
- Trình soạn thảo C# như Visual Studio, Visual Studio Code hoặc JetBrains Rider.
- Android Build Support, SDK, NDK và OpenJDK nếu muốn xuất bản Android.

Các package quan trọng:

- Universal Render Pipeline `17.6.0`.
- Input System `1.20.0`.
- AI Navigation `2.0.14`.
- 2D Sprite và 2D Tilemap.
- TextMesh Pro thông qua Unity UI.

## Cài đặt và chạy dự án

```bash
git clone <repository-url>
```

1. Mở Unity Hub và chọn **Add project from disk**.
2. Chọn thư mục gốc `Dungeon-Slayer-Android`.
3. Mở dự án bằng Unity `6000.6.0f1` và chờ Unity import toàn bộ asset/package.
4. Mở scene `Assets/Scenes/Menu.unity`.
5. Nhấn **Play** để chạy game từ Main Menu.

Nếu Unity yêu cầu phiên bản khác, nên cài đúng `6000.6.0f1` để tránh thay đổi scene, prefab hoặc package ngoài ý muốn.

## Thứ tự scene trong Build Settings

| Build Index | Scene |
| ---: | --- |
| 0 | `Assets/Scenes/Menu.unity` |
| 1 | `Assets/Scenes/Scene_1.unity` |
| 2 | `Assets/Scenes/Scene_2.unity` |
| 3 | `Assets/Scenes/Scene_3.unity` |
| 4 | `Assets/Scenes/Scene_Final_1.unity` |

Các scene đã được khai báo sẵn trong `ProjectSettings/EditorBuildSettings.asset`.

## Build game

### Windows

1. Mở **File > Build Profiles**.
2. Chọn Windows và cài Windows Build Support nếu Unity yêu cầu.
3. Kiểm tra toàn bộ scene phía trên đã được bật.
4. Chọn **Build** hoặc **Build And Run**.

### Android

1. Cài Android Build Support từ Unity Hub.
2. Mở **File > Build Profiles** và chuyển sang Android.
3. Thiết lập package name, orientation và signing theo môi trường phát hành.
4. Chọn **Build** để tạo APK/AAB.

## Cấu trúc thư mục

```text
Assets/
├── Animations/          # Animation clip và Animator Controller
├── Font/                # Font pixel và font asset
├── Prefabs/             # Player, enemies, UI, vật phẩm và object dùng lại
├── Resources/           # Tranh mở đầu/kết thúc và asset nạp lúc chạy
├── Scenes/              # Menu, ba màn chính và màn boss cuối
├── Scripts/
│   ├── Camera/          # Camera, Victory UI và hoạt cảnh kết thúc
│   ├── Dialogues/       # Hội thoại NPC
│   ├── Enemies/         # Enemy, miniboss và AI boss cuối
│   ├── Menu/            # Main Menu, Resume, New Game và mở đầu cốt truyện
│   ├── Player/          # Di chuyển, chiến đấu, chỉ số và kỹ năng
│   ├── Portal/          # Chìa khóa, mục tiêu và chuyển màn
│   └── Scores/          # Điểm số và vật phẩm thu thập
├── Sounds/              # Nhạc nền và hiệu ứng âm thanh
└── Sprites/             # Nhân vật, môi trường, UI và hiệu ứng pixel art
```

## Chế độ kiểm thử bất tử

Để kiểm tra boss mà không bị mất máu:

1. Mở scene cần kiểm thử.
2. Chọn `Player` trong Hierarchy.
3. Trong component `Stats (Script)`, bật `Invincible For Boss Testing`.
4. Bỏ chọn tùy chọn này trước khi tạo bản build chính thức.

## Công nghệ sử dụng

- Unity 6 và C#.
- Universal Render Pipeline 2D.
- Rigidbody2D, Collider2D và hệ thống vật lý 2D.
- Animator Controller và sprite animation.
- TextMesh Pro và Unity UI.
- PlayerPrefs cho dữ liệu tiến trình.

---

Đây là dự án game phục vụ học tập và trình diễn sản phẩm môn học.
