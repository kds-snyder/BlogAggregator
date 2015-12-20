use BlogAggregator_oauth;
GO

INSERT INTO Users
( Authorized, SecurityStamp, UserName) 
VALUES (0, 'XXXXX', 'tovahks@gmail.com');

INSERT INTO Users
( Authorized, SecurityStamp, UserName) 
VALUES (0, 'YYYY', 'kds@gmail.com');

INSERT INTO ExternalLogins
( UserID, LoginProvider, ProviderKey)
VALUES (1, 'Google', '114142399122115436007');

INSERT INTO ExternalLogins
( UserID, LoginProvider, ProviderKey)
VALUES (2, 'Google', '224142399121125436007');

SELECT * FROM Users;
SELECT * FROM ExternalLogins;