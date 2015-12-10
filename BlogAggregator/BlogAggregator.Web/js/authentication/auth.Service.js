angular.module('app').factory('authService', function (apiUrl, $http, localStorageService, $q, $state) {

    var authServiceFactory = {};

    var _authentication = {
        isAuthenticated: false,
        isAuthorized: false,
        userName: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(apiUrl + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(apiUrl + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
        .success(function (response) {

            localStorageService.set('authenticationData', { token: response.access_token, userName: loginData.userName });

            _authentication.isAuthenticated = true;
            _authentication.isAuthorized = response.authorized;
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
        _authentication.userName = "";

        $state.go('login');
    };

    var _loadAuthData = function () {

        var authData = localStorageService.get('authenticationData');
        if (authData) {
            _authentication.isAuthenticated = true;
            _authentication.isAuthorized = authData.isAuthorized;
            _authentication.userName = authData.userName;
        }
    }

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.loadAuthData = _loadAuthData;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
});