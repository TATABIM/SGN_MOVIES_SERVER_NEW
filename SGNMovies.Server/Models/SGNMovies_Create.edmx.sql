
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/18/2012 15:01:04
-- Generated from EDMX file: C:\Users\THANH BINH\Desktop\sgnmovies-server\SGNMovies.Server\Models\SGNMovies.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SGNMOVIE];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Providers'
CREATE TABLE [dbo].[Providers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [HostUrl] nvarchar(max)  NOT NULL,
);
GO

-- Creating table 'Cinemas'
CREATE TABLE [dbo].[Cinemas] (
    [Id] int IDENTITY(1,1) NOT NULL,
	[CinemaWebId] nvarchar(MAX) NOT NULL,
	[Name] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Latitude] float  NULL,
    [Longitude] float  NULL,
    [ImageUrl] nvarchar(max) NULL,
    [MapUrl] nvarchar(max) NULL,
);
GO

-- Creating table 'Movies'
CREATE TABLE [dbo].[Movies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MovieWebId] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Director] nvarchar(max)  NULL,
    [Duration] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [Genre] nvarchar(max)  NULL,
    [Cast] nvarchar(max)  NULL,
    [Language] nvarchar(max)  NULL,
    [Producer] nvarchar(max)  NULL,
    [Version] nvarchar(max)  NULL,
    [IsNowShowing] bit  NOT NULL,
    [InfoUrl] nvarchar(max)  NOT NULL,
    [ImageUrl] nvarchar(max)  NULL,
    [TrailerUrl] nvarchar(max)  NULL
);
GO

-- Creating table 'ProviderCinemas'
CREATE TABLE [dbo].[ProviderCinemas] (
    [Id] int IDENTITY(1,1) NOT NULL,
	[Provider_Id] int NOT NULL,
    [Cinema_Id] int  NOT NULL,
);
GO

-- Creating table 'SessionTimes'
CREATE TABLE [dbo].[SessionTimes] (
	[Id] int IDENTITY(1,1) NOT NULL,
    [ProviderCinema_Id] int NOT NULL,
    [Movie_Id] int NOT NULL,
    [Date] nvarchar(max)  NOT NULL,
    [Time] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Providers'
ALTER TABLE [dbo].[Providers]
ADD CONSTRAINT [PK_Providers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Cinemas'
ALTER TABLE [dbo].[Cinemas]
ADD CONSTRAINT [PK_Cinemas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Movies'
ALTER TABLE [dbo].[Movies]
ADD CONSTRAINT [PK_Movies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProviderCinemas'
ALTER TABLE [dbo].[ProviderCinemas]
ADD CONSTRAINT [PK_ProviderCinemas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SessionTimes'
ALTER TABLE [dbo].[SessionTimes]
ADD CONSTRAINT [PK_SessionTimes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Session_Id] in table 'SessionTimes'
ALTER TABLE [dbo].[SessionTimes]
ADD CONSTRAINT [FK_STProviderCinema]
    FOREIGN KEY ([ProviderCinema_Id])
    REFERENCES [dbo].[ProviderCinemas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SessionTime'
CREATE INDEX [IX_STProviderCinema]
ON [dbo].[SessionTimes]
    ([ProviderCinema_Id]);
GO

-- Creating foreign key on [Session_Id] in table 'SessionTimes'
ALTER TABLE [dbo].[SessionTimes]
ADD CONSTRAINT [FK_STMovie]
    FOREIGN KEY ([Movie_Id])
    REFERENCES [dbo].[Movies]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CinemaSession'
CREATE INDEX [IX_FK_STMovie]
ON [dbo].[SessionTimes]
    ([Movie_Id]);
GO

-- Creating foreign key on [MSession_Id] in table 'ProviderCinemas'
ALTER TABLE [dbo].[ProviderCinemas]
ADD CONSTRAINT [FK_PCProvider]
    FOREIGN KEY ([Provider_Id])
    REFERENCES [dbo].[Providers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProviderSession'
CREATE INDEX [IX_FK_PCProvider]
ON [dbo].[ProviderCinemas]
    ([Provider_Id]);
GO

-- Creating foreign key on [Session_Id] in table 'ProviderCinemas'
ALTER TABLE [dbo].[ProviderCinemas]
ADD CONSTRAINT [FK_PCCinemas]
    FOREIGN KEY ([Cinema_Id])
    REFERENCES [dbo].[Cinemas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MovieSession'
CREATE INDEX [FK_PCCinema]
ON [dbo].[ProviderCinemas]
    ([Cinema_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------