-- Simple script to add test data if your database is empty

-- Add a test category if none exists
IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    INSERT INTO Categories (Name) VALUES ('Test Category');
END

-- Add a test accessory if none exists
IF NOT EXISTS (SELECT 1 FROM Accessories)
BEGIN
    DECLARE @CategoryId INT = (SELECT TOP 1 Id FROM Categories ORDER BY Id);
    
    INSERT INTO Accessories (Title, PictureUrl, Units, Price, Url, Wishlist, CategoryId, SubcategoryId)
    VALUES ('Test Accessory', 'https://via.placeholder.com/150', 1, 9.99, 'https://example.com', 0, @CategoryId, NULL);
END

-- Verify
SELECT 'Categories' as TableName, COUNT(*) as Count FROM Categories
UNION ALL
SELECT 'Accessories', COUNT(*) FROM Accessories
UNION ALL
SELECT 'Slingshots', COUNT(*) FROM Slingshots
UNION ALL
SELECT 'AccessorySlingshots', COUNT(*) FROM AccessorySlingshots;
