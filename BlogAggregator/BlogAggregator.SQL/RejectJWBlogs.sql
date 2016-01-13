use BlogAggregator;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Jeff Ward');

UPDATE Blogs
SET Approved = 0
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Jeff Ward');

SELECT * FROM Blogs;
SELECT * FROM Posts;