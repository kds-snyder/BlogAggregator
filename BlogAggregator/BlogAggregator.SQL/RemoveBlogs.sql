use BlogAggregator;
GO

DELETE FROM Blogs
WHERE BlogID >=9;

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;