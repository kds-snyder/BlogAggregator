 use BlogAggregator_oauth;
  GO

 INSERT INTO Clients
( Id, [Secret], Name, ApplicationType, Active, RefreshTokenLifeTime, AllowedOrigin) 
VALUES ('BlogAggregatorApp', '12345', 'BlogAggregator Front End', 0, 1, 7200, '*');

INSERT INTO Clients
( Id, [Secret], Name, ApplicationType, Active, RefreshTokenLifeTime, AllowedOrigin) 
VALUES ('consoleApp', '12345', 'Console application', 1, 1, 14400, '*');

SELECT * FROM Clients;