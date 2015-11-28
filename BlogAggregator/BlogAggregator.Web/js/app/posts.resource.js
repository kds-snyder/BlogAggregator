angular.module('app').factory('Post', function ($resource, apiUrl) {
    
    return $resource(apiUrl + 'api/posts/:id', { id: '@PostID' }, {
		update: {
			method: 'PUT'
		}
	});
});