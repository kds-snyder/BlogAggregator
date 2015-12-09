angular.module('app').controller('LoginController', function ($scope) {
   
    $scope.googleLoginCallback = function (token) {
        alert('Google login callback');
    };

});