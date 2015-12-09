angular.module('app').controller('AdminController', function ($rootScope, $scope, $state) {

    // Default administrative state is blog moderation
    $state.go('admin.blogs');
});