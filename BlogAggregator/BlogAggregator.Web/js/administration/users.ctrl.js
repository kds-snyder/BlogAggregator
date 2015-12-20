angular.module('app').controller('AdminUsersController', function (authService, $mdToast, $scope, User) {

    $scope.authenticationData = authService.authentication;
   
    // Function to load users, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        $scope.users = User.query(function () {
            $scope.loading = false;
        });
    };

    // Authorize user
    $scope.authorizeUser = function (user) {
        user.Authorized = true;
        user.$update(function () {
            $mdToast.show($mdToast.simple().content('User authorized successfully')
                             .position('top left').theme("toast-success"));
        },
        function (error) {
            $mdToast.show($mdToast.simple()
                           .content('Unable to authorize user')
                           .position('top left').theme("toast-error"));
            user.Authorized = false;
        });
    };

    // Deauthorize user
    $scope.deauthorizeUser = function (user) {
        user.Authorized = false;
        user.$update(function () {
            $mdToast.show($mdToast.simple().content('User deauthorized successfully')
             .position('top left').theme("toast-success"));

            // If user deauthorized is same as logged-in user, then log out the user
            if (user.UserName == $scope.authenticationData.userName) {
                authService.logOut();
            }
        },
        function (error) {
            $mdToast.show($mdToast.simple()
                           .content('Unable to deauthorize user')
                           .position('top left').theme("toast-error"));
            user.Authorized = true;
        });
    };

    $scope.deleteUser = function (user) {
        if (confirm('Are you sure you want to delete this user?')) {
            User.delete({ id: user.Id }, function (data) {
                var index = $scope.users.indexOf(user);
                $scope.users.splice(index, 1);
                $mdToast.show($mdToast.simple().content('User deleted successfully')
                             .position('top left').theme("toast-success"));
            },
            function (error) {               
                $mdToast.show($mdToast.simple()
                          .content('Unable to delete user')
                          .position('top left').theme("toast-error"));
            });
        }
    };

    // After all definitions, load the users
    $scope.load();

});