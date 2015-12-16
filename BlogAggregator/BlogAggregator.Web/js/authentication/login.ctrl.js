angular.module('app').controller('LoginController', function (authService, $mdToast, $scope, User) {

    $scope.$on('event:google-plus-signin-success', function (event, authResult) {

        console.log('Google signin success');

        // Get user info, then check if user is registered in database with
        //  Google as provider, and Google user ID as provider key
        authService.getUserInfoFromGoogle(authResult.access_token).then(function (data) {

            var externalData = { provider: 'Google', providerKey: data.id };
            authService.findExternalLoginUser(externalData).then(function (response) {

                // Handle existing user
                console.log('Found external user ' + data.email);
                debugger;
                var userData = { provider: 'Google', externalAccessToken: authResult.access_token };
                $scope.handleExistingExternalUser(userData);
            },
            function (error) {
                
                // Handle new user (email is used for user name)
                console.log('Did not find external user ' + data.email);
                debugger;
                var newUserData =
                    { userName: data.email, provider: 'Google', externalAccessToken: authResult.access_token };
                $scope.handleNewExternalUser(newUserData);
            });

        }, function (error) {
            debugger;
            console.log('Error getting Google user info: ' + error);
        });
       
    });

    // External login for existing user: Obtain access token and redirect to admin
    $scope.handleExistingExternalUser = function(externalUserData) {
        authService.obtainAccessToken(externalUserData).then(function (response) {
            console.log('Obtained access token successfully');
            debugger;
            $state.go('admin');
        },
            function (err) {
                debugger;
                console.log('Failed to get access token: ' + err.error_description);
            });
    }

    // External login for new user: Register the new user (includes obtaining access token),
    //  and redirect to admin
    $scope.handleNewExternalUser = function (externalNewUserData) {
        authService.registerExternal(externalNewUserData).then(function (response) {
            console.log('Registered new user ' + externalNewUserData.userName);
            $state.go('admin');

        },
          function (response) {
              var errors = [];
              for (var key in response.modelState) {
                  errors.push(response.modelState[key]);
              }
              debugger;
              $mdToast.show($mdToast.simple()
                          .content('Unable to register you as a user')
                          .position('top left').theme("toast-error"));
              console.log('Failed to register user' + externalNewUserData.userName
                                                + ' due to: ' + errors.join(' '));
          });
    };

    $scope.$on('event:google-plus-signin-failure', function (event, authResult) {
        // Auth failure or signout detected
        console.log('Google signin failure');
    });


});