use BlogAggregator_oauth;
GO

INSERT INTO Users
( Authorized, PasswordHash, SecurityStamp, UserName) 
VALUES (0,
		'XXXXXX',
		'XXXXX',
		'testerson');

INSERT INTO Users
( Authorized, PasswordHash, SecurityStamp, UserName) 
VALUES (0,
		'XXXXXX',
		'XXXXX',
		'testerson');

SELECT * FROM Users;