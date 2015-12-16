angular.module('app').controller('LoginController', function (authService, ExternalLoginService, $mdToast, $scope, $state, User) {

    $scope.$on('event:google-plus-signin-success', function (event, authResult) {

        console.log('Google signin success, access token: ' + authResult.access_token);

        // Get user info, then check if user is registered in external logins with
        //  Google as provider, and Google user ID as provider key
        authService.getUserInfoFromGoogle(authResult.access_token).then(function (data) {

            ExternalLoginService.getExternalLoginForProviderAndKey('Google', data.id)
                .then(function (response) {
                    if (response.length > 0) {
                        // Handle existing user
                        console.log('Found existing external user ' + data.email);
                        var userData = { provider: 'Google', externalAccessToken: authResult.access_token };
                        $scope.handleExistingExternalUser(userData);
                    }
                    else {
                        // Handle new user (email is used for user name)
                        console.log('Found new external user ' + data.email);
                        var newUserData =
                            { userName: data.email, provider: 'Google', externalAccessToken: authResult.access_token };
                        $scope.handleNewExternalUser(newUserData);
                    }                   
                },
            function (error) {
                // Unable to get eternal login data 
                console.log('Error searching for external user in external logins' + data.email
                                + ' due to: ' + error);               
            });

        }, function (error) {
            // Unable to get Google user info, but change to admin state if user is authenticated & authorized
            debugger;
            console.log('Error getting Google user info: ' + error);
            console.log('authService.authentication.isAuthenticated: ' + authService.authentication.isAuthenticated);
            console.log('authService.authentication.isAuthorized: ' + authService.authentication.isAuthorized);
            if (authService.authentication.isAuthenticated
                && authService.authentication.isAuthorized) {
                $state.go('admin');
            }
                
        });

    });

    // External login for existing user: Obtain access token and redirect to admin
    $scope.handleExistingExternalUser = function (externalUserData) {
        authService.obtainAccessToken(externalUserData).then(function (response) {
            console.log('Obtained access token successfully');
            $state.go('admin');
        },
            function (err) {
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