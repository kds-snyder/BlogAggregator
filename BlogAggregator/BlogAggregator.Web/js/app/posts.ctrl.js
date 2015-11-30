angular.module('app').controller('PostsController', function ($scope, Post) {

    $scope.posts = Post.query();  
    
});