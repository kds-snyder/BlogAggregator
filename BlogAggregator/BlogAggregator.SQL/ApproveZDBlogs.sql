use BlogAggregator;
GO

UPDATE Blogs
SET Approved = 1
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Zak Dietzen');

SELECT * FROM Blogs;
SELECT * FROM Posts;