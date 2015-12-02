angular.module('app').controller('AddBlogController', function (Blog, $mdToast, $scope) {
  
    $scope.blog = new Blog();

    /*
    alert('After new: Blog ID: ' + $scope.blog.BlogID + ' CreatedDate: ' + $scope.blog.CreatedDate +
            ' Approved: ' + $scope.blog.Approved + 
            ' Description: ' + $scope.blog.Description + ' Link: ' + $scope.blog.Link +
            ' Title: ' + $scope.blog.Title + ' Author Email: ' + $scope.blog.AuthorEmail
            + ' Author Name: ' + $scope.blog.AuthorName);
    */

    $scope.addBlog = function () {

        // TODO: remove when blog moderation is ready
        $scope.blog.Approved = true;

        // Save the new blog in the database
        $scope.blog.$save(function () {
            $mdToast.showSimple('Blog was added successfully');

            // Clear the input form
            /*
            $scope.blog.BlogID = 0;
            $scope.blog.Approved = false;
            $scope.blog.authorEmail = '';
            $scope.blog.authorName = '';
            $scope.blog.Description = '';
            $scope.blog.Link = '';
            $scope.blog.Title = '';
            */
            $scope.blog = null;
            $scope.addBlogForm.$setPristine();
            $scope.addBlogForm.$setUntouched();

        },
        function (error) {
            $mdToast.showSimple('Error adding blog. Please verify that '
                        + $scope.blog.Link + ' is a link to a valid blog');

            debugger;
        });

    };
});