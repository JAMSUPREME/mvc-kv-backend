
TRUNCATE TABLE RootObject

--exp active
INSERT INTO RootObject(Id,[Description],ActiveFlag) VALUES ('DE3AA882-CC88-456A-A2CB-155D477D00A2','main obj for a person',1)
--date range active
INSERT INTO RootObject(Id,[Description],[OfferStartDate],[OfferEndDate]) VALUES ('86C3B9D1-5269-4D8B-BA99-4F7B3CEB371D','main obj for a person',GETDATE()-10,GETDATE()+10)
--date range inactive
INSERT INTO RootObject(Id,[Description],[OfferStartDate],[OfferEndDate]) VALUES ('D43D86C5-3335-4E41-95A4-539CBE462780','main obj for a person',GETDATE()+10,GETDATE()+11)

--SELECT NEWID()