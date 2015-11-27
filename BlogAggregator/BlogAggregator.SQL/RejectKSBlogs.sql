use BlogAggregator;
GO

UPDATE Blogs
SET Approved = 0
WHERE AuthorID = (SELECT AuthorID FROM Authors Where Name = 'Karen Snyder');

SELECT * FROM Blogs;