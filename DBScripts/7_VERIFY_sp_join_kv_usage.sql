

use Stuff

DECLARE @where varchar(max)
DECLARE @select varchar(max)

SET @where = 'WHERE Id = ''' + '86C3B9D1-5269-4D8B-BA99-4F7B3CEB371D' + ''''
SET @select = 'OfferStartDate,OfferEndDate,ActiveFlag,IsActive'

EXEC sp_getKVPairs @where, @select