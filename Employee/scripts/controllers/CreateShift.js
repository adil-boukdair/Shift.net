ShiftReportsApp.controller('CreateShiftController', function ($scope, $http, $rootScope, LSOperation, Shift, $cookieStore, $location, accessController, ShiftInfo) {

    accessController.authentificate();

    $rootScope.showMenu = false;

    

    // Getting data from LS
   
   
   
     
     // get shift Time info
    $http.get($rootScope.APIServerURL + "api/Shift/BusinessShiftsInfo?casheir_id=" + $cookieStore.get('cachier_id'), {
    }).success(function (data, status, headers, config) {
        console.log(data);
        $scope.shiftInfo = data.shifts;
        console.log($scope.shiftInfo);

    }).error(function (data, status, headers, config) {
        // TODO Error message can't connect to server 
        console.log("error");
    });
   
 
 
    $scope.createShift = function (shift_no,start_time,end_time) {
        
        console.log("shift_no:" + shift_no);

        console.log("###date" + moment($cookieStore.get('shift_session_started')).format('MM/DD/YYYY'));

        $http.post($rootScope.APIServerURL + "api/Shift/CreateShiftReport", {
            casheir_id: $cookieStore.get('cachier_id'),
            shift_no: shift_no,
            shift_date: moment($cookieStore.get('shift_session_started')).format('MM/DD/YYYY'),
            shift_session_started: moment().format('HHmm')
        }).success(function (data, status, headers, config) {

            //TODO need to find out data return in case of error
            
            console.log("shift id : "+data.shiftkey);
            $cookieStore.put('shift_id', data.shiftkey); // set Shift ID 
            $cookieStore.put('shift_time', start_time + ' - ' + end_time);
            $cookieStore.put('shift_no', shift_no);
            $rootScope.shiftTime = start_time + ' - ' + end_time;
              
            // session start time
            $cookieStore.put('shift_session_started', moment().format('YYYY-MM-DD HH:mm:ss'));
       
            ShiftInfo.set(); 



        }).error(function (data, status, headers, config) {
            // TODO Error message can't connect to server 
            console.log("error create shift");
        });


 


    };






});