# Moderno Commerce (.NET 8, MVC + Saga)

## Tổng quan

- `Ecommerce.Web`: giao diện khách + admin dashboard lấy cảm hứng từ template Moderno (Wix Studio).
- `Ecommerce.Workers`: MassTransit worker chạy saga tạo hóa đơn → thanh toán → thông báo.
- `Ecommerce.Infrastructure`: EF Core + Postgres cho catalog, orders, payments, notification logs.
- `Ecommerce.Contracts`: contracts message dùng chung giữa web và worker.

## Công nghệ chính

- ASP.NET Core MVC, Areas (Storefront + Admin).
- EF Core 8 + PostgreSQL (`ConnectionStrings:Postgres`).
- MassTransit 8.3 + RabbitMQ transport.
- Saga state machine + consumers (create invoice, process payment, send notification) lưu state bằng EF/Postgres.

## Chạy dự án

```bash
# 1. Tạo database & cập nhật connection string (appsettings.*)
psql -c "CREATE DATABASE ecommerce;"

# 2. Chạy web MVC
dotnet run --project src/Ecommerce.Web/Ecommerce.Web.csproj

# 3. Chạy worker saga
dotnet run --project src/Ecommerce.Workers/Ecommerce.Workers.csproj
```

## RabbitMQ & MassTransit

- Cấu hình tại `RabbitMq` section trong `appsettings.*`.
- Worker đăng ký:
  - Saga `OrderSaga` với state EF (`order_sagas` table).
  - Consumers: `CreateInvoiceConsumer`, `ProcessPaymentConsumer`, `SendNotificationConsumer`.
- Luồng:
  1. Web publish `CreateInvoice`.
  2. Consumer ghi Order + publish `InvoiceCreated`.
  3. Saga nhận `InvoiceCreated` → publish `ProcessPayment`.
  4. Payment consumer xác nhận → publish `PaymentProcessed`.
  5. Saga publish `SendNotification` → consumer ghi log & emit `NotificationSent` → saga finalize.

## Giao diện

- Storefront: trang hero, grid sản phẩm, form checkout (publish message khởi tạo saga).
- Admin (`/Admin/Dashboard`): cards KPI + bảng orders gần nhất đọc trực tiếp từ Postgres.

## Migrations

Chưa scaffold migration mặc định. Để tạo:

```bash
dotnet ef migrations add InitialCreate -p src/Ecommerce.Infrastructure -s src/Ecommerce.Web
dotnet ef database update -s src/Ecommerce.Web
```

Saga state dùng `OrderStateDbContext` -> tạo migration riêng:

```bash
dotnet ef migrations add OrderSagaInit -p src/Ecommerce.Workers -c OrderStateDbContext -s src/Ecommerce.Workers
```

## TODO tiếp theo

- Hoàn thiện UI assets (ảnh local, animation).
- Bổ sung unit tests cho saga và controllers.
- Thêm auth/role cho khu vực admin và health checks cho RabbitMQ/Postgres.

