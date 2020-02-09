ShiftReportsApp.controller('ScheduleController', function ($scope, $http, $rootScope, $location, $cookieStore, $route, calendarConfig, moment) {
    $rootScope.showMenu = false;
 
 
 
    //## scheduler configuration #########################################################################
    calendarConfig.templates.calendarWeekView = 'views/scheduler/calendar-week-view.html';
    calendarConfig.dateFormatter = 'moment'; //use either moment or angular to format dates on the calendar. Default angular. Setting this will override any date formats you have already set.
    // Configuring moment
    moment.updateLocale('en', {
        week: {
            dow: 1 // Monday is the first day of the week
        }
    });

    $scope.scheduler = new Object();
    $scope.scheduler.calendarView = 'week'; //moment().format('YYYY-MM-DD HH:mm:ss')
    $scope.scheduler.viewDate = moment();
    //## end #############################################################################################
 

    // Get Events API by store id and range ##################################################
    $scope.getSchedulerByCashierID = function (cashier_id, start_date, end_date) {
        // AJAX Request get_scheduler
        $http.get($rootScope.APIServerURL + "api/Scheduler/get_scheduler_by_cashier", {
            params: {
                cashier_id: cashier_id,//store_id, $scope.data.selectedStoreID
                date_range_start: start_date,
                date_range_end: end_date,
            }
        }).success(function (APIData, status, headers, config) {
            
            //create Reasons array
            $scope.reasons = [];
            angular.forEach(APIData.scheduler_decline_reasons_dtls, function (reason, key) {
                $scope.reasons.push({
                    "id": reason.id,
                    "text": reason.reason
                });
            });
            
            // create shift list
            calendarConfig.shift_list = [];
            angular.forEach(APIData.shift_store_times, function (shift, key) {
                calendarConfig.shift_list.push(shift);
            });
            
            // create events
            angular.forEach(APIData.scheduler_mst, function (event, key) {

                $scope.events.push({
                    id: event.id,
                    shift_no: event.shift_no,
                    status: event.status,
                    reason_id:event.decline_reason,
                    startsAt: moment(event.assignment_date), // A javascript date object for when the event starts
                });

            });
            console.log($scope.events);

        }).error(function (data, status, headers, config) {
          ///  menuControl.messageBox("modal-danger", "Error", "Can't connect to server");
        });
    };
    //end #######################################################################################





    //## Range operations #############################################################################
    //# create first range depending on today's date one month before and one month after today's date
    $scope.dateRange = [{
        "start_date": moment().subtract(1, 'month'),
        "end_date": moment().add(1, 'month')
    }]

    //# next week button clicked
    $scope.nextWeek = function () {

        var getMoreEvents = true;
        var HigherEndDate = $scope.dateRange[0].end_date; // init higherEndDate to the first range

        for (var i = 0; i < $scope.dateRange.length; i++) {

            var start = $scope.dateRange[i].start_date;
            var end = $scope.dateRange[i].end_date;
            var range = moment.range(start, end);
            if (range.contains($scope.scheduler.viewDate)) {
                getMoreEvents = false;
                //  break;
            }

            // get Higher end_date range
            if (HigherEndDate.isBefore($scope.dateRange[i].end_date)) {
                HigherEndDate = $scope.dateRange[i].end_date;
            }

        }

        if (getMoreEvents) {

            $scope.dateRange.push(
                {
                    "start_date": moment(HigherEndDate),
                    "end_date": moment(HigherEndDate).add(1, 'month')
                })
            // get New events 
            $scope.getSchedulerByCashierID($scope.cashier_id, moment(HigherEndDate).format("YYYY-MM-DD"), moment(HigherEndDate).add(1, 'month').format("YYYY-MM-DD"));

        }

    };
    // end next weeek button

    //# previews Week button
    $scope.previewsWeek = function () {
        var getMoreEvents = true;
        var HigherEndDate = $scope.dateRange[0].end_date; // init higherEndDate to the first range

        for (var i = 0; i < $scope.dateRange.length; i++) {

            var start = $scope.dateRange[i].start_date;
            var end = $scope.dateRange[i].end_date;
            var range = moment.range(start, end);
            if (range.contains($scope.scheduler.viewDate)) {
                getMoreEvents = false;
                //  break;
            }

            // get Higher end_date range
            if (HigherEndDate.isAfter($scope.dateRange[i].start_date)) {
                HigherEndDate = $scope.dateRange[i].start_date;
            }

        }

        if (getMoreEvents) {

            $scope.dateRange.push(
                {
                    "start_date": moment(HigherEndDate).subtract(1, 'month'),
                    "end_date": moment(HigherEndDate)
                })

            console.log($scope.dateRange);
            // call API get new events
            $scope.getSchedulerByCashierID($scope.cashier_id, moment(HigherEndDate).subtract(1, 'month').format("YYYY-MM-DD"), moment(HigherEndDate).format("YYYY-MM-DD"));

        }
    };
    //## end ###############################################################################################

    


    //## init App (entry point) ###############################################################################
    $scope.cashier_id = $cookieStore.get('cachier_id');
    calendarConfig.shift_list = [];
    $scope.getSchedulerByCashierID($scope.cashier_id, moment().subtract(1, 'month').format("YYYY-MM-DD"), moment().add(1, 'month').format("YYYY-MM-DD"));
    //## end ####################################################################################

 
         
    //## Cell clicked ##############################################################################
    $scope.cellClicked = function (event) {
        // get shift information for the modal title
        angular.forEach(calendarConfig.shift_list, function (shift, key) { 
            if (shift.shift_no == event.shift_no) {
                $scope.modal.shift = shift;
            }
        });

        // check if event exist or not
        if (event.exist == true) { // event Exist go to modification or delete modal

            $scope.modal.event = event;
            $scope.modal.status = event.status.toString();
            $scope.modal.reason_id = event.reason_id;
            

            $('#modifyEvent').modal('show');

        }
 
     
    };
    //## end #######################################################################################

 

    //## modal functions ###########################################################################
    // # Create modal object
    $scope.modal = new Object();
    //# modify on event, 
    $scope.modal.modifyAssignedShift = function () {

        angular.forEach($scope.events, function (event, eventkey) {

            if (event.id == $scope.modal.event.id) {
                postAPI = new Object();
                postAPI.id = $scope.modal.event.id;
                postAPI.status = $scope.modal.status;
                if ($scope.modal.status == -1) {
                    postAPI.decline_reason = parseInt($scope.modal.reason_id, 10);;
                }
                else {
                    postAPI.decline_reason = -1;
                }
                //AJAX POST CALL modify_schedule
                $http.post($rootScope.APIServerURL + "api/Scheduler/modify_schedule", angular.toJson(postAPI))
                 .success(function (APIData, status, headers, config) {

                     event.status = $scope.modal.status;
                     if ($scope.modal.status == -1) {// if status equal decline update reason_id

                         event.reason_id = $scope.modal.reason_id;
                     }
                     else {
                         event.reason_id = -1;
                     }

                 }).error(function (data, status, headers, config) {
                     // TODO Error message can't connect to server 
                   //  menuControl.messageBox("modal-danger", "Error", "Can't connect to server");
                     console.log(data);
                 });




            }

        });


    };
    //## end #######################################################################################


    //## Navigation button go back to the app ########################################################
    $scope.goBackToTheApp = function () {
        if (!angular.isUndefined($cookieStore.get('shift_id'))) {

            shift_id=$cookieStore.get('shift_id');
            
            if (shift_id != -1) {
            	if (!$rootScope.is_simpleplan)
            		$location.path('/cash_open_drawer');
            	else
            		$location.path('/shift_details');
            }
            else {
                $location.path("/select-shift");
            }

        }       
    };
   //## end  #########################################################################################  


    // data event
    $scope.events = [
 {
      id:1,
      shift_no: 1,
      status: 1,
      reason_id: 0,
      
 
      startsAt: moment("2016-05-16"), // A javascript date object for when the event starts
 },
   
  {
      id: 2,
      shift_no: 2,
      status: 0,
      reason_id: 0,
     
 
      startsAt: moment("2016-05-17"), // A javascript date object for when the event starts
 },
 {
    id: 3,
    shift_no: 3,
    status: -1,
    reason_id: 3,
    
 
    startsAt: moment("2016-05-18"), // A javascript date object for when the event starts
}
    ];


    
  



});