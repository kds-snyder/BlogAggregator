use BlogAggregator;
GO

DELETE FROM [Posts]
WHERE BlogID = (SELECT BlogID FROM Blogs
				WHERE AuthorID = (SELECT AuthorID FROM Authors WHERE Name = 'Adriann Bracken'));

DELETE FROM Blogs
WHERE AuthorID = (SELECT AuthorID FROM Authors WHERE Name = 'Adriann Bracken');

DELETE FROM Authors
WHERE Name = 'Adriann Bracken';

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;