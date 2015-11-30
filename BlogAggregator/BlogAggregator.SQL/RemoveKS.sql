use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorID = (SELECT AuthorID FROM Authors WHERE Name = 'Karen Snyder'));

DELETE FROM Blogs
WHERE AuthorID = (SELECT AuthorID FROM Authors WHERE Name = 'Karen Snyder');

DELETE FROM Authors
WHERE Name = 'Karen Snyder';

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;