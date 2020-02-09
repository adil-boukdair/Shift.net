ShiftReportsApp.controller('CloseDrawerController', function ($scope, $http, Shift, $rootScope, LSOperation, $cookieStore) {


    $rootScope.showMenu = true;

    $scope.total = 0;

    console.log("id shift from cookies" + $cookieStore.get('shift_id'));

    LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {

        if (data != null) {
            console.log("Shift found in LS loading data");
            $scope.shift = data;
            if (angular.isUndefined($cookieStore.get('shift_time'))) {
                $rootScope.shiftTime = $scope.shift.shiftBeginEndTime;
            }
            $scope.calculateTotal();

        }
 
    });


    $scope.calculateTotal = function () {
        $scope.total = ($scope.shift.CC_B_Fifties * 50) +
                        ($scope.shift.CC_B_Twenties * 20) +
                        ($scope.shift.CC_B_Tens * 10) +
                        ($scope.shift.CC_B_Fives * 5) +
                        ($scope.shift.CC_B_Singles * 1) +
                        ($scope.shift.CC_L_Dollars * 1) +
                        ($scope.shift.CC_L_Quarters * 0.25) +
                        ($scope.shift.CC_L_Dimes * 0.10) +
                        ($scope.shift.CC_L_Nickels * 0.05) +
                        ($scope.shift.CC_L_Pennies * 0.01) +
                        ($scope.shift.CC_R_Quarters * 10) +
                        ($scope.shift.CC_R_Dimes * 5) +
                        ($scope.shift.CC_R_Nickels * 2) +
                        ($scope.shift.CC_R_Pennies * 0.50);
    };



    $scope.updateField = function () {
        $scope.calculateTotal();
        console.log("shift_id before adding" + $scope.shift.shift_id);
        persistence.add($scope.shift); // Modify shift opening Cash Drawer data

        // commit changes.
        persistence.flush(function () {
            // commit done
            console.debug('Done Adding shift  shif_id=' + $scope.shift.shiftID);
        });

    };

 

});