

use Stuff

DECLARE @where varchar(max)
DECLARE @select varchar(max)
DECLARE @pageStart int
DECLARE @pageEnd int
DECLARE @customSort nvarchar(max) 

--SET @where = 'WHERE Id = ''' + '86C3B9D1-5269-4D8B-BA99-4F7B3CEB371D' + ''''
SET @select = 'OfferStartDate as OfferStartDate,OfferEndDate as OfferEndDate,ActiveFlag as ActiveFlag,IsActive as IsActive'

SET @pageStart = 1
SET @pageEnd = 1

--SET @customSort = 'Id'

--important that params are named if not passing all
EXEC sp_getKVPairs @pageStart=@pageStart,@pageEnd = @pageEnd,@customSort=@customSort