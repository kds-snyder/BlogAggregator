﻿angular.module('app').controller('LoginController', function (authService, ExternalLoginService, $mdToast, $scope, $state, User) {

    $scope.$on('event:google-plus-signin-success', function (event, authResult) {

        // Log out to clear token if any
        authService.logOut('login');

        // Get user info, then check if user is registered in external logins with
        //  Google as provider, and Google user ID as provider key
        authService.getUserInfoFromGoogle(authResult.access_token).then(function (data) {

            ExternalLoginService.getExternalLoginForProviderAndKey('Google', data.id)
                .then(function (response) {
                    if (response.length > 0) {
                        // Handle existing user
                        var userData = { provider: 'Google', externalAccessToken: authResult.access_token };
                        $scope.handleExistingExternalUser(userData);
                    }
                    else {
                        // Handle new user (email is used for user name)
                        var newUserData =
                            { userName: data.email, provider: 'Google', externalAccessToken: authResult.access_token };
                        $scope.handleNewExternalUser(newUserData);
                    }                   
                },
            function (error) {
                // Unable to get eternal login data 
                $mdToast.show($mdToast.simple()
                          .content('Unsuccessful login')
                          .position('top left').theme("toast-error"));
            });

        }, function (error) {
            // Unable to get Google user info   
            $mdToast.show($mdToast.simple()
                          .content('Unsuccessful login')
                          .position('top left').theme("toast-error"));
        });
    });

    // External login for existing user: Obtain access token and redirect according to authorization
    $scope.handleExistingExternalUser = function (externalUserData) {
        authService.obtainAccessToken(externalUserData).then(function (response) {
            $mdToast.show($mdToast.simple()
                    .content('Successful login')
                             .position('top left').theme("toast-success"));

            // Redirect to admin if authorized, otherwise to home
            authService.loadAuthData();
            if (authService.authentication.isAuthenticated && authService.authentication.isAuthorized) {
                $state.go('app.admin');
            }
            else {
                $state.go('app.posts');
            }            
        },
            function (err) {
                $mdToast.show($mdToast.simple()
                         .content('Unsuccessful login')
                         .position('top left').theme("toast-error"));
             });
    }

    // External login for new user: Register the new user (includes obtaining access token),
    //  and redirect to home
    $scope.handleNewExternalUser = function (externalNewUserData) {
        authService.registerExternal(externalNewUserData).then(function (response) {
            $mdToast.show($mdToast.simple()
                    .content('Successful login')
                             .position('top left').theme("toast-success"));
            $state.go('app.posts');
        },
          function (response) {
              var errors = [];
              for (var key in response.modelState) {
                  errors.push(response.modelState[key]);
              }
              $mdToast.show($mdToast.simple()
                          .content('Unsuccessful login')
                          .position('top left').theme("toast-error"));
          });
    };

    $scope.$on('event:google-plus-signin-failure', function (event, authResult) {
        // Auth failure or signout detected
        console.log('Google signin failure, error: ' + authResult.error +
                        ', error_subtype: ' + authResult.error_subtype);

        // Log out to clear token if any
        authService.logOut('login');
    });


});