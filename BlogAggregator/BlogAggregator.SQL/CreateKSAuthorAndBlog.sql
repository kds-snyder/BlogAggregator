use BlogAggregator;
GO

INSERT INTO Authors
(BlogID, CreatedDate, Email, Name) 
VALUES (1, '11/26/2015 12:40', 'a@b.com', 'Karen Snyder');

--INSERT INTO Authors
--(BlogID, CreatedDate, Email, Name) 
--VALUES (2, '11/26/2015 12:42', 'c@d.com', 'Adriann Bracken');

--INSERT INTO Authors
--(BlogID, CreatedDate, Email, Name) 
--VALUES (3, '11/26/2015 12:44', 'e@f.com', 'Zak Dietzen');
--GO

INSERT INTO Blogs
(AuthorID, CreatedDate, Approved, Description, Link, Title) 
VALUES (1, '11/26/2015 12:40', 1, 
		'Return to development via Origin Code Academy', 
		'https://kdssnyder.wordpress.com', 'Karen Snyder');

SELECT * FROM Authors;
SELECT * FROM Blogs;