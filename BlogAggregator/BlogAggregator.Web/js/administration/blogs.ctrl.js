angular.module('app').controller('AdminBlogsController', function ($scope, Blog) {
   
    // Function to load posts, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        $scope.blogs = Blog.query(function () {
            $scope.loading = false;
        });
    };

    $scope.approveBlog = function (blog) {
        alert('Approve');
    };

    $scope.deleteBlog = function (blog) {
        alert('Delete');
    };

    $scope.rejectBlog = function (blog) {
        alert('Reject');
    };

    // After all definitions, load the posts
    $scope.load();
});