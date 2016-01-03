use BlogAggregator;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Adriann Bracken');

UPDATE Blogs
SET Approved = 0
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Adriann Bracken');

SELECT * FROM Blogs;
SELECT * FROM Posts;