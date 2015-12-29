angular.module('app').factory('Blog', function ($resource, apiUrl) {
    
    return $resource(apiUrl + 'api/blogs/:id', { id: '@BlogID' }, {
		update: {
			method: 'PUT'
		}
	});
});