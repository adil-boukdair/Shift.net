ShiftReportsApp.controller('SafeDropsController', function ($scope, $http, Shift, $rootScope, LSOperation, $cookieStore, accessController) {

    accessController.authentificate();
     
    $scope.total = 0;
    $scope.safeDrop = { _id: 1, value: '' };



    // get that from local storage if exist
   // $scope.safeDrops = [{ number: 1, amount: '7.03' }, { number: 2, amount: '6.50' }, { number: 3, amount: '5.52' }];

   
    LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {

        if (data != null) {
            console.log("Shift found in LS loading data");
            $scope.shift = data;
            if (angular.isUndefined($cookieStore.get('shift_time'))) {
                $rootScope.shiftTime = $scope.shift.shiftBeginEndTime;
            }
            if ($scope.shift.SD_SafeDrops == null) { // safedrops counter 1
                $scope.safeDrop._id = 1;
                $scope.shift.SD_SafeDrops = [];
            }
            else {
                $scope.safeDrop._id = $scope.shift.SD_SafeDrops.length + 1;// get safe drops counter
            }
           
        }
        
        
        $scope.calculate();
        
        
    });


    
    //

    


    $scope.addSafeDrop = function () {
        

        if ($scope.safeDrop.value != '') { //TODO indicate to user to enter an amount

           
            $scope.shift.SD_SafeDrops.push({ _id: ($scope.shift.SD_SafeDrops.length + 1), value: $scope.safeDrop.value }); // add safe drop to safedrops array

            console.log($scope.shift.SD_SafeDrops);

            $scope.safeDrop._id = $scope.safeDrop._id + 1; // increment safe drop counter
                        

            persistence.add($scope.shift); // Modify shift safeDrop data
            
            $scope.shift.markDirty('SD_SafeDrops'); // special for JSON fields
            // commit changes.
            persistence.flush(function () {
                // commit done
                console.debug('Done Adding shift  shif_id=' + $scope.shift.shiftID);
                console.log($scope.shift.SD_SafeDrops);
                
            });

            $scope.safeDrop.value = '';
            $scope.calculate();// calculate total
            
            console.log(JSON.stringify($scope.shift.SD_SafeDrops));
            
            
        }
 
    };


    $scope.updateField = function () {

         

        persistence.add($scope.shift); // Modify shift safeDrop data

        $scope.shift.markDirty('SD_SafeDrops'); // special for JSON fields
        // commit changes.
        persistence.flush(function () {
            // commit done
            console.debug('Done Adding shift  shif_id=' + $scope.shift.shiftID);
            console.log(angular.toJson($scope.shift.SD_SafeDrops));

        });
       
        $scope.calculate();// calculate total
 
    };
    

    $scope.calculate = function () {

        $scope.total = 0;
        if ($scope.safeDrop.value != '') {
            $scope.total = $scope.total + parseFloat($scope.safeDrop.value);


        }
        angular.forEach($scope.shift.SD_SafeDrops, function (safeDrop, key) {
            $scope.total = $scope.total + parseFloat(safeDrop.value);
        });

       
        
        console.log("total= " + $scope.total);
    };




    $scope.removeFromSafeDrop = function (_id) {


        angular.forEach($scope.shift.SD_SafeDrops, function (safeDrop, key) {
            
            if (safeDrop._id == _id) {
                console.log("key of the element = " + key);
                console.log("value on array: " + $scope.shift.SD_SafeDrops[key].value);

                $scope.shift.SD_SafeDrops.splice(key, 1);

                persistence.add($scope.shift); // Modify shift safeDrop data

                $scope.shift.markDirty('SD_SafeDrops'); // special for JSON fields
                // commit changes.
                persistence.flush(function () {
                    // commit done
                    console.debug('Done Adding shift  shif_id=' + $scope.shift.shiftID);
                    console.log($scope.shift.SD_SafeDrops);

                });

            }
             
               
            
        });

    };



});