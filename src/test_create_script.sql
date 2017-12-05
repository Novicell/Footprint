CREATE TABLE [ncBtTESTAction](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[SegmentId] [int] NOT NULL,
	[ActionType] [int] NOT NULL,
	[EmailPropertyId] [int] NULL,
	[EmailSubject] [nvarchar](255) NULL,
	[EmailNodeId] [int] NULL
)

CREATE TABLE [ncBtTESTSegment](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
)

CREATE TABLE [ncBtTESTProperty](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[DataType] [int] NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Examples] [nvarchar](1000) NULL,
)

CREATE TABLE [ncBtTESTOperator](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[DisplayName] [nvarchar](255) NULL,
	[IsInverted] [bit] NOT NULL,
	[OperatorType] [int] NOT NULL,
	[DataType] [int] NOT NULL,
)

CREATE TABLE [ncBtTESTCriterionGroup](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[SegmentId] [int] NOT NULL,
	[IsInclude] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
)

CREATE TABLE [ncBtTESTCriterion](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CriterionGroupId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[PropertyAlias] [nvarchar](255) NOT NULL,
	[PropertyValue] [nvarchar](255) NULL,
	[SortOrder] [int] NOT NULL,
)

GO

CREATE TYPE [dbo].[ncBtTESTIdList] AS TABLE(
	[Id] [int] NULL
)

GO

CREATE TYPE [dbo].[ncBtTESTAliasList] AS TABLE(
	[Alias] [varchar](255) NULL
)

GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [OperatorGetByIdsStpNcBtTest] 
	@ncBtTESTIdList AS [dbo].[ncBtTESTIdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch operators as temp table
    SELECT
                    o.*
	INTO			
					#tmpDataOperators
    FROM
                    ncBtTESTOperator o
    WHERE
                    o.Id IN (SELECT Id FROM @ncBtTESTIdList)

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
CREATE PROCEDURE [OperatorGetByIdStpNcBtTest] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList (Id) VALUES (@Id)
	EXEC			[dbo].[OperatorGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByCriterionGroupIdsStpNcBtTest] 
	@ncBtTESTIdList AS [dbo].[ncBtTESTIdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterions as temp table
    SELECT
                    c.*
	INTO			
					#tmpDataCriterions 
    FROM
                    ncBtTESTCriterion c
    WHERE
                    c.CriterionGroupId IN (SELECT Id FROM @ncBtTESTIdList)

	-- Fetch criterions from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterions

	-- Fetch criterion operator ids from temp table as a new id list
	DECLARE @OperatorncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO
					@OperatorncBtTESTIdList
		SELECT DISTINCT
					OperatorId AS Id
		FROM
					#tmpDataCriterions
	
	-- Fetch operators using stored procedure
	EXEC			[dbo].[OperatorGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @OperatorncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByIdsStpNcBtTest] 
	@ncBtTESTIdList AS [dbo].[ncBtTESTIdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterion as temp table
    SELECT
                    c.*
	INTO			
					#tmpDataCriterions 
    FROM
                    ncBtTESTCriterion c
    WHERE
                    c.Id IN (SELECT Id FROM @ncBtTESTIdList)

	-- Fetch criterion from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterions

	-- Fetch operator ids from temp table as a new id list
	DECLARE @OperatorncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO
					@OperatorncBtTESTIdList
		SELECT DISTINCT
					OperatorId AS Id
		FROM
					#tmpDataCriterions
	
	-- Fetch data types using stored procedure
	EXEC			[dbo].[OperatorGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @OperatorncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetByIdsStpNcBtTest] 
	@ncBtTESTIdList AS [dbo].[ncBtTESTIdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterion groups as temp table
    SELECT
                    cg.*
	INTO			
					#tmpDataCriterionGroup
    FROM
                    ncBtTESTCriterionGroup cg
    WHERE
                    cg.Id IN (SELECT Id FROM @ncBtTESTIdList)

	-- Fetch criterion group from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterionGroup

	-- Fetch operator ids from temp table as a new id list
	DECLARE @CriterionGroupncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO
					@CriterionGroupncBtTESTIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataCriterionGroup
	
	-- Fetch data types using stored procedure
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStpNcBtTest]
					@ncBtTESTIdList = @CriterionGroupncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByIdStpNcBtTest] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGetByCriterionGroupIdStpNcBtTest] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetBySegmentIdsStpNcBtTest] 
	@ncBtTESTIdList AS [dbo].[ncBtTESTIdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch criterion groups as temp table
    SELECT
                    c.*
	INTO			
					#tmpDataCriterionGroups
    FROM
                    ncBtTESTCriterionGroup c
    WHERE
                    c.SegmentId IN (SELECT Id FROM @ncBtTESTIdList)

	-- Fetch criterion groups from temp table
	SELECT
					* 
	FROM
					#tmpDataCriterionGroups

	-- Fetch criterion group ids from temp table as a new id list
	DECLARE @CriterionncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO
					@CriterionncBtTESTIdList
		SELECT DISTINCT
					Id
		FROM
					#tmpDataCriterionGroups
	
	-- Fetch criterions using stored procedure
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStpNcBtTest]
					@ncBtTESTIdList = @CriterionncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetByIdStpNcBtTest] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGroupGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [CriterionGroupGetBySegmentIdStpNcBtTest] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetAllStpNcBtTest] 

AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch segment as temp table
    SELECT
                    s.*
	INTO			
					#tmpDataSegment
    FROM
                    ncBtTESTSegment s

	-- Fetch segment from temp table
	SELECT
					* 
	FROM
					#tmpDataSegment

	-- Fetch segment ids from temp table as a new id list
	DECLARE @SegmentncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO
					@SegmentncBtTESTIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataSegment

	-- Fetch criterion groups using stored procedure
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStpNcBtTest]
					@ncBtTESTIdList = @SegmentncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByIdsStpNcBtTest] 
	@ncBtTESTIdList AS [dbo].[ncBtTESTIdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Fetch segment as temp table
    SELECT
                    s.*
	INTO			
					#tmpDataSegment
    FROM
                    ncBtTESTSegment s
    WHERE
                    s.Id IN (SELECT Id FROM @ncBtTESTIdList)

	-- Fetch segment from temp table
	SELECT
					* 
	FROM
					#tmpDataSegment

	-- Fetch segment ids from temp table as a new id list
	DECLARE @SegmentncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO
					@SegmentncBtTESTIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataSegment

	-- Fetch criterion groups using stored procedure
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStpNcBtTest]
					@ncBtTESTIdList = @SegmentncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByIdStpNcBtTest] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList (Id) VALUES (@Id)
	EXEC			[dbo].[SegmentGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByAliasMultipleStpNcBtTest] 
	@ncBtTESTAliasList AS [dbo].[ncBtTESTAliasList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTIdList AS [dbo].[ncBtTESTIdList];
	INSERT INTO @ncBtTESTIdList SELECT Id FROM ncBtTESTSegment WHERE Alias IN (SELECT * FROM @ncBtTESTAliasList)
	EXEC			[dbo].[SegmentGetByIdsStpNcBtTest]
					@ncBtTESTIdList = @ncBtTESTIdList
END
GO

-- =============================================
-- Author:		SLY
-- Create date: <2015-04-16>
-- =============================================
CREATE PROCEDURE [SegmentGetByAliasStpNcBtTest] 
	@Alias varchar(255)
AS
BEGIN
	SET NOCOUNT ON;

	-- Convert to list and call stored procedure for lists
	DECLARE @ncBtTESTAliasList AS [dbo].[ncBtTESTAliasList];
	INSERT INTO @ncBtTESTAliasList (Alias) VALUES (@Alias)
	EXEC			[dbo].[SegmentGetByAliasMultipleStpNcBtTest]
					@ncBtTESTAliasList = @ncBtTESTAliasList
END
GO