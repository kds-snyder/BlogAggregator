//angular.module('app', ['directive.g+signin', 'ngMaterial', 'ngMessages', 'ngResource', 'ngSanitize', 'ui.router'])
angular.module('app', ['ngMaterial', 'ngMessages', 'ngResource', 'ngSanitize', 'ui.router'])
                .config(function ($stateProvider, $urlRouterProvider, $mdThemingProvider) {

    $mdThemingProvider.theme('default')
        .primaryPalette('cyan')
        .accentPalette('green');
    $mdThemingProvider.theme('toast-error');
    $mdThemingProvider.theme('toast-success');

    $urlRouterProvider.otherwise('/app/posts');

    $stateProvider
        .state('login', { url: '/login', templateUrl: '/templates/authentication/login.html', controller: 'LoginController' })

         .state('admin', { url: '/admin', templateUrl: '/templates/administration/admin.html', controller: 'AdminController' })
            .state('admin.blogs', { url: '/blogs', templateUrl: '/templates/administration/blogs.html', controller: 'AdminBlogsController' })
            .state('admin.users', { url: '/users', templateUrl: '/templates/administration/users.html', controller: 'AdminUsersController' })
 
        .state('app', { url: '/app', templateUrl: '/templates/app/app.html', controller: 'AppController' })

         .state('app.posts', { url: '/posts', templateUrl: '/templates/app/posts.html', controller: 'PostsController' })
         .state('app.addblog', { url: '/addblog', templateUrl: '/templates/app/addblog.html', controller: 'AddBlogController' })

});

//API link
angular.module('app').value('apiUrl', 'http://localhost:3000/');
