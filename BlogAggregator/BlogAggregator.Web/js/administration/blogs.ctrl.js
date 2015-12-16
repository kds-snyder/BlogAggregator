angular.module('app').controller('AdminBlogsController', function (Blog, $mdToast, $scope) {

    // Function to load blogs, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        console.log('Loading set to true');
        $scope.blogs = Blog.query(function () {
            $scope.loading = false;
            console.log('Loading set to false');
        });
        console.log('Finished blog query');
    };

    $scope.approveBlog = function (blog) {
        $scope.approvingBlog = true;
        blog.Approved = true;
        blog.$update(function () {
            $scope.approvingBlog = false;
            $mdToast.show($mdToast.simple().content('Blog approved successfully')
                             .position('top left').theme("toast-success"));
        },
        function (error) {
            $scope.approvingBlog = false;
            $mdToast.show($mdToast.simple()
                           .content('Unable to approve blog')
                           .position('top left').theme("toast-error"));
            blog.Approved = false;
        });
    };

    $scope.deleteBlog = function (blog) {
        /*
        $mdToast.show($mdToast.simple()
                           .content('Are you sure you want to delete this blog?')
                           .position('top left').theme("toast-error"))
                           .action('')
                           .highlightAction(true)
                           .action('Cancel')
          .highlightAction(false)
*/
        if (confirm('Are you sure you want to delete this blog?')) {
            Blog.delete({ id: blog.BlogID }, function (data) {
                var index = $scope.blogs.indexOf(blog);
                $scope.blogs.splice(index, 1);
                $mdToast.show($mdToast.simple().content('Blog deleted successfully')
                             .position('top left').theme("toast-success"));
            },
            function (error) {
                  $mdToast.show($mdToast.simple()
                           .content('Unable to delete blog')
                           .position('top left').theme("toast-error"));           
            });
        }
    };

    $scope.rejectBlog = function (blog) {
        blog.Approved = false;
        $scope.rejectingBlog = true;
        blog.$update(function () {
            $scope.rejectingBlog = false;
            $mdToast.show($mdToast.simple().content('Blog rejected successfully')
                             .position('top left').theme("toast-success"));
        },
        function (error) {
            $scope.rejectingBlog = false;
            $mdToast.show($mdToast.simple()
                           .content('Unable to reject blog')
                           .position('top left').theme("toast-error"));
            blog.Approved = true;
        });
    };

    // After all definitions, load the blogs
    $scope.load();
});