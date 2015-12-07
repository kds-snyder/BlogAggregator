angular.module('app').service('PostService', function ($http, apiUrl) {
    return {
        getAllPostsByDateDesc: function () {
            var promise = $http.get(apiUrl + 'api/posts?$orderby=PublicationDate desc')
//            var promise = $http.get('http://a.b.com')
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

