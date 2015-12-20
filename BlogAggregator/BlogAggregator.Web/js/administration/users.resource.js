angular.module('app').factory('User', function ($resource, apiUrl) {

    return $resource(apiUrl + 'api/users/:id', { id: '@Id' }, {
        update: {
            method: 'PUT'
        }
    });
});