ShiftReportsApp.controller('SalesInfoController', function ($scope, $http, Shift, $rootScope, LSOperation, $cookieStore, accessController) {

    accessController.authentificate();


    console.log("id shift from cookies" + $cookieStore.get('shift_id'));

    LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {

        if (data != null) {

            console.log("Shift found in LS loading data");
            $scope.shift = data;
 
        }
 
    });


    $scope.updateField = function () {
         
        console.log("shift_id before adding" + $scope.shift.shift_id);
        persistence.add($scope.shift); // Modify shift opening Cash Drawer data

        // commit changes.
        persistence.flush(function () {
            // commit done
            console.debug('Done Adding shift  shif_id=' + $scope.shift.shiftID);
        });

    };

 

});