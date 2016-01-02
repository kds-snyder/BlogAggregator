use BlogAggregator_oauth;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Karen D. Snyder');

UPDATE Blogs
SET Approved = 0
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Karen D. Snyder');

SELECT * FROM Blogs;
SELECT * FROM Posts;