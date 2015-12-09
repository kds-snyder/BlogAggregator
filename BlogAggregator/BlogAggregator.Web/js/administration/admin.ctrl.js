angular.module('app').controller('AdminController', function ($rootScope, $scope, $state) {

    $scope.tabs = [
        { heading: "Blog Moderation", state: "admin.blogs" },
        { heading: "User Management", state: "admin.users" }
    ];

    // Default administrative state is blog moderation
    $state.go('admin.blogs');
});