﻿angular.module('app').controller('AdminController', function (authService, $rootScope, $scope, $state) {
   
    $scope.authenticationData = authService.authentication;  

    $scope.tabs = [
        { heading: "Blog Moderation", state: "app.admin.blogs" },
        { heading: "User Management", state: "app.admin.users" }
    ];

    // Log out from administrative state: go to home screen
    $scope.logout = function () {
        authService.logOut('app.posts');
    };   

    // Default administrative state is blog moderation
    $state.go('app.admin.blogs');
});