# AdminUser Management Implementation - Walkthrough

## Overview

Đã implement đầy đủ chức năng quản lý tài khoản Admin với khả năng gán nhóm (Group) cho mỗi admin user.

## Changes Made

### 1. Database Layer

#### [AdminUser.cs](file:///e:/Data/Ecommerce/src/Ecommerce.Infrastructure/Entities/AdminUser.cs)

Thêm GroupId và navigation property:

```diff
 public class AdminUser : BaseEntity
 {
     public required string Username { get; set; }
     public required string PasswordHash { get; set; }
     public required string Email { get; set; }
     public string? FullName { get; set; }
     public bool IsActive { get; set; } = true;
+    
+    // Group assignment
+    public Guid? GroupId { get; set; }
+    public Group? Group { get; set; }
 }
```

#### [EcommerceDbContext.cs](file:///e:/Data/Ecommerce/src/Ecommerce.Infrastructure/Persistence/EcommerceDbContext.cs)

Thêm foreign key configuration:

```diff
 modelBuilder.Entity<AdminUser>(entity =>
 {
     entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
     entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
     entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
     entity.Property(x => x.FullName).HasMaxLength(200);
     
     entity.HasIndex(x => x.Username).IsUnique();
     entity.HasIndex(x => x.Email).IsUnique();
+    
+    // Foreign key to Group
+    entity.HasOne(x => x.Group)
+        .WithMany()
+        .HasForeignKey(x => x.GroupId)
+        .OnDelete(DeleteBehavior.SetNull);
 });
```

**Migration**: `AddGroupIdToAdminUser` đã được tạo

---

### 2. Application Layer

#### [AdminUserViewModels.cs](file:///e:/Data/Ecommerce/src/Ecommerce.Web/Areas/Admin/ViewModels/AdminUserViewModels.cs) - NEW

Tạo 2 ViewModels:
- `AdminUserFormViewModel`: Form create/edit với validation
- `ResetPasswordViewModel`: Form reset password

#### [AdminUsersController.cs](file:///e:/Data/Ecommerce/src/Ecommerce.Web/Areas/Admin/Controllers/AdminUsersController.cs) - NEW

Full CRUD controller với các actions:

| Action | Method | Chức năng |
|--------|--------|-----------|
| `Index` | GET | Danh sách với search & pagination |
| `Create` | GET/POST | Tạo admin user mới |
| `Edit` | GET/POST | Chỉnh sửa thông tin |
| `Delete` | POST | Xóa admin user |
| `ToggleActive` | POST | Bật/tắt tài khoản |
| `ResetPassword` | GET/POST | Đặt lại mật khẩu |

**Key Features:**
- ✅ Validation: Username/Email unique, password min 6 chars
- ✅ BCrypt password hashing
- ✅ Group dropdown với active groups
- ✅ Search: username, email, fullname
- ✅ Pagination: 10 items per page

---

### 3. Presentation Layer

#### Views Created

1. **[Index.cshtml](file:///e:/Data/Ecommerce/src/Ecommerce.Web/Areas/Admin/Views/AdminUsers/Index.cshtml)**
   - Table hiển thị: Username, Email, Full Name, Group, Status
   - Search box
   - Pagination
   - Actions: Reset Password, Toggle Active, Edit, Delete

2. **[Create.cshtml](file:///e:/Data/Ecommerce/src/Ecommerce.Web/Areas/Admin/Views/AdminUsers/Create.cshtml)**
   - Form tạo mới với password fields
   - Group dropdown
   - IsActive checkbox
   - Validation messages

3. **[Edit.cshtml](file:///e:/Data/Ecommerce/src/Ecommerce.Web/Areas/Admin/Views/AdminUsers/Edit.cshtml)**
   - Username readonly (không cho sửa)
   - Email, FullName, Group có thể sửa
   - Link đến Reset Password
   - KHÔNG có password fields

4. **[ResetPassword.cshtml](file:///e:/Data/Ecommerce/src/Ecommerce.Web/Areas/Admin/Views/AdminUsers/ResetPassword.cshtml)**
   - Hiển thị username/email
   - New password + confirm password
   - Warning về mật khẩu cũ không còn dùng được

---

## Testing Instructions

### 1. Apply Migration

```bash
dotnet ef database update -p src/Ecommerce.Infrastructure -s src/Ecommerce.Web
```

Hoặc chạy ứng dụng, migration sẽ tự động apply.

### 2. Test CRUD Operations

1. **Truy cập**: Admin → Settings → Tài khoản Admin
2. **Create**: Tạo admin user mới
   - Test validation: username/email unique
   - Test password validation: min 6 chars
   - Test group assignment
3. **Edit**: Sửa thông tin admin user
   - Verify username readonly
   - Test email unique validation
   - Test group change
4. **Reset Password**: Đổi mật khẩu
   - Test password validation
   - Test confirm password match
5. **Toggle Active**: Bật/tắt tài khoản
6. **Delete**: Xóa admin user
   - Test confirmation dialog

### 3. Verify Database

```sql
-- Check GroupId column
SELECT "Id", "Username", "Email", "GroupId" 
FROM "AdminUsers";

-- Check foreign key relationship
SELECT au."Username", g."Name" as "GroupName"
FROM "AdminUsers" au
LEFT JOIN "Groups" g ON au."GroupId" = g."Id";

-- Test ON DELETE SET NULL
DELETE FROM "Groups" WHERE "Code" = 'TEST';
-- GroupId của admin users thuộc group TEST sẽ set NULL
```

---

## Important Notes

> [!IMPORTANT]
> **Migration**: Migration `AddGroupIdToAdminUser` đã được tạo nhưng chưa apply. Cần chạy `dotnet ef database update` hoặc restart ứng dụng để apply.

> [!NOTE]
> **Password Security**: Mật khẩu được hash bằng BCrypt (giống như hiện tại). Không thể xem lại mật khẩu, chỉ có thể reset.

> [!WARNING]
> **Foreign Key**: Khi xóa Group, `GroupId` của AdminUser sẽ set NULL (ON DELETE SET NULL). Admin user vẫn tồn tại nhưng không thuộc nhóm nào.

---

## Navigation

Link đến AdminUsers đã có sẵn trong Settings page:
- **Path**: Admin → Settings → Tài khoản Admin
- **Icon**: fas fa-user-tie (màu đỏ)

---

## Summary

✅ Database schema updated with GroupId foreign key  
✅ Migration created: AddGroupIdToAdminUser  
✅ Full CRUD controller with 6 actions  
✅ 4 views with validation and help panels  
✅ BCrypt password hashing  
✅ Search and pagination  
✅ Group assignment dropdown  
✅ Build successful
