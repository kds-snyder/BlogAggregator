angular.module('app').controller('LoginController', function ($scope) {
   
    $scope.$on('event:google-plus-signin-success', function (event, authResult) {
        // Send login to server or save into cookie
        alert('Google+ signin success');
    });

    $scope.$on('event:google-plus-signin-failure', function (event, authResult) {
        // Auth failure or signout detected
        alert('Google+ signin failure');
    });

});