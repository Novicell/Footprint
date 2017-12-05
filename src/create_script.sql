CREATE TABLE [ncBtAction](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[SegmentId] [int] NOT NULL,
	[ActionType] [int] NOT NULL,
	[EmailPropertyId] [int] NULL,
	[EmailSubject] [nvarchar](255) NULL,
	[EmailNodeId] [int] NULL
)

CREATE TABLE [ncBtSegment](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
)

CREATE TABLE [ncBtProperty](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[DataType] [int] NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Examples] [nvarchar](1000) NULL,
)

CREATE TABLE [ncBtOperator](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[DisplayName] [nvarchar](255) NULL,
	[IsInverted] [bit] NOT NULL,
	[OperatorType] [int] NOT NULL,
	[DataType] [int] NOT NULL,
)

CREATE TABLE [ncBtCriterionGroup](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[SegmentId] [int] NOT NULL,
	[IsInclude] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
)

CREATE TABLE [ncBtCriterion](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CriterionGroupId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[PropertyAlias] [nvarchar](255) NOT NULL,
	[PropertyValue] [nvarchar](255) NULL,
	[SortOrder] [int] NOT NULL,
)

GO

CREATE TYPE [dbo].[IdList] AS TABLE(
	[Id] [int] NULL
)

GO

CREATE TYPE [dbo].[AliasList] AS TABLE(
	[Alias] [varchar](255) NULL
)

GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [OperatorGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch operators as temp table
    SELECT
                    o.*
	INTO			
					#tmpDataOperators
    FROM
                    ncBtOperator o
    WHERE
                    o.Id IN (SELECT Id FROM @IdList)

	-- Fetch operator from temp table
	SELECT
					* 
	FROM
					#tmpDataOperators
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [OperatorGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[OperatorGetByIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByCriterionGroupIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterions as temp table
    SELECT
                    c.*
	INTO			
					#tmpDataCriterions 
    FROM
                    ncBtCriterion c
    WHERE
                    c.CriterionGroupId IN (SELECT Id FROM @IdList)

	-- Fetch criterions from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterions

	-- Fetch criterion operator ids from temp table as a new id list
	DECLARE @OperatorIdList AS [dbo].[IdList];
	INSERT INTO
					@OperatorIdList
		SELECT DISTINCT
					OperatorId AS Id
		FROM
					#tmpDataCriterions
	
	-- Fetch operators using stored procedure
	EXEC			[dbo].[OperatorGetByIdsStp]
					@IdList = @OperatorIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterion as temp table
    SELECT
                    c.*
	INTO			
					#tmpDataCriterions 
    FROM
                    ncBtCriterion c
    WHERE
                    c.Id IN (SELECT Id FROM @IdList)

	-- Fetch criterion from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterions

	-- Fetch operator ids from temp table as a new id list
	DECLARE @OperatorIdList AS [dbo].[IdList];
	INSERT INTO
					@OperatorIdList
		SELECT DISTINCT
					OperatorId AS Id
		FROM
					#tmpDataCriterions
	
	-- Fetch data types using stored procedure
	EXEC			[dbo].[OperatorGetByIdsStp]
					@IdList = @OperatorIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterion groups as temp table
    SELECT
                    cg.*
	INTO			
					#tmpDataCriterionGroup
    FROM
                    ncBtCriterionGroup cg
    WHERE
                    cg.Id IN (SELECT Id FROM @IdList)

	-- Fetch criterion group from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterionGroup

	-- Fetch operator ids from temp table as a new id list
	DECLARE @CriterionGroupIdList AS [dbo].[IdList];
	INSERT INTO
					@CriterionGroupIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataCriterionGroup
	
	-- Fetch data types using stored procedure
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStp]
					@IdList = @CriterionGroupIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGetByIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByCriterionGroupIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetBySegmentIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterion groups as temp table
    SELECT
                    c.*
	INTO			
					#tmpDataCriterionGroups
    FROM
                    ncBtCriterionGroup c
    WHERE
                    c.SegmentId IN (SELECT Id FROM @IdList)

	-- Fetch criterion groups from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterionGroups

	-- Fetch criterion group ids from temp table as a new id list
	DECLARE @CriterionIdList AS [dbo].[IdList];
	INSERT INTO
					@CriterionIdList
		SELECT DISTINCT
					Id
		FROM
					#tmpDataCriterionGroups
	
	-- Fetch criterions using stored procedure
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStp]
					@IdList = @CriterionIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGroupGetByIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetBySegmentIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetAllStp] 

AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch segment as temp table
    SELECT
                    s.*
	INTO			
					#tmpDataSegment
    FROM
                    ncBtSegment s

	-- Fetch segment from temp table
	SELECT
					* 
	FROM
					#tmpDataSegment

	-- Fetch segment ids from temp table as a new id list
	DECLARE @SegmentIdList AS [dbo].[IdList];
	INSERT INTO
					@SegmentIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataSegment

	-- Fetch criterion groups using stored procedure
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStp]
					@IdList = @SegmentIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch segment as temp table
    SELECT
                    s.*
	INTO			
					#tmpDataSegment
    FROM
                    ncBtSegment s
    WHERE
                    s.Id IN (SELECT Id FROM @IdList)

	-- Fetch segment from temp table
	SELECT
					* 
	FROM
					#tmpDataSegment

	-- Fetch segment ids from temp table as a new id list
	DECLARE @SegmentIdList AS [dbo].[IdList];
	INSERT INTO
					@SegmentIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataSegment

	-- Fetch criterion groups using stored procedure
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStp]
					@IdList = @SegmentIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[SegmentGetByIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByAliasMultipleStp] 
	@AliasList AS [dbo].[AliasList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList SELECT Id FROM ncBtSegment WHERE Alias IN (SELECT * FROM @AliasList)
	EXEC			[dbo].[SegmentGetByIdsStp]
					@IdList = @IdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByAliasStp] 
	@Alias varchar(255)
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @AliasList AS [dbo].[AliasList];
	INSERT INTO @AliasList (Alias) VALUES (@Alias)
	EXEC			[dbo].[SegmentGetByAliasMultipleStp]
					@AliasList = @AliasList
END
GO