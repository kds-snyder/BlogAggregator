use BlogAggregator;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Karen D. Snyder');

SELECT * FROM Blogs;
SELECT * FROM Posts;