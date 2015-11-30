use BlogAggregator;
GO

UPDATE Blogs
SET Approved = 0
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Karen Snyder');

SELECT * FROM Blogs;
SELECT * FROM Posts;