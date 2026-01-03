-- Migration Script: Remove UserId from Accessories, Categories, and Subcategories
-- Run this script against your database to update the schema

-- WARNING: This will remove UserId columns and their associated data/indexes
-- Make sure you have a backup before running!

USE [Slingcessories_DB]
GO

PRINT 'Starting migration: Remove UserId from shared entities'
PRINT '------------------------------------------------------------'

-- Step 1: Drop indexes on UserId columns
PRINT 'Step 1: Dropping indexes...'

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Accessories_UserId' AND object_id = OBJECT_ID('dbo.Accessories'))
BEGIN
    DROP INDEX [IX_Accessories_UserId] ON [dbo].[Accessories]
    PRINT '  Dropped index IX_Accessories_UserId'
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_UserId' AND object_id = OBJECT_ID('dbo.Categories'))
BEGIN
    DROP INDEX [IX_Categories_UserId] ON [dbo].[Categories]
    PRINT '  Dropped index IX_Categories_UserId'
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subcategories_UserId' AND object_id = OBJECT_ID('dbo.Subcategories'))
BEGIN
    DROP INDEX [IX_Subcategories_UserId] ON [dbo].[Subcategories]
    PRINT '  Dropped index IX_Subcategories_UserId'
END

-- Step 2: Drop foreign key constraints (if any exist to Users table)
PRINT 'Step 2: Dropping foreign key constraints...'

DECLARE @sql NVARCHAR(MAX) = ''

-- Drop FK from Accessories to Users
SELECT @sql = 'ALTER TABLE [dbo].[Accessories] DROP CONSTRAINT [' + name + '];'
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('dbo.Accessories')
AND referenced_object_id = OBJECT_ID('dbo.Users')

IF @sql <> ''
BEGIN
    EXEC sp_executesql @sql
    PRINT '  Dropped FK from Accessories to Users'
    SET @sql = ''
END

-- Drop FK from Categories to Users
SELECT @sql = 'ALTER TABLE [dbo].[Categories] DROP CONSTRAINT [' + name + '];'
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('dbo.Categories')
AND referenced_object_id = OBJECT_ID('dbo.Users')

IF @sql <> ''
BEGIN
    EXEC sp_executesql @sql
    PRINT '  Dropped FK from Categories to Users'
    SET @sql = ''
END

-- Drop FK from Subcategories to Users
SELECT @sql = 'ALTER TABLE [dbo].[Subcategories] DROP CONSTRAINT [' + name + '];'
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('dbo.Subcategories')
AND referenced_object_id = OBJECT_ID('dbo.Users')

IF @sql <> ''
BEGIN
    EXEC sp_executesql @sql
    PRINT '  Dropped FK from Subcategories to Users'
END

-- Step 3: Drop UserId columns
PRINT 'Step 3: Dropping UserId columns...'

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Accessories') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[Accessories] DROP COLUMN [UserId]
    PRINT '  Dropped UserId column from Accessories'
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Categories') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[Categories] DROP COLUMN [UserId]
    PRINT '  Dropped UserId column from Categories'
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Subcategories') AND name = 'UserId')
BEGIN
    ALTER TABLE [dbo].[Subcategories] DROP COLUMN [UserId]
    PRINT '  Dropped UserId column from Subcategories'
END

PRINT '------------------------------------------------------------'
PRINT 'Migration completed successfully!'
PRINT 'Accessories, Categories, and Subcategories are now shared across all users.'
GO
