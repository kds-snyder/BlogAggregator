use BlogAggregator;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Karen Deborah Snyder');

UPDATE Blogs
SET Approved = 0
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Karen Deborah Snyder');

SELECT * FROM Blogs;
SELECT * FROM Posts;