angular.module('app').controller('AppController', function (authService, $scope) {
    $scope.authenticationData = authService.authentication;

    $scope.logout = function () {
        authService.logOut();
    };
           
});