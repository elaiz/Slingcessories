CREATE TABLE [dbo].[Accessories]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(MAX) NOT NULL,
    [PictureUrl] NVARCHAR(MAX) NULL,
    [Units] INT NOT NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [Url] NVARCHAR(MAX) NULL,
    [Wishlist] BIT NOT NULL DEFAULT 0,
    [CategoryId] INT NOT NULL,
    [SubcategoryId] INT NULL,
    CONSTRAINT [PK_Accessories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Accessories_Categories_CategoryId] 
        FOREIGN KEY ([CategoryId]) 
        REFERENCES [dbo].[Categories] ([Id]),
    CONSTRAINT [FK_Accessories_Subcategories_SubcategoryId] 
        FOREIGN KEY ([SubcategoryId]) 
        REFERENCES [dbo].[Subcategories] ([Id]) 
        ON DELETE SET NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_Accessories_CategoryId] 
    ON [dbo].[Accessories]([CategoryId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Accessories_SubcategoryId] 
    ON [dbo].[Accessories]([SubcategoryId] ASC);
GO
