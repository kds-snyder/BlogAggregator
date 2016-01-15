use BlogAggregator;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Zak Dietzen');

SELECT * FROM Blogs;
SELECT * FROM Posts;