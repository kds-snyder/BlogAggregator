use BlogAggregator;
GO

--INSERT INTO Authors
--(CreatedDate, Email, Name) 
--VALUES ('11/26/2015 12:40', 'a@b.com', 'Karen Snyder');

INSERT INTO Authors
(CreatedDate, Email, Name) 
VALUES ('11/28/2015 19:42', 'c@d.com', 'Adriann Bracken');

--INSERT INTO Authors
--(CreatedDate, Email, Name) 
--VALUES ('11/26/2015 12:44', 'e@f.com', 'Zak Dietzen');
--GO

--INSERT INTO Blogs
--(AuthorID, CreatedDate, Approved, Description, Link, Title) 
--VALUES (1, '11/26/2015 12:40', 0, 
--		'Return to development via Origin Code Academy', 
--		'https://kdssnyder.wordpress.com', 'Karen Snyder');

INSERT INTO Blogs
(AuthorID, CreatedDate, Approved, Description, Link, Title) 
VALUES (2, '11/28/2015 19:40', 0, 
		'My journey to becoming a web developer', 
		'http://bracken.design', 
		'Bracken Design');

SELECT * FROM Authors;
SELECT * FROM Blogs;