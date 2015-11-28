angular.module('app').factory('Author', function ($resource, apiUrl) {
    
    return $resource(apiUrl + 'api/authors/:id', { id: '@AuthorID' }, {   
		update: {
			method: 'PUT'
		}
	});
});