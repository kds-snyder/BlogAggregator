use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorName = 'Adriann Bracken');

DELETE FROM Blogs
WHERE AuthorName = 'Adriann Bracken';

SELECT * FROM Blogs;
SELECT * FROM Posts;