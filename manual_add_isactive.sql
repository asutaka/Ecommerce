-- Manual fix for IsActive column
-- Run this SQL directly in your PostgreSQL database

-- Add IsActive column to Products table
ALTER TABLE "Products" 
ADD COLUMN "IsActive" boolean NOT NULL DEFAULT true;

-- Set existing products to active
UPDATE "Products" 
SET "IsActive" = true;

-- Verify the column was added
SELECT column_name, data_type, column_default 
FROM information_schema.columns 
WHERE table_name = 'Products' AND column_name = 'IsActive';
