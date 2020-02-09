ShiftReportsApp.controller('ForgotPasswordController', function ($scope,ShiftInfo, $http, $rootScope, $location, $cookieStore, Shift, LSOperation) {

    $rootScope.showMenu = false;
     
    $scope.forgotPassword = function () {

        

        //AJAX GET CALL forgot_manager_password
        $http.get($rootScope.APIServerURL + "api/Business/cashier_forgot_password", {
            params: {
                cashier_email: $scope.cashier_email
            }
        }).success(function (APIData, status, headers, config) {
            // test if success =1
            $('#successPasswordModal').modal('show');

            $rootScope.passwordSuccesfullySent = true;
            $location.path('/mobile-cash-business-login');

        }).error(function (data, status, headers, config) {
            // TODO Error message can't connect to server 
            $('#errorPasswordModal').modal('show');
        });

 
         
    };


    $scope.login = function () {

        $location.path('/mobile-cash-business-login');
    };


 
});