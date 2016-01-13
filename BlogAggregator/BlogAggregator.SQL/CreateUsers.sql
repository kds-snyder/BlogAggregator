use BlogAggregator;
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
		'testuser');

SELECT * FROM Users;