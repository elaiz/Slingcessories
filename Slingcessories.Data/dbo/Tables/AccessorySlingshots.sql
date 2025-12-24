CREATE TABLE [dbo].[AccessorySlingshots] (
    [AccessoryId] INT NOT NULL,
    [SlingshotId] INT NOT NULL,
    CONSTRAINT [PK_AccessorySlingshots] PRIMARY KEY CLUSTERED ([AccessoryId] ASC, [SlingshotId] ASC),
    CONSTRAINT [FK_AccessorySlingshots_Accessories_AccessoryId] FOREIGN KEY ([AccessoryId]) REFERENCES [dbo].[Accessories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AccessorySlingshots_Slingshots_SlingshotId] FOREIGN KEY ([SlingshotId]) REFERENCES [dbo].[Slingshots] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AccessorySlingshots_SlingshotId]
    ON [dbo].[AccessorySlingshots]([SlingshotId] ASC);

