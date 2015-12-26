angular.module('app').controller('AppController', function (authService, $scope, $state) {
    $scope.authenticationData = authService.authentication;

    // Log out: change the state to home if in an admin state
    $scope.logout = function () {
        if ($state.current.name.substr(0,9) == 'app.admin') {
            authService.logOut('app.posts');
        }
        else {
            authService.logOut('');
        }       
    };

    // Admin button clicked:
    //  Default administrative state is blog moderation,
    //  unless current state is user management   
    $scope.gotoAdmin = function () {
        console.log('app.ctrl.js, Admin button clicked, state: ' + $state.current.name);
        if ($state.current.name != 'app.admin.users') {
            $state.go('app.admin.blogs');
        }       
    };
           
});