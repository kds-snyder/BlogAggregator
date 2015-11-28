//angular.module('app').controller('PostsController', function ($scope, Post) {
angular.module('app').controller('PostsController', function ($scope, $http) {

    // $scope.posts = Post.query();

    // Simple GET request example:        
    debugger;

    $http.get('http://localhost:3000/api/posts')
        .then(function successCallback(response) {
            $scope.posts = response.data;
            debugger;
        },
        function errorCallback(error) {
            alert('An error occurred');
            debugger;
        }
   );

    
});