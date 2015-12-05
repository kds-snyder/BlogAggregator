use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorName = 'Zak Dietzen');

DELETE FROM Blogs
WHERE AuthorName = 'Zak Dietzen';

SELECT * FROM Blogs;
SELECT * FROM Posts;