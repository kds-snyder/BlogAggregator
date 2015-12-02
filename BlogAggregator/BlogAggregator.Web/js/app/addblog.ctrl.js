angular.module('app').controller('AddBlogController', function (Blog, $mdToast, $scope) {
  
    $scope.blog = new Blog();

    $scope.addBlog = function () {

        // TODO: remove when blog moderation is ready
        $scope.blog.Approved = true;

        // Save the new blog in the database
        $scope.blog.$save(function () {
 
            $mdToast.show($mdToast.simple().content('Blog was added successfully')
                            .position('top right').theme("toast-success"));

            // Clear the blog object and input form
            $scope.clearInputBlog();

        },
        function (error) {
            $mdToast.show($mdToast.simple()
                           .content('Error adding blog: Please verify that you entered a link to a valid blog')
                           .position('top right').theme("toast-error"));
        });
    };

    // Clear blog object and input form
    $scope.clearInputBlog = function () {
        $scope.blog.BlogID = 0;
        $scope.blog.Approved = false;
        $scope.blog.AuthorEmail = '';
        $scope.blog.AuthorName = '';
        $scope.blog.Description = '';
        $scope.blog.Link = '';
        $scope.blog.Title = '';
        $scope.addBlogForm.$setPristine();
        $scope.addBlogForm.$setUntouched();
    };
});