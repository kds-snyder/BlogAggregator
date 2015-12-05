angular.module('app').controller('PostsController', function ($scope, Post, PostService) {

    // Function to load posts, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        $scope.posts = Post.query(function () {
            $scope.loading = false;          
        });
    };

   // $scope.posts = PostService.getAllPostsByDateDesc();
    
    // After all definitions, load the posts
    $scope.load();
    
});