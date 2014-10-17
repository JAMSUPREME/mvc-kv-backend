-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.sp_getKVPairs
	-- Add the parameters for the stored procedure here
	@whereClause varchar(max) = '',
	@selectClause varchar(max) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @selectClause = CASE WHEN @selectClause = '' THEN '' ELSE ',' + @selectClause END

	DECLARE @addlFields nvarchar(2000)

	DECLARE @sql AS nvarchar(max)
	DECLARE @pivot_list AS nvarchar(max) -- Leave NULL for COALESCE technique
	DECLARE @select_list AS nvarchar(max) -- Leave NULL for COALESCE technique

	SELECT @pivot_list = COALESCE(@pivot_list + ', ', '') + '[' + CONVERT(varchar, PIVOT_CODE) + ']'
    		,@select_list = COALESCE(@select_list + ', ', '') + '[' + CONVERT(varchar, PIVOT_CODE) + '] AS ''' + [Key] + ''''
	FROM (
		SELECT DISTINCT PIVOT_CODE, [Key]
		FROM (
    		SELECT [Key], ROW_NUMBER() OVER (PARTITION BY [Schema] ORDER BY [Key]) AS PIVOT_CODE
			FROM KvPairTable
			GROUP BY [Key],[Schema]
			--optional where clause to restrict expected pivot fields
		) AS rows
	) AS PIVOT_CODES

	--SELECT @pivot_list

	SET @sql = '
	;WITH p AS (
		SELECT RootObjectId, [Value], ROW_NUMBER() OVER (PARTITION BY RootObjectId ORDER BY [Key]) AS PIVOT_CODE
		FROM KvPairTable
		--optional where clause can be introduced here, dynamically or otherwise
	)
	SELECT r.Id' + @selectClause + ',' + @select_list + '
	FROM p
	PIVOT (
		MIN(Value)
		FOR PIVOT_CODE IN (
			' + @pivot_list + '
		)
	) AS pvt
	RIGHT OUTER JOIN RootObject r ON r.Id = pvt.RootObjectId
	'
	+ @whereClause

	
	EXEC sp_executesql @sql
END
GO

--DROP PROCEDURE sp_getKVPairs