angular.module('app').controller('AdminController', function (authService, $rootScope, $scope, $state) {

    $scope.authenticationData = authService.authentication;  

    $scope.tabs = [
        { heading: "Blog Moderation", state: "admin.blogs" },
        { heading: "User Management", state: "admin.users" }
    ];

    $scope.logout = function () {
        authService.logOut();
    };

    // Default administrative state is blog moderation
    $state.go('admin.blogs');
});