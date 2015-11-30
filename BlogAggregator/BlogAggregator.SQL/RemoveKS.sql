use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorName = 'Karen D. Snyder');

DELETE FROM Blogs
WHERE AuthorName = 'Karen D. Snyder';

SELECT * FROM Blogs;
SELECT * FROM Posts;