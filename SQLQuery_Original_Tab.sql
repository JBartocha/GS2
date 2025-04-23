CREATE TABLE [dbo].[Food] (
    [Food_ID] INT      IDENTITY (1, 1) NOT NULL,
    [PosX]    SMALLINT NOT NULL,
    [PosY]    SMALLINT NOT NULL,
    PRIMARY KEY CLUSTERED ([Food_ID] ASC)
);


CREATE TABLE [dbo].[GameNumbers] (
    [GameNumbers_ID] INT      IDENTITY (1, 1) NOT NULL,
    [GameNumber]     INT      NOT NULL,
    [Date]           DATETIME NOT NULL,
    [Settings]       TEXT     NOT NULL,
    PRIMARY KEY CLUSTERED ([GameNumbers_ID] ASC),
    UNIQUE NONCLUSTERED ([GameNumber] ASC)
);

CREATE TABLE [dbo].[SnakeMoves] (
    [SnakeMoves_ID] INT          IDENTITY (1, 1) NOT NULL,
    [GameID]        INT          NOT NULL,
    [MoveNumber]    INT          NOT NULL,
    [Direction]     VARCHAR (10) NOT NULL,
    [FoodID]        INT          NULL,
    PRIMARY KEY CLUSTERED ([SnakeMoves_ID] ASC),
    UNIQUE NONCLUSTERED ([FoodID] ASC),
    CONSTRAINT [GameIDFK] FOREIGN KEY ([GameID]) REFERENCES [dbo].[GameNumbers] ([GameNumbers_ID]),
    CONSTRAINT [FoodFK] FOREIGN KEY ([FoodID]) REFERENCES [dbo].[Food] ([Food_ID])
);

CREATE TABLE [dbo].[FoodSettings] (
    [FoodSettings_ID] INT IDENTITY (1, 1) NOT NULL,
    [GameID]          INT NOT NULL,
    [FoodID]          INT NOT NULL,
    PRIMARY KEY CLUSTERED ([FoodSettings_ID] ASC),
    CONSTRAINT [FoodToSettingsFK] FOREIGN KEY ([FoodID]) REFERENCES [dbo].[Food] ([Food_ID]),
    FOREIGN KEY ([GameID]) REFERENCES [dbo].[GameNumbers] ([GameNumbers_ID])
);
