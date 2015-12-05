use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorName = 'Karen Deborah Snyder');

DELETE FROM Blogs
WHERE AuthorName = 'Karen Deborah Snyder';

SELECT * FROM Blogs;
SELECT * FROM Posts;