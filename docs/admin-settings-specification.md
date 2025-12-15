# Admin Settings - Specification Document

Tài liệu mô tả chi tiết các cài đặt cần thiết cho trang quản trị Moderno E-commerce.

## Tổng quan

Mục **Cài đặt** (Settings) cho phép admin cấu hình toàn bộ hệ thống e-commerce, bao gồm thanh toán, vận chuyển, email, SEO, và các tích hợp bên ngoài.

---

## 1. Cài đặt chung (General Settings)

### Thông tin cửa hàng
- **Tên website/cửa hàng**: Moderno Commerce
- **Slogan/Tagline**: Phong cách thời trang hiện đại
- **Logo**: Upload logo cho header và favicon
- **Favicon**: Icon hiển thị trên browser tab

### Thông tin liên hệ
- **Email chính**: contact@moderno.vn
- **Hotline**: 0123 456 789
- **Địa chỉ**: 123 Đường ABC, Quận XYZ, Hà Nội
- **Giờ làm việc**: 8:00 AM - 10:00 PM

### Cấu hình hệ thống
- **Timezone**: Asia/Ho_Chi_Minh (UTC+7)
- **Ngôn ngữ mặc định**: vi-VN (Tiếng Việt)
- **Tiền tệ**: VND (Vietnam Dong)
- **Format tiền tệ**: 1.000.000₫
- **Maintenance Mode**: Bật/Tắt chế độ bảo trì

---

## 2. Cài đặt thanh toán (Payment Settings)

### Phương thức thanh toán

#### COD - Cash on Delivery
- **Kích hoạt**: Yes/No
- **Mô tả**: "Thanh toán khi nhận hàng"
- **Phí COD**: 0 VND hoặc tính theo %

#### Chuyển khoản ngân hàng
- **Kích hoạt**: Yes/No
- **Thông tin TK 1**:
  - Ngân hàng: Vietcombank
  - Số tài khoản: 0123456789
  - Chủ tài khoản: MODERNO COMMERCE
- **Thông tin TK 2**:
  - Ngân hàng: Techcombank
  - Số tài khoản: 9876543210
  - Chủ tài khoản: MODERNO COMMERCE

#### VNPay
- **Kích hoạt**: Yes/No
- **TMN Code**: (API key)
- **Hash Secret**: (Secret key)
- **Mode**: Sandbox / Production
- **Return URL**: https://moderno.vn/payment/vnpay-return

#### MoMo
- **Kích hoạt**: Yes/No
- **Partner Code**: (API key)
- **Access Key**: 
- **Secret Key**:
- **Mode**: Sandbox / Production
- **Return URL**: https://moderno.vn/payment/momo-return

#### ZaloPay
- **Kích hoạt**: Yes/No
- **App ID**:
- **Key1**:
- **Key2**:
- **Endpoint**: Sandbox / Production

### Cấu hình chung
- **Auto-confirm payment**: Tự động xác nhận thanh toán sau X phút
- **Payment timeout**: Hủy đơn nếu không thanh toán sau X giờ

---

## 3. Cài đặt vận chuyển (Shipping Settings)

### Phí vận chuyển
- **Phí ship mặc định**: 30.000 VND
- **Free shipping từ**: 500.000 VND
- **Phí ship theo khu vực**:
  - Nội thành Hà Nội: 30.000 VND
  - Ngoại thành Hà Nội: 50.000 VND
  - Tỉnh khác: 80.000 VND

### Đơn vị vận chuyển

#### GHN - Giao hàng nhanh
- **Kích hoạt**: Yes/No
- **Token API**: 
- **Shop ID**:
- **Webhook URL**: https://moderno.vn/webhook/ghn

#### GHTK - Giao hàng tiết kiệm
- **Kích hoạt**: Yes/No
- **Token API**:

#### Viettel Post
- **Kích hoạt**: Yes/No
- **Username**:
- **Password**:

### Thời gian giao hàng
- **Nội thành**: 1-2 ngày
- **Ngoại thành**: 2-3 ngày
- **Tỉnh khác**: 3-5 ngày

---

## 4. Cài đặt Email (Email Settings)

### SMTP Configuration
- **Host**: smtp.gmail.com
- **Port**: 587
- **Username**: noreply@moderno.vn
- **Password**: ************
- **Encryption**: TLS
- **From Email**: noreply@moderno.vn
- **From Name**: Moderno Commerce

### Email Templates

#### Đơn hàng mới
- **Subject**: "Xác nhận đơn hàng #{{OrderId}}"
- **Gửi tới**: Customer email
- **Template**: order-confirmation.html

#### Thanh toán thành công
- **Subject**: "Thanh toán thành công - Đơn hàng #{{OrderId}}"
- **Template**: payment-success.html

#### Đơn hàng đang giao
- **Subject**: "Đơn hàng #{{OrderId}} đang được giao đến bạn"
- **Template**: order-shipping.html

#### Đơn hàng hoàn thành
- **Subject**: "Đơn hàng #{{OrderId}} đã giao thành công"
- **Template**: order-completed.html

#### Đơn hàng bị hủy
- **Subject**: "Đơn hàng #{{OrderId}} đã bị hủy"
- **Template**: order-cancelled.html

### Test Email
- **Test recipient**: admin@moderno.vn
- **Send test email** button

---

## 5. Cài đặt SEO & Marketing

### SEO
- **Meta Title mặc định**: "Moderno - Thời trang hiện đại"
- **Meta Description**: "Mua sắm thời trang online với Moderno - Sản phẩm chất lượng cao, giá tốt, giao hàng nhanh"
- **Meta Keywords**: "thời trang, quần áo, moderno"
- **Google Analytics ID**: UA-XXXXXXXXX-X hoặc G-XXXXXXXXXX
- **Google Tag Manager ID**: GTM-XXXXXXX

### Social Media Tracking
- **Facebook Pixel ID**: 
- **TikTok Pixel ID**:
- **Google Ads Conversion ID**:

### Schema Markup
- **Organization Schema**: Kích hoạt
- **Product Schema**: Kích hoạt
- **Breadcrumb Schema**: Kích hoạt

---

## 6. Cài đặt sản phẩm (Product Settings)

### Hiển thị
- **Sản phẩm/trang**: 12
- **Sản phẩm related**: 4
- **Sản phẩm new arrivals**: 8
- **Sort mặc định**: Mới nhất

### Reviews & Ratings
- **Cho phép reviews**: Yes/No
- **Auto-approve reviews**: Yes/No
- **Chỉ customer đã mua mới review được**: Yes/No
- **Hiển thị rating trung bình**: Yes/No

### Stock Management
- **Track inventory**: Yes/No
- **Low stock threshold**: 10 items
- **Out of stock visibility**: Ẩn / Hiển thị
- **Allow backorder**: Yes/No

### Images
- **Default product image**: /images/no-product.jpg
- **Max upload size**: 5 MB
- **Allowed formats**: jpg, png, webp
- **Auto resize**: 800x800px

---

## 7. Cài đặt đơn hàng (Order Settings)

### Order ID
- **Prefix**: MOD-
- **Format**: MOD-YYYYMMDD-XXXX
- **Example**: MOD-20251215-0001

### Auto Processing
- **Auto-cancel nếu chưa thanh toán**: 24 giờ
- **Auto-complete sau khi giao**: 7 ngày
- **Cho phép customer hủy đơn**: Trong vòng 30 phút sau đặt hàng

### Order Workflow
1. Pending → Confirmed → Processing → Shipping → Completed
2. Có thể Cancelled ở bất kỳ bước nào trước Shipping

---

## 8. Cài đặt bảo mật (Security Settings)

### Authentication
- **Two-Factor Authentication (2FA)**: Kích hoạt cho admin
- **Session Timeout**: 12 giờ
- **Remember Me Duration**: 30 ngày

### Access Control
- **IP Whitelist**: Enable/Disable
- **Allowed IPs**: 
  - 192.168.1.1
  - 103.xxx.xxx.xxx
- **Max login attempts**: 5 lần
- **Lockout duration**: 30 phút

### Password Policy
- **Minimum length**: 8 ký tự
- **Require uppercase**: Yes
- **Require lowercase**: Yes
- **Require numbers**: Yes
- **Require special characters**: Yes
- **Password expiry**: 90 ngày (cho admin)

---

## 9. Social Media Links

### Platform Links
- **Facebook**: https://facebook.com/moderno
- **Instagram**: https://instagram.com/moderno
- **TikTok**: https://tiktok.com/@moderno
- **YouTube**: https://youtube.com/@moderno
- **Zalo**: https://zalo.me/moderno

### Social Sharing
- **Enable social share buttons**: Yes
- **Platforms**: Facebook, Twitter, Pinterest, WhatsApp

---

## 10. Thông báo (Notification Settings)

### Admin Notifications
- **Email khi có đơn mới**: Yes
- **Email khi thanh toán thành công**: Yes
- **Email khi tồn kho thấp**: Yes
- **Admin email**: admin@moderno.vn

### Customer Notifications
- **SMS khi đơn hàng được xác nhận**: Yes/No
- **SMS khi đơn hàng đang giao**: Yes/No
- **Email marketing**: Opt-in by default

### Push Notifications (nếu có app)
- **Firebase Server Key**:
- **OneSignal App ID**:

### Team Alerts
- **Slack Webhook URL**: https://hooks.slack.com/services/xxx
- **Discord Webhook URL**: https://discord.com/api/webhooks/xxx
- **Alert cho**: Đơn hàng mới, Lỗi hệ thống, Low stock

---

## 11. Backup & Logs

### Automatic Backup
- **Kích hoạt**: Yes/No
- **Frequency**: Daily / Weekly
- **Time**: 2:00 AM
- **Storage**: Local / Cloud (AWS S3, Google Cloud Storage)
- **Retention**: 30 ngày

### Activity Logs
- **Log admin actions**: Yes
- **Log user actions**: Yes
- **Retention period**: 90 ngày
- **Export logs**: CSV, JSON

### Error Logging
- **Level**: Debug / Info / Warning / Error
- **Log to file**: Yes
- **Log to external service**: Sentry, LogRocket

---

## 12. API & Integrations

### Internal API
- **API Enabled**: Yes/No
- **Base URL**: https://api.moderno.vn/v1
- **API Key**: Generate new key
- **Rate Limit**: 1000 requests/hour

### Third-party Integrations

#### RabbitMQ (Message Queue)
- **Host**: localhost
- **Port**: 5672
- **Username**: guest
- **Password**: guest
- **Virtual Host**: /

#### Redis (Caching)
- **Host**: localhost
- **Port**: 6379
- **Password**: (optional)
- **Database**: 0

#### CDN
- **Provider**: Cloudflare / AWS CloudFront
- **CDN URL**: https://cdn.moderno.vn
- **Purge cache**: Button

---

## Implementation Notes

### Priority Levels
1. **High Priority**: General, Payment, Shipping, Email
2. **Medium Priority**: SEO, Product, Order, Security
3. **Low Priority**: Backup, API, Advanced integrations

### UI Recommendations
- Sử dụng **Tabs** để organize các settings categories
- Mỗi tab có form riêng với Save button
- Real-time validation cho các trường bắt buộc
- Test buttons cho Email, Payment gateways
- Confirmation dialog khi thay đổi critical settings

### Database Schema Suggestion
```sql
CREATE TABLE Settings (
    Id INT PRIMARY KEY,
    Category VARCHAR(50), -- 'General', 'Payment', 'Shipping'...
    Key VARCHAR(100),
    Value TEXT,
    Type VARCHAR(20), -- 'String', 'Boolean', 'Integer', 'JSON'
    IsEncrypted BIT,
    UpdatedAt DATETIME,
    UpdatedBy VARCHAR(100)
)
```

### Security Considerations
- Encrypt sensitive values (API keys, passwords)
- Audit log cho mọi thay đổi settings
- Require admin confirmation cho critical changes
- Backup settings trước khi update
