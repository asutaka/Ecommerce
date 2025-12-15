DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'ecommerce') THEN
        CREATE SCHEMA ecommerce;
    END IF;
END $EF$;
CREATE TABLE IF NOT EXISTS ecommerce.__ef_migrations_history (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___ef_migrations_history" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Orders" (
    "Id" uuid NOT NULL,
    "CustomerName" character varying(200) NOT NULL,
    "CustomerEmail" character varying(320) NOT NULL,
    "Status" integer NOT NULL,
    "Total" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
);

CREATE TABLE "Products" (
    "Id" uuid NOT NULL,
    "Name" character varying(250) NOT NULL,
    "Description" text NOT NULL,
    "HeroImageUrl" character varying(1024) NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "IsFeatured" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id")
);

CREATE TABLE "ShoppingCarts" (
    "Id" uuid NOT NULL,
    "SessionId" character varying(128) NOT NULL,
    "UserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_ShoppingCarts" PRIMARY KEY ("Id")
);

CREATE TABLE "Notifications" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Channel" character varying(50) NOT NULL,
    "Destination" character varying(320) NOT NULL,
    "Template" text NOT NULL,
    "SentAt" timestamp with time zone NOT NULL,
    "Success" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Notifications_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "OrderItems" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "ProductName" character varying(200) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "Quantity" integer NOT NULL,
    "OrderId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Payments" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Method" text NOT NULL,
    "Status" integer NOT NULL,
    "ProcessedAt" timestamp with time zone,
    "Reference" character varying(64),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Payments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Payments_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CartItems" (
    "Id" uuid NOT NULL,
    "CartId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "ProductName" character varying(250) NOT NULL,
    "ProductImageUrl" character varying(1024) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "Quantity" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_CartItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CartItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_CartItems_ShoppingCarts_CartId" FOREIGN KEY ("CartId") REFERENCES "ShoppingCarts" ("Id") ON DELETE CASCADE
);

INSERT INTO "Products" ("Id", "CreatedAt", "Description", "HeroImageUrl", "IsFeatured", "Name", "Price", "UpdatedAt")
VALUES ('c812781e-3cc3-4cef-a0aa-3b40ffde4f74', TIMESTAMPTZ '2024-01-03T00:00:00Z', 'Bàn ăn đá cẩm thạch với đường nét tối giản.', 'https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60', FALSE, 'Nordic Marble Table', 980.0, NULL);
INSERT INTO "Products" ("Id", "CreatedAt", "Description", "HeroImageUrl", "IsFeatured", "Name", "Price", "UpdatedAt")
VALUES ('dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10', TIMESTAMPTZ '2024-01-01T00:00:00Z', 'Góc ngồi sang trọng với chất liệu da Ý cao cấp.', 'https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60', TRUE, 'Moderno Leather Chair', 420.0, NULL);
INSERT INTO "Products" ("Id", "CreatedAt", "Description", "HeroImageUrl", "IsFeatured", "Name", "Price", "UpdatedAt")
VALUES ('f5d9d030-8cde-41e4-ac7c-7e27c856223c', TIMESTAMPTZ '2024-01-02T00:00:00Z', 'Bộ đèn canvas cân bằng ánh sáng tự nhiên.', 'https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60', TRUE, 'Canvas Lighting Kit', 289.0, NULL);

CREATE INDEX "IX_CartItems_CartId" ON "CartItems" ("CartId");

CREATE INDEX "IX_CartItems_ProductId" ON "CartItems" ("ProductId");

CREATE UNIQUE INDEX "IX_Notifications_OrderId" ON "Notifications" ("OrderId");

CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");

CREATE UNIQUE INDEX "IX_Payments_OrderId" ON "Payments" ("OrderId");

CREATE INDEX "IX_ShoppingCarts_SessionId" ON "ShoppingCarts" ("SessionId");

CREATE INDEX "IX_ShoppingCarts_UserId" ON "ShoppingCarts" ("UserId");

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251201151656_AddShoppingCart', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "Products" DROP COLUMN "HeroImageUrl";

ALTER TABLE "Products" DROP COLUMN "Images";

ALTER TABLE "Products" ADD "Images" text[] NOT NULL DEFAULT '{}' NOT NULL;

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251201154655_UpdateProductImages', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "Products" ADD "CategoryId" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

CREATE TABLE "AdminUsers" (
    "Id" uuid NOT NULL,
    "Username" character varying(50) NOT NULL,
    "PasswordHash" character varying(256) NOT NULL,
    "Email" character varying(320) NOT NULL,
    "FullName" character varying(200),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_AdminUsers" PRIMARY KEY ("Id")
);

CREATE TABLE "Categories" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
);

INSERT INTO "AdminUsers" ("Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "UpdatedAt", "Username")
VALUES ('00000000-0000-0000-0000-000000000001', TIMESTAMPTZ '2025-12-02T04:30:19.853168Z', 'admin@ecommerce.com', 'Administrator', TRUE, '$2a$11$EvYW/yd3Ibd8FHcGtqm9COnDunJkQY3yVvgeBlXAAD5FwegIGDgNO', TIMESTAMPTZ '2025-12-02T04:30:19.853168Z', 'admin');

UPDATE "Products" SET "CategoryId" = '00000000-0000-0000-0000-000000000000', "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "CategoryId" = '00000000-0000-0000-0000-000000000000', "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "CategoryId" = '00000000-0000-0000-0000-000000000000', "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE INDEX "IX_Products_CategoryId" ON "Products" ("CategoryId");

CREATE UNIQUE INDEX "IX_AdminUsers_Email" ON "AdminUsers" ("Email");

CREATE UNIQUE INDEX "IX_AdminUsers_Username" ON "AdminUsers" ("Username");

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251202043021_AddCategory', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "ShoppingCarts" RENAME COLUMN "UserId" TO "CustomerId";

ALTER INDEX "IX_ShoppingCarts_UserId" RENAME TO "IX_ShoppingCarts_CustomerId";

ALTER TABLE "Orders" ADD "CustomerId" uuid;

CREATE TABLE "Customers" (
    "Id" uuid NOT NULL,
    "Email" character varying(320) NOT NULL,
    "PasswordHash" character varying(256) NOT NULL,
    "FullName" character varying(200),
    "Phone" character varying(20),
    "EmailConfirmed" boolean NOT NULL,
    "LastLoginAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

CREATE TABLE "ExternalLogins" (
    "Id" uuid NOT NULL,
    "Provider" character varying(50) NOT NULL,
    "ProviderKey" character varying(256) NOT NULL,
    "CustomerId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_ExternalLogins" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ExternalLogins_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE CASCADE
);

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-09T16:36:10.851818Z', "PasswordHash" = '$2a$11$7QCRjrtZIfUyw0mq6hx4zuWY0H5zEnBSx7GdfWOuf2b6wy0YqSG0C', "UpdatedAt" = TIMESTAMPTZ '2025-12-09T16:36:10.851818Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE INDEX "IX_Orders_CustomerId" ON "Orders" ("CustomerId");

CREATE UNIQUE INDEX "IX_Customers_Email" ON "Customers" ("Email");

CREATE INDEX "IX_ExternalLogins_CustomerId" ON "ExternalLogins" ("CustomerId");

CREATE UNIQUE INDEX "IX_ExternalLogins_Provider_ProviderKey" ON "ExternalLogins" ("Provider", "ProviderKey");

ALTER TABLE "Orders" ADD CONSTRAINT "FK_Orders_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE SET NULL;

ALTER TABLE "ShoppingCarts" ADD CONSTRAINT "FK_ShoppingCarts_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE SET NULL;

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251209163612_AddCustomer', '8.0.8');

COMMIT;

START TRANSACTION;

CREATE TABLE "Coupons" (
    "Id" uuid NOT NULL,
    "Code" character varying(50) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "DiscountAmount" numeric(18,2) NOT NULL,
    "DiscountPercentage" numeric(5,2),
    "MinimumOrderAmount" numeric(18,2) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "IsActive" boolean NOT NULL,
    "UsageLimit" integer NOT NULL,
    "UsedCount" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Coupons" PRIMARY KEY ("Id")
);

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-14T17:05:53.39904Z', "PasswordHash" = '$2a$11$p76vBaCoVsibTIETs8GWh.5qse8Gc.vK5ySDR0w1z2mSfXb2n2f9C', "UpdatedAt" = TIMESTAMPTZ '2025-12-14T17:05:53.39904Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE UNIQUE INDEX "IX_Coupons_Code" ON "Coupons" ("Code");

CREATE INDEX "IX_Coupons_IsActive" ON "Coupons" ("IsActive");

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251214170555_AddCouponTable', '8.0.8');

COMMIT;

START TRANSACTION;

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-14T17:08:06.756348Z', "PasswordHash" = '$2a$11$VNfSBPpUBCUBumss5lVkLeY8BKndFNs/fj/Iya9Dx0ICSoHCw6Xsq', "UpdatedAt" = TIMESTAMPTZ '2025-12-14T17:08:06.756348Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('6f4d0420-a892-4314-be9b-46e28553b18c', 'GIAM40K', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 'Giảm 40.000đ cho đơn hàng từ 500.000đ', 40000.0, NULL, TIMESTAMPTZ '2026-01-13T17:08:06.243286Z', TRUE, 500000.0, TIMESTAMPTZ '2025-12-07T17:08:06.243286Z', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 100, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('c1ffec36-0b3d-43c9-9a3e-f0f5caa8a047', 'GIAM50K', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 'Giảm 50.000đ cho đơn hàng từ 800.000đ', 50000.0, NULL, TIMESTAMPTZ '2026-01-28T17:08:06.243286Z', TRUE, 800000.0, TIMESTAMPTZ '2025-12-09T17:08:06.243286Z', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 50, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('c67947f3-1662-4cdf-878b-30dc6f751107', 'FREESHIP', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 'Miễn phí vận chuyển cho đơn hàng từ 300.000đ', 30000.0, NULL, TIMESTAMPTZ '2026-03-14T17:08:06.243286Z', TRUE, 300000.0, TIMESTAMPTZ '2025-12-04T17:08:06.243286Z', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 200, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('d5eba7ed-d836-460e-9c6c-d47f595663e6', 'GIAM100K', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 'Giảm 100.000đ cho đơn hàng từ 1.500.000đ', 100000.0, NULL, TIMESTAMPTZ '2026-02-12T17:08:06.243286Z', TRUE, 1500000.0, TIMESTAMPTZ '2025-12-11T17:08:06.243286Z', TIMESTAMPTZ '2025-12-14T17:08:06.243286Z', 30, 0);

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251214170807_SeedCouponData', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "Products" DROP CONSTRAINT IF EXISTS "FK_Products_Categories_CategoryId";

DROP INDEX IF EXISTS "IX_Products_CategoryId";

DELETE FROM "Coupons"
WHERE "Id" = '6f4d0420-a892-4314-be9b-46e28553b18c';

DELETE FROM "Coupons"
WHERE "Id" = 'c1ffec36-0b3d-43c9-9a3e-f0f5caa8a047';

DELETE FROM "Coupons"
WHERE "Id" = 'c67947f3-1662-4cdf-878b-30dc6f751107';

DELETE FROM "Coupons"
WHERE "Id" = 'd5eba7ed-d836-460e-9c6c-d47f595663e6';

ALTER TABLE "Products" DROP COLUMN "CategoryId";

ALTER TABLE "Products" ADD "PrimaryCategoryId" uuid;

CREATE TABLE "ProductCategories" (
    "ProductId" uuid NOT NULL,
    "CategoryId" uuid NOT NULL,
    CONSTRAINT "PK_ProductCategories" PRIMARY KEY ("ProductId", "CategoryId"),
    CONSTRAINT "FK_ProductCategories_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProductCategories_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-15T04:13:15.820862Z', "PasswordHash" = '$2a$11$yELtFxlbPAYwezMgjlWX.OfXOMpRjekvnUheBcb79WriCjH8YjmLq', "UpdatedAt" = TIMESTAMPTZ '2025-12-15T04:13:15.820862Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('3e752b0f-acf1-429f-bfd8-0bd8885bf98f', 'GIAM40K', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 'Giảm 40.000đ cho đơn hàng từ 500.000đ', 40000.0, NULL, TIMESTAMPTZ '2026-01-14T04:13:15.690603Z', TRUE, 500000.0, TIMESTAMPTZ '2025-12-08T04:13:15.690603Z', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 100, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('969abe04-bdf3-43d5-bbf5-16717578bcef', 'FREESHIP', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 'Miễn phí vận chuyển cho đơn hàng từ 300.000đ', 30000.0, NULL, TIMESTAMPTZ '2026-03-15T04:13:15.690603Z', TRUE, 300000.0, TIMESTAMPTZ '2025-12-05T04:13:15.690603Z', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 200, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('a095ce13-fefa-41d8-b520-c24182024737', 'GIAM50K', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 'Giảm 50.000đ cho đơn hàng từ 800.000đ', 50000.0, NULL, TIMESTAMPTZ '2026-01-29T04:13:15.690603Z', TRUE, 800000.0, TIMESTAMPTZ '2025-12-10T04:13:15.690603Z', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 50, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('bf149b6b-faf9-42a6-961b-7601c0ea0f3b', 'GIAM100K', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 'Giảm 100.000đ cho đơn hàng từ 1.500.000đ', 100000.0, NULL, TIMESTAMPTZ '2026-02-13T04:13:15.690603Z', TRUE, 1500000.0, TIMESTAMPTZ '2025-12-12T04:13:15.690603Z', TIMESTAMPTZ '2025-12-15T04:13:15.690603Z', 30, 0);

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[], "PrimaryCategoryId" = NULL
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[], "PrimaryCategoryId" = NULL
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[], "PrimaryCategoryId" = NULL
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE INDEX "IX_Products_PrimaryCategoryId" ON "Products" ("PrimaryCategoryId");

CREATE INDEX "IX_ProductCategories_CategoryId" ON "ProductCategories" ("CategoryId");

ALTER TABLE "Products" ADD CONSTRAINT "FK_Products_Categories_PrimaryCategoryId" FOREIGN KEY ("PrimaryCategoryId") REFERENCES "Categories" ("Id") ON DELETE SET NULL;

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251215041317_ManyToManyProductCategory', '8.0.8');

COMMIT;

START TRANSACTION;

DELETE FROM "Coupons"
WHERE "Id" = '3e752b0f-acf1-429f-bfd8-0bd8885bf98f';

DELETE FROM "Coupons"
WHERE "Id" = '969abe04-bdf3-43d5-bbf5-16717578bcef';

DELETE FROM "Coupons"
WHERE "Id" = 'a095ce13-fefa-41d8-b520-c24182024737';

DELETE FROM "Coupons"
WHERE "Id" = 'bf149b6b-faf9-42a6-961b-7601c0ea0f3b';

ALTER TABLE "Categories" ADD "ParentId" uuid;

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-15T04:20:47.405588Z', "PasswordHash" = '$2a$11$64WSIQ1w06UOix/2NcKxZOznVwPvMV1a7QVSbEJaLFmtNbVejC8C2', "UpdatedAt" = TIMESTAMPTZ '2025-12-15T04:20:47.405588Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('25d75562-ce14-455f-86fc-eb54d4636319', 'FREESHIP', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 'Miễn phí vận chuyển cho đơn hàng từ 300.000đ', 30000.0, NULL, TIMESTAMPTZ '2026-03-15T04:20:47.102081Z', TRUE, 300000.0, TIMESTAMPTZ '2025-12-05T04:20:47.102081Z', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 200, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('83434861-f0ee-4be2-8273-9bcd6e231a74', 'GIAM40K', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 'Giảm 40.000đ cho đơn hàng từ 500.000đ', 40000.0, NULL, TIMESTAMPTZ '2026-01-14T04:20:47.102081Z', TRUE, 500000.0, TIMESTAMPTZ '2025-12-08T04:20:47.102081Z', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 100, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('838ed180-5318-40c5-8303-e28e7e20eae2', 'GIAM100K', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 'Giảm 100.000đ cho đơn hàng từ 1.500.000đ', 100000.0, NULL, TIMESTAMPTZ '2026-02-13T04:20:47.102081Z', TRUE, 1500000.0, TIMESTAMPTZ '2025-12-12T04:20:47.102081Z', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 30, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('a3efc911-b4f2-4cdb-acfb-0df721793598', 'GIAM50K', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 'Giảm 50.000đ cho đơn hàng từ 800.000đ', 50000.0, NULL, TIMESTAMPTZ '2026-01-29T04:20:47.102081Z', TRUE, 800000.0, TIMESTAMPTZ '2025-12-10T04:20:47.102081Z', TIMESTAMPTZ '2025-12-15T04:20:47.102081Z', 50, 0);

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE INDEX "IX_Categories_ParentId" ON "Categories" ("ParentId");

ALTER TABLE "Categories" ADD CONSTRAINT "FK_Categories_Categories_ParentId" FOREIGN KEY ("ParentId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT;

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251215042047_AddCategoryHierarchy', '8.0.8');

COMMIT;

START TRANSACTION;

DELETE FROM "Coupons"
WHERE "Id" = '25d75562-ce14-455f-86fc-eb54d4636319';

DELETE FROM "Coupons"
WHERE "Id" = '83434861-f0ee-4be2-8273-9bcd6e231a74';

DELETE FROM "Coupons"
WHERE "Id" = '838ed180-5318-40c5-8303-e28e7e20eae2';

DELETE FROM "Coupons"
WHERE "Id" = 'a3efc911-b4f2-4cdb-acfb-0df721793598';

CREATE TABLE "Suppliers" (
    "Id" uuid NOT NULL,
    "Code" character varying(50) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Address" character varying(500) NOT NULL,
    "PhoneNumbers" text[] NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Suppliers" PRIMARY KEY ("Id")
);

CREATE TABLE "Warehouses" (
    "Id" uuid NOT NULL,
    "Code" character varying(50) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Address" character varying(500) NOT NULL,
    "PhoneNumbers" text[] NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Warehouses" PRIMARY KEY ("Id")
);

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-15T06:07:50.251652Z', "PasswordHash" = '$2a$11$J8Dn0/CJzPYsHGmS1OVOBO0XW.FQyyjvDYqlUMXhKnwW1dUil311e', "UpdatedAt" = TIMESTAMPTZ '2025-12-15T06:07:50.251652Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('15738cbc-31c4-4d2c-8a8c-fedbc5b0bdd4', 'GIAM40K', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 'Giảm 40.000đ cho đơn hàng từ 500.000đ', 40000.0, NULL, TIMESTAMPTZ '2026-01-14T06:07:49.947014Z', TRUE, 500000.0, TIMESTAMPTZ '2025-12-08T06:07:49.947014Z', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 100, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('1f53f4c5-3f0a-409e-afd7-24891b1867fd', 'GIAM100K', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 'Giảm 100.000đ cho đơn hàng từ 1.500.000đ', 100000.0, NULL, TIMESTAMPTZ '2026-02-13T06:07:49.947014Z', TRUE, 1500000.0, TIMESTAMPTZ '2025-12-12T06:07:49.947014Z', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 30, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('5ddfa261-c28d-401f-9890-9d8de000c1c9', 'GIAM50K', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 'Giảm 50.000đ cho đơn hàng từ 800.000đ', 50000.0, NULL, TIMESTAMPTZ '2026-01-29T06:07:49.947014Z', TRUE, 800000.0, TIMESTAMPTZ '2025-12-10T06:07:49.947014Z', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 50, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('c477cbfb-f831-47fc-b091-dd0fa892778b', 'FREESHIP', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 'Miễn phí vận chuyển cho đơn hàng từ 300.000đ', 30000.0, NULL, TIMESTAMPTZ '2026-03-15T06:07:49.947014Z', TRUE, 300000.0, TIMESTAMPTZ '2025-12-05T06:07:49.947014Z', TIMESTAMPTZ '2025-12-15T06:07:49.947014Z', 200, 0);

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE UNIQUE INDEX "IX_Suppliers_Code" ON "Suppliers" ("Code");

CREATE UNIQUE INDEX "IX_Warehouses_Code" ON "Warehouses" ("Code");

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251215060751_AddSupplierAndWarehouse', '8.0.8');

COMMIT;

START TRANSACTION;

DELETE FROM "Coupons"
WHERE "Id" = '15738cbc-31c4-4d2c-8a8c-fedbc5b0bdd4';

DELETE FROM "Coupons"
WHERE "Id" = '1f53f4c5-3f0a-409e-afd7-24891b1867fd';

DELETE FROM "Coupons"
WHERE "Id" = '5ddfa261-c28d-401f-9890-9d8de000c1c9';

DELETE FROM "Coupons"
WHERE "Id" = 'c477cbfb-f831-47fc-b091-dd0fa892778b';

CREATE TABLE "Groups" (
    "Id" uuid NOT NULL,
    "Code" character varying(50) NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(500),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Groups" PRIMARY KEY ("Id")
);

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-15T09:33:16.686248Z', "PasswordHash" = '$2a$11$zqzS3ECHcThAXbD3RFAaHuB3x3a3JbLiKCcw4AYXqdvELzsEtz0gm', "UpdatedAt" = TIMESTAMPTZ '2025-12-15T09:33:16.686249Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('38cdf653-2d9c-405e-8949-ddac2e00a7ac', 'GIAM100K', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 'Giảm 100.000đ cho đơn hàng từ 1.500.000đ', 100000.0, NULL, TIMESTAMPTZ '2026-02-13T09:33:16.555615Z', TRUE, 1500000.0, TIMESTAMPTZ '2025-12-12T09:33:16.555615Z', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 30, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('7508f25e-79f7-4506-adaa-8c527848d031', 'FREESHIP', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 'Miễn phí vận chuyển cho đơn hàng từ 300.000đ', 30000.0, NULL, TIMESTAMPTZ '2026-03-15T09:33:16.555615Z', TRUE, 300000.0, TIMESTAMPTZ '2025-12-05T09:33:16.555615Z', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 200, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('bf1762dd-5d1e-4bd1-9ebf-a4c74315536d', 'GIAM40K', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 'Giảm 40.000đ cho đơn hàng từ 500.000đ', 40000.0, NULL, TIMESTAMPTZ '2026-01-14T09:33:16.555615Z', TRUE, 500000.0, TIMESTAMPTZ '2025-12-08T09:33:16.555615Z', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 100, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('cd706f41-e87c-4dae-ac23-2959c16b7ce5', 'GIAM50K', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 'Giảm 50.000đ cho đơn hàng từ 800.000đ', 50000.0, NULL, TIMESTAMPTZ '2026-01-29T09:33:16.555615Z', TRUE, 800000.0, TIMESTAMPTZ '2025-12-10T09:33:16.555615Z', TIMESTAMPTZ '2025-12-15T09:33:16.555615Z', 50, 0);

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE UNIQUE INDEX "IX_Groups_Code" ON "Groups" ("Code");

CREATE INDEX "IX_Groups_IsActive" ON "Groups" ("IsActive");

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251215093318_AddGroupEntity', '8.0.8');

COMMIT;

START TRANSACTION;

ALTER TABLE "Groups" ADD "Permissions" jsonb NOT NULL DEFAULT ('{}'::jsonb);

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251215100304_AddPermissionsToGroup', '8.0.8');

COMMIT;

START TRANSACTION;

DELETE FROM "Coupons"
WHERE "Id" = 'b9ed7b14-469f-4368-bd59-e00d546aab65';

DELETE FROM "Coupons"
WHERE "Id" = 'be44c249-019e-4e9c-9175-6d2e4d9d5cc7';

DELETE FROM "Coupons"
WHERE "Id" = 'e84040a4-e72b-4b9b-97ea-27e5b129ef2e';

DELETE FROM "Coupons"
WHERE "Id" = 'ef69a7fb-8da0-4dd4-a9e8-a994c18315c6';

ALTER TABLE "AdminUsers" ADD "GroupId" uuid;

UPDATE "AdminUsers" SET "CreatedAt" = TIMESTAMPTZ '2025-12-15T13:36:26.267149Z', "GroupId" = NULL, "PasswordHash" = '$2a$11$tRnu2F7Wro2/kI766P9bgutnChn2pgmCv6mUkZjbCnsVxjbFdjDtu', "UpdatedAt" = TIMESTAMPTZ '2025-12-15T13:36:26.26715Z'
WHERE "Id" = '00000000-0000-0000-0000-000000000001';

INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('69c18bf4-5fb9-48b6-b418-88f17ce0fc0b', 'FREESHIP', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 'Miễn phí vận chuyển cho đơn hàng từ 300.000đ', 30000.0, NULL, TIMESTAMPTZ '2026-03-15T13:36:26.137226Z', TRUE, 300000.0, TIMESTAMPTZ '2025-12-05T13:36:26.137226Z', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 200, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('76087f3f-2674-4a87-b45a-f0bb56391a3a', 'GIAM40K', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 'Giảm 40.000đ cho đơn hàng từ 500.000đ', 40000.0, NULL, TIMESTAMPTZ '2026-01-14T13:36:26.137226Z', TRUE, 500000.0, TIMESTAMPTZ '2025-12-08T13:36:26.137226Z', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 100, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('8b9ff976-d40e-4da2-856f-9519d5049ba8', 'GIAM50K', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 'Giảm 50.000đ cho đơn hàng từ 800.000đ', 50000.0, NULL, TIMESTAMPTZ '2026-01-29T13:36:26.137226Z', TRUE, 800000.0, TIMESTAMPTZ '2025-12-10T13:36:26.137226Z', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 50, 0);
INSERT INTO "Coupons" ("Id", "Code", "CreatedAt", "Description", "DiscountAmount", "DiscountPercentage", "EndDate", "IsActive", "MinimumOrderAmount", "StartDate", "UpdatedAt", "UsageLimit", "UsedCount")
VALUES ('c14a0ff9-2e7f-4b54-b31d-82660ceedc53', 'GIAM100K', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 'Giảm 100.000đ cho đơn hàng từ 1.500.000đ', 100000.0, NULL, TIMESTAMPTZ '2026-02-13T13:36:26.137226Z', TRUE, 1500000.0, TIMESTAMPTZ '2025-12-12T13:36:26.137226Z', TIMESTAMPTZ '2025-12-15T13:36:26.137226Z', 30, 0);

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'c812781e-3cc3-4cef-a0aa-3b40ffde4f74';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1503602642458-232111445657?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'dc1c2d1d-a2bb-45f7-9e1b-87cdc53a7a10';

UPDATE "Products" SET "Images" = ARRAY['https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=60']::text[]
WHERE "Id" = 'f5d9d030-8cde-41e4-ac7c-7e27c856223c';

CREATE INDEX "IX_AdminUsers_GroupId" ON "AdminUsers" ("GroupId");

ALTER TABLE "AdminUsers" ADD CONSTRAINT "FK_AdminUsers_Groups_GroupId" FOREIGN KEY ("GroupId") REFERENCES "Groups" ("Id") ON DELETE SET NULL;

INSERT INTO ecommerce.__ef_migrations_history ("MigrationId", "ProductVersion")
VALUES ('20251215133627_AddGroupIdToAdminUser', '8.0.8');

COMMIT;

