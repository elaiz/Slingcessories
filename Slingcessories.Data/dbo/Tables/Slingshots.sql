CREATE TABLE [dbo].[Slingshots] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Year]   INT            NOT NULL,
    [Model]  NVARCHAR (450) NOT NULL,
    [Color]  NVARCHAR (450) NOT NULL,
    [UserId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_Slingshots] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Slingshots_Year_Model_Color]
    ON [dbo].[Slingshots]([Year] ASC, [Model] ASC, [Color] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Slingshots_UserId]
    ON [dbo].[Slingshots]([UserId] ASC);

