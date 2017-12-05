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