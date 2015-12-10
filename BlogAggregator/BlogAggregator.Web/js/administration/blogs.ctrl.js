angular.module('app').controller('AdminBlogsController', function (Blog, $mdToast, $scope) {

    // Function to load posts, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        $scope.blogs = Blog.query(function () {
            $scope.loading = false;
        });
    };

    $scope.approveBlog = function (blog) {
        blog.Approved = true;
        blog.$update(function () {
            $mdToast.show($mdToast.simple().content('Blog approved successfully')
                             .position('top left').theme("toast-success"));
        },
        function (error) {
            $mdToast.show($mdToast.simple()
                           .content('Unable to approve blog')
                           .position('top left').theme("toast-error"));
            blog.Approved = false;
        });
    };

    $scope.deleteBlog = function (blog) {
        if (confirm('Are you sure you want to delete this blog?')) {
            Blog.delete({ id: blog.BlogID }, function (data) {
                var index = $scope.blogs.indexOf(blog);
                $scope.blogs.splice(index, 1);
                $mdToast.show($mdToast.simple().content('Blog deleted successfully')
                             .position('top left').theme("toast-success"));
            },
            function (error) {
                debugger;
                 $mdToast.show($mdToast.simple()
                           .content('Unable to delete blog')
                           .position('top left').theme("toast-error"));           
            });
        }
    };

    $scope.rejectBlog = function (blog) {
        blog.Approved = false;
        blog.$update(function () {
            $mdToast.show($mdToast.simple().content('Blog rejected successfully')
                             .position('top left').theme("toast-success"));
        },
        function (error) {
            $mdToast.show($mdToast.simple()
                           .content('Unable to reject blog')
                           .position('top left').theme("toast-error"));
            blog.Approved = true;
        });
    };

    // After all definitions, load the blogs
    $scope.load();
});