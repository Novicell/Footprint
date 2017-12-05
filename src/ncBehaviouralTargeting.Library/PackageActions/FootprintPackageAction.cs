using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Umbraco.Core;
using umbraco.interfaces;
using System.Xml;
using System.IO;
using System.Web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.packager.standardPackageActions;


namespace ncBehaviouralTargeting.Library.PackageActions
{
    public class FootprintPackageAction : IPackageAction
    {
        #region IPackageAction Members

        private string _packageName = "";

        public string Alias()
        {
            return "FootprintPackageAction";
        }

        /// <summary>
        /// Executes the sql action
        /// </summary>
        public bool Execute(string packageName, XmlNode xmlData)
        {
            _packageName = packageName;

            // Here is comes
            string sqlWithGos = @"
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ncBtAction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[SegmentId] [int] NOT NULL,
	[ActionType] [int] NOT NULL,
	[EmailPropertyId] [int] NULL,
	[EmailSubject] [nvarchar](255) NULL,
	[EmailNodeId] [int] NULL,
 CONSTRAINT [PK_ncBtAction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TYPE [dbo].[IdList] AS TABLE(
	[Id] [int] NULL
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ncBtSegment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[Persistence] [int] NOT NULL,
 CONSTRAINT [PK_ncBtSegment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ncBtProperty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Alias] [nvarchar](255) NOT NULL,
	[DisplayName] [nvarchar](255) NOT NULL,
	[DataType] [int] NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Examples] [nvarchar](1000) NULL,
	[IsArray] [bit] NOT NULL,
 CONSTRAINT [PK_ncBtProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ncBtOperator](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](255) NULL,
	[IsInverted] [bit] NOT NULL,
	[OperatorType] [int] NOT NULL,
	[DataType] [int] NOT NULL,
 CONSTRAINT [PK_ncBtOperator] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TYPE [dbo].[AliasList] AS TABLE(
	[Alias] [varchar](255) NULL
)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[OperatorGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    o.*
	INTO			
					#tmpDataOperators
    FROM
                    ncBtOperator o
    WHERE
                    o.Id IN (SELECT Id FROM @IdList)

	SELECT
					* 
	FROM
					#tmpDataOperators
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ncBtCriterionGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SegmentId] [int] NOT NULL,
	[IsInclude] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_CriteriaGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ncBtCriterion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CriterionGroupId] [int] NOT NULL,
	[OperatorId] [int] NOT NULL,
	[PropertyAlias] [nvarchar](255) NOT NULL,
	[PropertyValue] [nvarchar](255) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ncBtCriterion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[OperatorGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[OperatorGetByIdsStp]
					@IdList = @IdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGetByCriterionGroupIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    c.*
	INTO			
					#tmpDataCriterions 
    FROM
                    ncBtCriterion c
    WHERE
                    c.CriterionGroupId IN (SELECT Id FROM @IdList)

	SELECT
					* 
	FROM
					#tmpDataCriterions

	DECLARE @OperatorIdList AS [dbo].[IdList];
	INSERT INTO
					@OperatorIdList
		SELECT DISTINCT
					OperatorId AS Id
		FROM
					#tmpDataCriterions
	
	EXEC			[dbo].[OperatorGetByIdsStp]
					@IdList = @OperatorIdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    c.*
	INTO			
					#tmpDataCriterions 
    FROM
                    ncBtCriterion c
    WHERE
                    c.Id IN (SELECT Id FROM @IdList)

	SELECT
					* 
	FROM
					#tmpDataCriterions

	DECLARE @OperatorIdList AS [dbo].[IdList];
	INSERT INTO
					@OperatorIdList
		SELECT DISTINCT
					OperatorId AS Id
		FROM
					#tmpDataCriterions
	
	EXEC			[dbo].[OperatorGetByIdsStp]
					@IdList = @OperatorIdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGroupGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    cg.*
	INTO			
					#tmpDataCriterionGroup
    FROM
                    ncBtCriterionGroup cg
    WHERE
                    cg.Id IN (SELECT Id FROM @IdList)

	SELECT
					* 
	FROM
					#tmpDataCriterionGroup

	DECLARE @CriterionGroupIdList AS [dbo].[IdList];
	INSERT INTO
					@CriterionGroupIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataCriterionGroup
	
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStp]
					@IdList = @CriterionGroupIdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGetByIdsStp]
					@IdList = @IdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGetByCriterionGroupIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStp]
					@IdList = @IdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGroupGetBySegmentIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    c.*
	INTO			
					#tmpDataCriterionGroups
    FROM
                    ncBtCriterionGroup c
    WHERE
                    c.SegmentId IN (SELECT Id FROM @IdList)

	SELECT
					* 
	FROM
					#tmpDataCriterionGroups

	DECLARE @CriterionIdList AS [dbo].[IdList];
	INSERT INTO
					@CriterionIdList
		SELECT DISTINCT
					Id
		FROM
					#tmpDataCriterionGroups
	
	EXEC			[dbo].[CriterionGetByCriterionGroupIdsStp]
					@IdList = @CriterionIdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGroupGetBySegmentIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStp]
					@IdList = @IdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CriterionGroupGetByIdStp] 
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SegmentGetAllStp] 

AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    s.*
	INTO			
					#tmpDataSegment
    FROM
                    ncBtSegment s

	SELECT
					* 
	FROM
					#tmpDataSegment

	DECLARE @SegmentIdList AS [dbo].[IdList];
	INSERT INTO
					@SegmentIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataSegment

	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStp]
					@IdList = @SegmentIdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SegmentGetByIdsStp] 
	@IdList AS [dbo].[IdList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

    SELECT
                    s.*
	INTO			
					#tmpDataSegment
    FROM
                    ncBtSegment s
    WHERE
                    s.Id IN (SELECT Id FROM @IdList)

	SELECT
					* 
	FROM
					#tmpDataSegment

	DECLARE @SegmentIdList AS [dbo].[IdList];
	INSERT INTO
					@SegmentIdList
		SELECT DISTINCT
					Id AS Id
		FROM
					#tmpDataSegment

	EXEC			[dbo].[CriterionGroupGetBySegmentIdsStp]
					@IdList = @SegmentIdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SegmentGetByIdStp] 
	@Id int = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList (Id) VALUES (@Id)
	EXEC			[dbo].[SegmentGetByIdsStp]
					@IdList = @IdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SegmentGetByAliasMultipleStp] 
	@AliasList AS [dbo].[AliasList] READONLY
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @IdList AS [dbo].[IdList];
	INSERT INTO @IdList SELECT Id FROM ncBtSegment WHERE Alias IN (SELECT * FROM @AliasList)
	EXEC			[dbo].[SegmentGetByIdsStp]
					@IdList = @IdList
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SegmentGetByAliasStp] 
	@Alias varchar(255)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @AliasList AS [dbo].[AliasList];
	INSERT INTO @AliasList (Alias) VALUES (@Alias)
	EXEC			[dbo].[SegmentGetByAliasMultipleStp]
					@AliasList = @AliasList
END
GO

ALTER TABLE [dbo].[ncBtAction] ADD  CONSTRAINT [DF_ncBtAction_ActionType]  DEFAULT ((0)) FOR [ActionType]
GO

ALTER TABLE [dbo].[ncBtProperty] ADD  CONSTRAINT [DF_ncBtProperty_DataType]  DEFAULT ((5)) FOR [DataType]
GO

ALTER TABLE [dbo].[ncBtProperty] ADD  CONSTRAINT [DF_ncBtProperty_IsArray]  DEFAULT ((0)) FOR [IsArray]
GO

ALTER TABLE [dbo].[ncBtSegment] ADD  CONSTRAINT [DF_ncBtSegment_Persistence]  DEFAULT ((0)) FOR [Persistence]
GO

ALTER TABLE [dbo].[ncBtCriterion]  WITH NOCHECK ADD  CONSTRAINT [FK_ncBtCriterion_ncBtCriterionGroup] FOREIGN KEY([CriterionGroupId])
REFERENCES [dbo].[ncBtCriterionGroup] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ncBtCriterion] CHECK CONSTRAINT [FK_ncBtCriterion_ncBtCriterionGroup]
GO

ALTER TABLE [dbo].[ncBtCriterion]  WITH NOCHECK ADD  CONSTRAINT [FK_ncBtCriterion_ncBtOperator] FOREIGN KEY([OperatorId])
REFERENCES [dbo].[ncBtOperator] ([Id])
GO
ALTER TABLE [dbo].[ncBtCriterion] CHECK CONSTRAINT [FK_ncBtCriterion_ncBtOperator]
GO

ALTER TABLE [dbo].[ncBtCriterionGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_ncBtCriterionGroup_ncBtSegment] FOREIGN KEY([SegmentId])
REFERENCES [dbo].[ncBtSegment] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ncBtCriterionGroup] CHECK CONSTRAINT [FK_ncBtCriterionGroup_ncBtSegment]
GO


INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.browser', N'Browser', 5, N'Browser used by the visitor', N'Chrome, Firefox and Safari', 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.userAgent', N'User agent', 5, N'User agent of the visitor''s browser', N'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36', 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.isMobile', N'Is mobile device', 4, N'Does the visitor visit your website from a mobile device?', N'true and false', 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.mobileDeviceManufacturer', N'Mobile device manufacturer', 5, N'Manufacturer of the visitors device (only applies for mobile devices)', N'Samsung, Apple and Nokia', 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.mobileDeviceModel', N'Mobile device model', 5, N'Model of the visitors device (only applies for mobile devices)', N'IPhone', 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.userIp', N'IP address', 5, N'IP address of the visitor', N'69.89.31.226', 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.queryString', N'Query string', 5, N'Query string for current pageview', NULL, 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.pageId', N'Visited page', 1, N'Page visited by the visitor', NULL, 0)
INSERT INTO [dbo].[ncBtProperty] ([Alias], [DisplayName], [DataType], [Description], [Examples], [IsArray]) VALUES (N'ncbt.httpReferrer', N'Referring url', 5, N'URL of site/service that linked to you', N'Match banana if you want to target hits from novicell.dk/banana', 0)

INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'contains', 0, 4, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is equal to', 0, 1, 1)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is not equal to', 1, 1, 1)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'starts with', 0, 5, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is greater than', 0, 3, 1)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'ends with', 0, 6, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is less than', 0, 2, 1)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is equal to', 0, 1, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'doesn''t contain', 1, 4, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'does not start with', 1, 5, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'does not end with', 1, 6, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is not equal to', 1, 1, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is equal to', 0, 1, 3)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is not equal to', 0, 1, 3)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is greater than', 0, 3, 3)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is less than', 0, 2, 3)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is less than or equal to', 1, 3, 1)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is greater than or equal to', 1, 2, 1)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is', 0, 1, 6)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is before', 0, 2, 6)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is after', 0, 3, 6)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is matching regex', 0, 7, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is not matching regex', 1, 7, 5)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'visited exactly', 0, 1, 7)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'visited more than', 0, 3, 7)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'visited less than', 0, 2, 7)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is equal to', 0, 1, 2)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is not equal to', 1, 1, 2)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is greater than', 0, 3, 2)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is less than', 0, 2, 2)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is less than or equal to', 1, 3, 2)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is greater than or equal to', 1, 2, 2)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is less than or equal to', 1, 3, 3)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is greater than or equal to', 1, 2, 3)
INSERT INTO [dbo].[ncBtOperator] ([DisplayName], [IsInverted], [OperatorType], [DataType]) VALUES (N'is', 0, 1, 4)
";
            return ExecuteSql(sqlWithGos);
        }

        public System.Xml.XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"false\" alias=\"" + Alias() + "\"></Action>";
            return helper.parseStringToXmlNode(sample);
        }

        
        /// <summary>
        /// When you want to Undo an Sql Install create a new action that only runs at UnInstall 
        /// </summary>
        public bool Undo(string packageName, System.Xml.XmlNode xmlData)
        {
            _packageName = packageName;

            return true;

            var sqlWithGos = @"
DROP TABLE [ncBtOperator];
DROP TABLE [ncBtProperty];
DROP TABLE [ncBtCriterion];
DROP TABLE [ncBtCriterionGroup];
DROP TABLE [ncBtSegment];
DROP TABLE [ncBtAction];
GO

DROP PROCEDURE [OperatorGetByIdsStp];
DROP PROCEDURE [OperatorGetByIdStp];
DROP PROCEDURE [CriterionGetByCriterionGroupIdsStp];
DROP PROCEDURE [CriterionGetByIdsStp];
DROP PROCEDURE [CriterionGroupGetByIdsStp];
DROP PROCEDURE [CriterionGetByIdStp];
DROP PROCEDURE [CriterionGetByCriterionGroupIdStp];
DROP PROCEDURE [CriterionGroupGetBySegmentIdsStp];
DROP PROCEDURE [CriterionGroupGetByIdStp];
DROP PROCEDURE [CriterionGroupGetBySegmentIdStp];
DROP PROCEDURE [SegmentGetAllStp];
DROP PROCEDURE [SegmentGetByIdsStp];
DROP PROCEDURE [SegmentGetByIdStp];
DROP PROCEDURE [SegmentGetByAliasMultipleStp];
DROP PROCEDURE [SegmentGetByAliasStp];
GO

DROP TYPE [AliasList];
DROP TYPE [IdList];
GO
";
            return ExecuteSql(sqlWithGos);
        }


        #endregion

        private bool ExecuteSql(string sql)
        {
            bool result = false;
            try
            {
                var db = ApplicationContext.Current.DatabaseContext.Database;
                var enableNamedParamsSetting = db.EnableNamedParams;
                db.EnableNamedParams = false;
                foreach (var s in SplitSqlStatements(sql))
                {
                    db.Execute(s);
                }
                db.EnableNamedParams = enableNamedParamsSetting;

                //Everything okay return true
                result = true;
            }
            catch (Exception ex)
            {
                Log.Add(LogTypes.Error, -1, string.Format("Error in Execute SQL Package action for package {0} error:{1} ", _packageName, ex.ToString()));
            }
            return result;
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^\s*GO\s* ($ | \-\- .*$)",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }
    }
}
