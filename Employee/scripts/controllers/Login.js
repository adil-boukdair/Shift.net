ShiftReportsApp.controller('LoginController', function ($scope,ShiftInfo, $http, $rootScope, $location, $cookieStore, Shift, LSOperation) {

    $rootScope.showMenu = false;

    $http.defaults.useXDomain = true;

    $scope.username;
    $scope.password;
 
    if ($rootScope.shiftSuccessfullySubmited) {
        $rootScope.shiftSuccessfullySubmited = false;
        $('#successSubmitModal').modal('show');
    }

    if ($rootScope.passwordSuccesfullySent) {
        $rootScope.passwordSuccesfullySent = false;
        $('#successPasswordModal').modal('show');
    }


    $scope.logIn = function () {
        
        $http.post($rootScope.APIServerURL + "api/Shift/cashier_login", {
            username: $scope.username,
            password: $scope.password
        }).success(function (data, status, headers, config) {
            if (data == -1) { // password inccorect
                //TODO show user password is incorrect 

                $scope.errorMessage = 'Wrong Username or Password';
                
            }
            else {

                if ($rootScope.checkPaymentStatus(data.payment_status) == -1) {
                    return 0; // exit function
                }

                $cookieStore.put('cachier_id', data.casher_id);
                $cookieStore.put('cachier_name', data.casher_name);
                $cookieStore.put('is_Loged', true);
              
                $rootScope.cashierName = data.casher_name;
                $rootScope.plan_id = data.plan_id;

				// Added by henry
                $rootScope.is_basicplan = (data.plan_no == 3 );// Test is it a basic plan
                $rootScope.is_plusplan = (data.plan_no == 4);// Test is it a plus plan
                $rootScope.is_premiumplan = (data.plan_no == 5);// Test is it primum
                $rootScope.is_platinumplan = (data.plan_no == 37); // Test is it platinum
                $rootScope.is_simpleplan = (data.plan_no == 29); // Test if it is a simple plan
                

                if (data.shift_id == -1) { // no shift found go to create shift
                    $cookieStore.put('shift_id', data.shift_id); // -1 for no shift found
                    $location.path('/select-shift');
                }
                else {
                    console.log("have shift in db " + data.shift_id);
                    $cookieStore.put('shift_id', data.shift_id); 
                    
                    LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {
                        if (data == null) {
                            // if no shift in local storage return error message
                            // show error message
                            $scope.errorMessage = 'This shift is already in progress on a different device or you have already successfully submitted a shift for this time period.Please submit the previous report before starting a new shift report.';
                            $cookieStore.put('is_Loged', false);
                        }
                        else { // shift exist in local storage permit login in this device
                        	//ShiftInfo.set();
                        	if (!$rootScope.is_simpleplan)
                        		$location.path('/cash_open_drawer');
                        	else
                        		$location.path('/shift_details');
                        }
                    });

                }

            }

        }).error(function (data, status, headers, config) {
            // TODO Error message can't connect to server 
        });

        
         
    };


    $scope.forgotPassword = function () {

        $location.path('/forgot-password');
    };

 

 
});