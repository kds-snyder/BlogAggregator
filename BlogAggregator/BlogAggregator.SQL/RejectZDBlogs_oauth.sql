use BlogAggregator_oauth;
GO

DELETE FROM Posts WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Zak Dietzen');

UPDATE Blogs
SET Approved = 0
WHERE BlogID = (SELECT BlogID FROM Blogs Where AuthorName = 'Zak Dietzen');

SELECT * FROM Blogs;
SELECT * FROM Posts;