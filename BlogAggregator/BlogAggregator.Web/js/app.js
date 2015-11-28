angular.module('app', ['ui.router', 'ngMaterial', 'ngMessages', 'ngResource']).config(function ($stateProvider, $urlRouterProvider, $mdThemingProvider) {

    $mdThemingProvider.theme('default')
        .primaryPalette('cyan')
        .accentPalette('green');
    
    $urlRouterProvider.otherwise('/app/posts');

    $stateProvider
        .state('app', { url: '/app', templateUrl: '/templates/app/app.html', controller: 'AppController' })
               
         .state('app.posts', { url: '/posts', templateUrl: '/templates/app/posts.html', controller: 'PostsController' })
            .state('app.posts.addblog', { url: '/addblog', templateUrl: '/templates/app/addblog.html', controller: 'AddBlogController' })

});

//API link
angular.module('app').value('apiUrl', 'http://localhost:3000/');
