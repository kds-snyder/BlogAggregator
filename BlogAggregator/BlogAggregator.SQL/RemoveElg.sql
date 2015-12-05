use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorName = 'Eleganto');

DELETE FROM Blogs
WHERE AuthorName = 'Eleganto';

SELECT * FROM Blogs;
SELECT * FROM Posts;