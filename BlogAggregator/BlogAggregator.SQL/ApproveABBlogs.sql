use BlogAggregator;
GO

UPDATE Blogs
SET Approved = 1
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Adriann Bracken');

SELECT * FROM Blogs;
SELECT * FROM Posts;