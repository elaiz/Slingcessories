-- Script to copy data from Slinsessories database to Slingcessories database
-- Run this script while connected to the NEW Slingcessories database

USE Slingcessories;
GO

-- Copy Categories
SET IDENTITY_INSERT Categories ON;

INSERT INTO Categories (Id, Name)
SELECT Id, Name
FROM Slinsessories.dbo.Categories
WHERE NOT EXISTS (
    SELECT 1 FROM Categories WHERE Categories.Id = Slinsessories.dbo.Categories.Id
);

SET IDENTITY_INSERT Categories OFF;
GO

-- Copy Accessories
SET IDENTITY_INSERT Accessories ON;

INSERT INTO Accessories (Id, PictureUrl, Title, Units, Price, Url, Wishlist, CategoryId)
SELECT Id, PictureUrl, Title, Units, Price, Url, Wishlist, CategoryId
FROM Slinsessories.dbo.Accessories
WHERE NOT EXISTS (
    SELECT 1 FROM Accessories WHERE Accessories.Id = Slinsessories.dbo.Accessories.Id
);

SET IDENTITY_INSERT Accessories OFF;
GO

-- Copy Slingshots (if they exist in old database)
IF EXISTS (SELECT * FROM Slinsessories.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Slingshots')
BEGIN
    SET IDENTITY_INSERT Slingshots ON;

    INSERT INTO Slingshots (Id, Year, Model, Color)
    SELECT Id, Year, Model, Color
    FROM Slinsessories.dbo.Slingshots
    WHERE NOT EXISTS (
        SELECT 1 FROM Slingshots WHERE Slingshots.Id = Slinsessories.dbo.Slingshots.Id
    );

    SET IDENTITY_INSERT Slingshots OFF;
END
GO

-- Copy AccessorySlingshots (if they exist in old database)
IF EXISTS (SELECT * FROM Slinsessories.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AccessorySlingshots')
BEGIN
    INSERT INTO AccessorySlingshots (AccessoryId, SlingshotId)
    SELECT AccessoryId, SlingshotId
    FROM Slinsessories.dbo.AccessorySlingshots
    WHERE NOT EXISTS (
        SELECT 1 FROM AccessorySlingshots 
        WHERE AccessorySlingshots.AccessoryId = Slinsessories.dbo.AccessorySlingshots.AccessoryId
        AND AccessorySlingshots.SlingshotId = Slinsessories.dbo.AccessorySlingshots.SlingshotId
    );
END
GO

PRINT 'Data copy completed successfully!';
