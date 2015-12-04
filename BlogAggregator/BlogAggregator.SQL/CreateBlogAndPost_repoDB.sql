use BlogAggregator_repo;
GO

INSERT INTO Blogs
( CreatedDate, BlogType, Approved, AuthorName, AuthorEmail, Description, Link, Title) 
VALUES ('11/28/2015 19:40', 1, 1,
		'Adriann Bracken',
		'c@d.com',
		'My journey to becoming a web developer', 
		'http://bracken.design', 
		'Bracken Design');

INSERT INTO Posts
(BlogID, Link, PublicationDate, Title, [Description], Content) 
VALUES (2, 
		'http://bracken.design/2015/10/19/coding-with-girls/',
		'10/18/2015 7:01:18 PM', 
		'Coding with girls', 
		'',
		'<div style="text-align: center;">
<p><iframe width="640" height="360" src="https://www.youtube.com/embed/H5Vzo-iPGCo?feature=oembed" frameborder="0" allowfullscreen></iframe></p>
</div>');

SELECT * FROM Blogs;
SELECT * FROM Posts;