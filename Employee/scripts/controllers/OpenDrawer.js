ShiftReportsApp.controller('OpenDrawerController', function ($scope, $http, Shift, $rootScope, LSOperation, $cookieStore, accessController) {

    accessController.authentificate();

   
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
        $scope.total =  ($scope.shift.OC_B_Fifties * 50)+
                        ($scope.shift.OC_B_Twenties * 20) +
                        ($scope.shift.OC_B_Tens * 10) +
                        ($scope.shift.OC_B_Fives * 5) +
                        ($scope.shift.OC_B_Singles * 1) +
                        ($scope.shift.OC_L_Dollars * 1) +
                        ($scope.shift.OC_L_Quarters * 0.25) +
                        ($scope.shift.OC_L_Dimes * 0.10) +
                        ($scope.shift.OC_L_Nickels * 0.05) +
                        ($scope.shift.OC_L_Pennies * 0.01) +
                        ($scope.shift.OC_R_Quarters * 10) +
                        ($scope.shift.OC_R_Dimes * 5) +
                        ($scope.shift.OC_R_Nickels * 2) +
                        ($scope.shift.OC_R_Pennies * 0.50);
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


/*
     
    var shift = new Shift();
    shift.cachierID = 2221;
    shift.cachierName = 'Adil12';


    persistence.flush(function () {
        // commit done
        console.debug('Done flushing');
    });

    persistence.add(shift);
    */
   
    
    /*
        
    LSOperation.getShift(2221).then(function (messages) {

        if (messages == null) {
            console.log("no shift found");
        }
        else {
            console.log("Shift found");
            console.log(messages.cachierID);
            $scope.message = messages.cachierID;
        }
    });
     
     
    console.log('Cachier id' + Shift.cachierID);
      
 */

    /* last commented

   var result=Shift.all().filter('cachierID', '=', 2221);

   result.list(null, function (result) {

        result.forEach(function (t) {
            
            //console.log(t.cachierID);
            $scope.message = t.cachierID;
            console.log('Request completed ' + t.cachierID);
             
 
        });

       
    });
    

    
     

    console.log('Getting the scoop variable ' + $scope.message);
   */
    

   // var results = db.query("SELECT * FROM Shift");


     /*
    var shift = new Shift();
    shift.cachierID = 2221;
    shift.cachierName = 'Adil12';


    persistence.flush(function () {
        // commit done
        console.debug('Done flushing');
    });

    persistence.add(shift);
    */

    
    /*
    Shift.all().list(null, function (result) {
        result.forEach(function (t) {
            console.log(t.cachierID);
            console.log(t.cachierName);
             
        });;
    });
    */


     

    /* Working example
    var tarea = new Task();
    tarea.name = 'Una tarea importante 4';
    tarea.description = 'Una descripcion larga larga largaaaa';
    tarea.done = false;


    persistence.add(tarea);


    // commit changes.
    persistence.flush(function() {
        // commit done
        console.debug('Done flushing');
    });


     

    Task.all().list(null, function (result) {
        result.forEach(function (t) {
            console.log(t.name);
        });;
    });
    */


   // console.log(localStorageService.get('key1'));

    //$scope.message = test;
 
    //localStorageService.set('1', 'hello');

    /*

    $scope.$storage = $localStorage.$default({

        active_shift: {
            cachier_info: { cachier_id: 81, cashier_name: 'adil' },
            shift_info: { shift_number: 1, shift_time_begins: '0AM', shift_time_ends: '2AM' },
            adilo_c: { shift_number: 1, shift_time_begins: '0AM', shift_time_ends: '2AM' },
            opening_c: {
                bills: { fifties: 2, twenties: 4, tens: 6, fives: 2, singles: 30 },
                l_coins: { dollars: 12, quarters: 14, dimes: 16, nickels: 12, pennies: 31 },
                r_coins: { dollars: 4, quarters: 2, dimes: 10, nickels: 6, pennies: 6 }
            },

        }

    });

    // delete $scope.$storage;

    console.log($scope.$storage);

    */

});