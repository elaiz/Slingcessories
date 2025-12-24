CREATE TABLE [dbo].[Categories]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Name] 
    ON [dbo].[Categories]([Name] ASC);
GO
