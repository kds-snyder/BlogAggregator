angular.module('app').controller('AddBlogController', function (Blog, $mdToast, $scope) {
  
    $scope.blog = new Blog();
    $scope.loading = false;

    $scope.addBlog = function () {

        // Set blog type to Word Press
        $scope.blog.BlogType = 1;

        // TODO: remove when blog moderation is ready
        $scope.blog.Approved = true;

        // Set loading indicator and save the new blog
        $scope.loading = true;
        $scope.blog.$save(function () {
            
            // Blog saved successfully: 
            //  Clear loading indicator and display success message
            $scope.loading = false;
            $mdToast.show($mdToast.simple().content('Blog was added successfully')
                            .position('top center').theme("toast-success"));

            // Clear the blog object and input form
            $scope.clearInputBlog();

        },
        function (error) {
            // Blog could not be saved: 
            //  Clear loading indicator and display error message
            $scope.loading = false;
            $mdToast.show($mdToast.simple()
                           .content('Error adding blog: Please verify that you entered a link to a valid blog')
                           .position('top right').theme("toast-error"));
        });
    };

    // Clear blog object and input form
    $scope.clearInputBlog = function () {
        $scope.blog.BlogID = 0;
        $scope.blog.BlogType = 0;
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