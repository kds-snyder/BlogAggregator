angular.module('app').controller('LoginController', function (authService, $scope) {

    $scope.$on('event:google-plus-signin-success', function (event, authResult) {

        console.log('Google signin success');

        //Obtain access token and redirect to admin
        var externalData = { provider: 'Google', externalAccessToken: authResult.access_token };
        authService.obtainAccessToken(externalData).then(function (response) {
            debugger;
            $location.path('/admin');

        },
        function (err) {
         debugger;
         $scope.message = err.error_description;
        });
    });

    $scope.$on('event:google-plus-signin-failure', function (event, authResult) {
        // Auth failure or signout detected
        alert('Google signin failure');
        debugger;
    });


});