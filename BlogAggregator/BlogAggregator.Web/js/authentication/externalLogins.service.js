angular.module('app').service('ExternalLoginService', function ($http, apiUrl) {
    return {
        getExternalLoginForProviderAndKey: function (providerName, providerKey) {
            var promise = $http.get(apiUrl + 'api/ExternalLogins?$filter=LoginProvider eq ' +
                                        providerName + '&$filter=ProviderKey eq ' + providerKey)
                            .then(function (response) {
                              return response.data;
                            },
                            function (error) {
                                return error;
                            });
            return promise;
        }
    };
});

