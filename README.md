# Campus Safety Hub - Bài Thực Hành Mẫu Thiết Kế (Bản Nháp v1)

## 1. Giới thiệu
Chương trình mô phỏng trung tâm giám sát an toàn cho phòng máy (Campus Safety Hub).
Áp dụng 3 mẫu thiết kế:
- **Adapter**: Chuyển đổi cảm biến cũ `LegacySmokeDetector` sang chuẩn mới `ISensor`.
- **Observer**: Phân phối cảnh báo từ `SensorMonitor` tới các bên quan tâm (`Dashboard`, `Email`, `AuditLog`, `EmergencyBridge`).
- **Mediator**: Điều phối các thiết bị ứng phó khẩn cấp (`Alarm`, `DoorController`, `VentilationController`) mà không để chúng phụ thuộc trực tiếp vào nhau.

## 2. Tình trạng hiện tại (v1 - Còn lỗi)
- `SmokeSensorAdapter`: Phép tính chuyển đổi thang đo đang bị nhầm `/ 100.0` thay vì `/ 10.0`.
- `EmergencyBridgeObserver`: Đang kích hoạt chuông cứu hỏa cho cả mức WARNING (chỉ được kích hoạt khi CRITICAL >= 70%).
- `SensorMonitor`: Chưa kiểm tra trùng lặp observer, duyệt trực tiếp `observers` không an toàn khi detach, chưa bọc `try-catch`.
- Chưa có bộ test suite hoàn chỉnh.

## 3. Kế hoạch hoàn thiện (TODO)
- [ ] Sửa lại phép tính Adapter trong `SmokeSensorAdapter.cs`.
- [ ] Chỉ kích hoạt Mediator khi severity == "CRITICAL" trong `EmergencyBridgeObserver.cs`.
- [ ] Tối ưu `SensorMonitor.cs` với `ToArray()` và `try-catch`.
- [ ] Hoàn thiện báo cáo, sơ đồ UML và trả lời câu hỏi lý thuyết.
