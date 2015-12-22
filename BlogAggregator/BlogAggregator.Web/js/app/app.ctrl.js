angular.module('app').controller('AppController', function (authService, $scope) {
    $scope.authenticationData = authService.authentication;

    // Log out: do not change the state
    $scope.logout = function () {
        authService.logOut('');
    };
           
});