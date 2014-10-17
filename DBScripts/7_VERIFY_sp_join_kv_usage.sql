

use Stuff

DECLARE @where varchar(max)
DECLARE @select varchar(max)
DECLARE @pageStart int
DECLARE @pageEnd int
DECLARE @customSort nvarchar(max) 

SET @where = 'AND [OfferStartDate] LIKE ''%ou%'''
SET @select = 'OfferStartDate,OfferEndDate,ActiveFlag,IsActive,[Description]'

--SET @pageStart = 1
--SET @pageEnd = 1

--SET @customSort = 'Id'

--important that params are named if not passing all
EXEC sp_getKVPairs @whereClause=@where,@selectClause=@select

--SELECT * FROM KvPairTable
