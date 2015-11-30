use BlogAggregator;
GO

SELECT * FROM Authors
WHERE AuthorID = (SELECT AuthorID FROM Blogs WHERE BlogID =
 (SELECT BlogID FROM Posts WHERE PostID = 2));

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;