angular.module('app').controller('PostsController', function ($scope, Post, PostService) {

    // Function to load posts, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        PostService.getAllPostsByDateDesc().then(
            function (data) {
            // Set posts if function returned data
            if (data.length > 0) {
                $scope.posts = data;
                $scope.loading = false;
            }
            else {
                $scope.posts = null;
                $scope.loading = false;
            }
        });
        
    };
 
    
    // After all definitions, load the posts
    $scope.load();

});