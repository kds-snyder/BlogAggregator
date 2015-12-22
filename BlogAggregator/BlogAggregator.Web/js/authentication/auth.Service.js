angular.module('app').factory('authService', function (apiUrl, $http, localStorageService, $q, $state) {

    var authServiceFactory = {};

    var _authentication = {
        isAuthenticated: false,
        isAuthorized: false,
        userName: ""
    };

    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };

    // Get user info from Google by sending token
    var _getUserInfoFromGoogle = function (external_access_token) {
        var deferred = $q.defer();

        $http.get('https://www.googleapis.com/oauth2/v1/userinfo?alt=json', {
            headers: {
                'Authorization': 'Bearer ' + external_access_token
            }
        }).then(function (response) {
            deferred.resolve(response.data);
        }, function (error) {
            console.log('Error getting Google user info');
            _logOut('login');
            deferred.reject(error);
        });

        return deferred.promise;
    };

    // Load data from local storage
    var _loadAuthData = function () {

        var authData = localStorageService.get('authenticationData');
        if (authData) {
            _authentication.isAuthenticated = true;
            _authentication.isAuthorized = authData.isAuthorized;
            _authentication.userName = authData.userName;
        }
    };

    // Login: post token
    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(apiUrl + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
        .success(function (response) {

            localStorageService.set('authenticationData',
                 { token: response.access_token, userName: loginData.userName });

            _authentication.isAuthenticated = true;
            _authentication.isAuthorized = response.authorized;
            _authentication.userName = loginData.userName;

            deferred.resolve(response);

        }).error(function (err, status) {

            _logOut('login');
            deferred.reject(err);
        });

        return deferred.promise;

    };

    // Logout: Remove token, reset authentication data, and change to specified state
    var _logOut = function (stateChange) {

        localStorageService.remove('authenticationData');

        _authentication.isAuthenticated = false;
        _authentication.isAuthorized = false;
        _authentication.userName = "";

        if (stateChange.length > 0) {
            $state.go(stateChange);
        }       
    };

    // Get local access token for provider and external token
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
                        isAuthorized: response.isAuthorized
                    });

                _authentication.isAuthenticated = true;
                _authentication.isAuthorized = response.isAuthorized;
                _authentication.userName = response.userName;
 
                deferred.resolve(response);

            }).error(function (err, status) {
                _logOut('app.posts');
                deferred.reject(err);
            });

        return deferred.promise;
    };

    // Register user
    var _registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(apiUrl + 'api/account/registerexternal', registerExternalData)
            .success(function (response) {

                localStorageService.set('authenticationData',
                    {
                        token: response.access_token, userName: response.userName,
                        isAuthorized: response.isAuthorized
                    });

                _authentication.isAuthenticated = true;
                _authentication.isAuthorized = response.isAuthorized;
                _authentication.userName = response.userName;
 
                deferred.resolve(response);

            }).error(function (err, status) {
                _logOut('login');
                deferred.reject(err);
            });

        return deferred.promise;
    };

    // Save user registration
    var _saveRegistration = function (registration) {

        _logOut('login');

        return $http.post(apiUrl + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    authServiceFactory.authentication = _authentication;
    authServiceFactory.externalAuthData = _externalAuthData;   
    authServiceFactory.getUserInfoFromGoogle = _getUserInfoFromGoogle;
    authServiceFactory.loadAuthData = _loadAuthData;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.registerExternal = _registerExternal;
    authServiceFactory.saveRegistration = _saveRegistration;

    return authServiceFactory;
});