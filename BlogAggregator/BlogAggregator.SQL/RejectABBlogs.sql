use BlogAggregator;
GO

UPDATE Blogs
SET Approved = 0
WHERE AuthorID = (SELECT AuthorID FROM Authors Where Name = 'Adriann Bracken');

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;