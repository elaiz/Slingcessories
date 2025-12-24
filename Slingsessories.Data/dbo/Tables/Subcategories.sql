CREATE TABLE [dbo].[Subcategories]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(450) NOT NULL,
    [CategoryId] INT NOT NULL,
    CONSTRAINT [PK_Subcategories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Subcategories_Categories_CategoryId] 
        FOREIGN KEY ([CategoryId]) 
        REFERENCES [dbo].[Categories] ([Id]) 
        ON DELETE CASCADE
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Subcategories_CategoryId_Name] 
    ON [dbo].[Subcategories]([CategoryId] ASC, [Name] ASC);
GO
