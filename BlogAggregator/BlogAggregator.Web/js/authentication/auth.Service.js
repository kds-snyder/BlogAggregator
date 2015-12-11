angular.module('app').factory('authService', function (apiUrl, appClientID, $http, localStorageService, $q, $state) {

    var authServiceFactory = {};

    var _authentication = {
        isAuthenticated: false,
        isAuthorized: false,
        useRefreshTokens: false,
        userName: ""
    };

    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(apiUrl + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        if (loginData.useRefreshTokens) {
            data = data + "&client_id=" + appClientID;
        }

        var deferred = $q.defer();

        $http.post(apiUrl + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
        .success(function (response) {

            if (loginData.useRefreshTokens) {
                localStorageService.set('authenticationData',
                    {
                        token: response.access_token, userName: loginData.userName,
                        refreshToken: response.refresh_token, useRefreshTokens: true
                    });
            }
            else {
                localStorageService.set('authenticationData',
                    { token: response.access_token, userName: loginData.userName });
            }

            _authentication.isAuthenticated = true;
            _authentication.isAuthorized = response.authorized;
            _authentication.useRefreshTokens = loginData.useRefreshTokens;
            _authentication.userName = loginData.userName;

            deferred.resolve(response);

        }).error(function (err, status) {

            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorageService.remove('authenticationData');

        _authentication.isAuthenticated = false;
        _authentication.useRefreshTokens = false;
        _authentication.userName = "";

        $state.go('login');
    };

    var _loadAuthData = function () {

        var authData = localStorageService.get('authenticationData');
        if (authData) {
            _authentication.isAuthenticated = true;
            _authentication.isAuthorized = authData.isAuthorized;
            _authentication.useRefreshTokens = authData.useRefreshTokens;
            _authentication.userName = authData.userName;
        }
    }

    var _refreshToken = function () {
        var deferred = $q.defer();

        var authData = localStorageService.get('authenticationData');

        if (authData) {

            if (authData.useRefreshTokens) {

                var data = "grant_type=refresh_token&refresh_token=" +
                    authData.refreshToken + "&client_id=" + appClientID;

                localStorageService.remove('authenticationData');

                $http.post(apiUrl + 'token', data,
                    { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
                    .success(function (response) {

                        localStorageService.set('authenticationData',
                            {
                                token: response.access_token, userName: response.userName,
                                refreshToken: response.refresh_token, useRefreshTokens: true
                            });

                        deferred.resolve(response);

                    }).error(function (err, status) {
                        _logOut();
                        deferred.reject(err);
                    });
            }
        }

        return deferred.promise;
    };

    var _obtainAccessToken = function (externalData) {

        var deferred = $q.defer();

        $http.get(apiUrl + 'api/account/ObtainLocalAccessToken',
            {
                params: {
                    provider: externalData.provider,
                    externalAccessToken: externalData.externalAccessToken
                }
            })
            .success(function (response) {

                localStorageService.set('authenticationData',
                    {
                        token: response.access_token, userName: response.userName,
                        refreshToken: "", useRefreshTokens: false
                    });

                _authentication.isAuthenticated = true;
                _authentication.isAuthorized = response.isAuthorized;
                _authentication.userName = response.userName;
                _authentication.useRefreshTokens = false;

                deferred.resolve(response);

            }).error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

        return deferred.promise;
    };

    var _registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(apiUrl + 'api/account/registerexternal', registerExternalData)
            .success(function (response) {

                localStorageService.set('authenticationData',
                    {
                        token: response.access_token, userName: response.userName,
                        refreshToken: "", useRefreshTokens: false
                    });

                _authentication.isAuthenticated = true;
                _authentication.isAuthorized = response.isAuthorized;
                _authentication.userName = response.userName;
                _authentication.useRefreshTokens = false;

                deferred.resolve(response);

            }).error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

        return deferred.promise;
    };
    
    authServiceFactory.authentication = _authentication;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.loadAuthData = _loadAuthData;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.refreshToken = _refreshToken;
    authServiceFactory.registerExternal = _registerExternal;
    authServiceFactory.saveRegistration = _saveRegistration;

    return authServiceFactory;
});