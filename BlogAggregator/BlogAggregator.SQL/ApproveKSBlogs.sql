use BlogAggregator;
GO

UPDATE Blogs
SET Approved = 1
WHERE AuthorID = (SELECT AuthorID FROM Authors Where Name = 'Karen Snyder');

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;