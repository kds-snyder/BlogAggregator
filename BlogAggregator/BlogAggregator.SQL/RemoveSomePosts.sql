use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE PostID=99;

DELETE FROM [Posts]
WHERE PostID=100;

DELETE FROM [Posts]
WHERE PostID=101;

DELETE FROM [Posts]
WHERE PostID=104;

DELETE FROM [Posts]
WHERE PostID=107;

DELETE FROM [Posts]
WHERE PostID=109;

DELETE FROM [Posts]
WHERE PostID=110;

DELETE FROM [Posts]
WHERE PostID=113;

SELECT * FROM Blogs;
SELECT * FROM Posts;