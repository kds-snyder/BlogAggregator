use BlogAggregator;
GO

--INSERT INTO Blogs
--(CreatedDate, Approved, AuthorName, AuthorEmail, Description, Link, Title) 
--VALUES ('11/26/2015 12:40', 0, 
--		'Karen Snyder',
--		'a@b.com',
--		'Return to development via Origin Code Academy', 
--		'https://kdssnyder.wordpress.com', 'Karen Snyder');

INSERT INTO Blogs
( CreatedDate, Approved, AuthorName, AuthorEmail, Description, Link, Title) 
VALUES ('11/28/2015 19:40', 0,
		'Adriann Bracken',
		'c@d.com',
		'My journey to becoming a web developer', 
		'http://bracken.design', 
		'Bracken Design');

SELECT * FROM Blogs;