use BlogAggregator;
GO

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

SELECT * FROM Authors;
SELECT * FROM Blogs;
SELECT * FROM Posts;