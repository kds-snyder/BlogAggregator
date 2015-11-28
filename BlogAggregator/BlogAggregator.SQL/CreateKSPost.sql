use BlogAggregator;
GO

INSERT INTO Posts
(BlogID, Link, PublicationDate, Title, [Description], Content) 
VALUES (1, 
		'https://kdssnyder.wordpress.com/2015/11/21/ninth-week-in-coding-bootcamp-continuing-group-project/',
		'11/21/2015 3:51:35 PM', 1, 
		'Ninth Week in Coding Bootcamp: Continuing Group Project', 
		'On November 16 I started the ninth week', 
		'');

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;