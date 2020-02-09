//Define an angular module for our app
var ShiftReportsApp = angular.module('ShiftReportsApp', ['ngRoute', 'ngCookies', 'ngResource', 'mwl.calendar']); // this line ?
 


/*####### Directives ################### */
ShiftReportsApp.directive('stringToNumber', function () {
    return {
        require: 'ngModel',
        link: function(scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function(value) {
                return '' + value;
            });
            ngModel.$formatters.push(function(value) {
                return parseFloat(value, 10);
            });
        }
    };
});
// filters
ShiftReportsApp.filter('unique', function () {

    return function (arr, targetField) {

        var values = [],
            i,
            unique,
            l = arr.length,
            results = [],
            obj;
        for (i = 0; i < arr.length; i++) {

            obj = arr[i];

            unique = true;
            for (v = 0; v < values.length; v++) {
                if (obj[targetField] == values[v]) {
                    unique = false;
                }
            }
            if (unique) {
                values.push(obj[targetField]);
                results.push(obj);
            }

        }
        return results;
    };
})

// json date formater
ShiftReportsApp.filter("formatdate", [function () {
    var result = function (date, formatstring) {
        if (formatstring === null) {
            formatstring = "DD MMM YYYY";
        }
        return moment(date).format(formatstring);
    }
    return result;
}]);

/*###################### Factories ##################################*/
 
ShiftReportsApp.factory('accessController', function ($cookieStore, $location, $rootScope) {

    var authentificate = function () {

        if (angular.isUndefined($cookieStore.get('cachier_id'))) {
            // add tutorial page control
            if ($location.path().indexOf("tutorial") != -1) {
                $rootScope.showMenu = false;
                tutorialPath = $location.path();
                $location.path(tutorialPath);
            }
            else {
 
            $rootScope.showMenu = false;
            $location.path('/mobile-cash-business-login');
            }


        }
        else {
            $rootScope.showMenu = true;
            
        }
 
    };

 
    return {
        authentificate: authentificate
    };


});

/*#### Get shift informations from API and set them to LS*/
ShiftReportsApp.factory('ShiftInfo', function (LSOperation, Shift, $http, $cookieStore, $location, $rootScope, $timeout) {

    var set = function () {

        var shift;

        // get shift Time info
        return $http.get($rootScope.APIServerURL + "api/Shift/BusinessShiftsInfo?casheir_id=" + $cookieStore.get('cachier_id'), {
        }).success(function (APIData, status, headers, config) {
             

            // create Local storage if doesnt exist
            LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {

                console.log("returned data:" + APIData);
                if (data == null) {
                    
                    // if no shift found in local storage create one with the shift id from cookie
                    shift = new Shift();
                    shift.shiftID = $cookieStore.get('shift_id');
                    shift.shiftNO = $cookieStore.get('shift_no'); // get this from API
                    shift.cachierID = $cookieStore.get('cachier_id');
                    shift.shiftBeginEndTime = $cookieStore.get('shift_time'); // get this from API

                   // shift.SD_checklist = data.check; 
                    shift.shiftsInformations = APIData.shifts;
                    shift.SD_checklist = APIData.check;
                    // add a response to the checklist
                    angular.forEach(shift.SD_checklist, function (value, key) {
                        if (value.q_answer == null) {
                            if (value.q_type.toLowerCase() == "checkbox") {
                                value.q_answer = false;
                            }
                            else {
                                value.q_answer = "";
                            }
                        }
                    });


                    //## get Racksets Menu from API #####################
                    $rootScope.rackSetsMenu = [];
                    angular.forEach(APIData.store_racks_array.store_racks, function (rackset, key) {

                        $rootScope.rackSetsMenu.push({
                            "id": rackset.id,
                            "title": rackset.title
                            });

                    });
                    /* test menu
                    $rootScope.rackSetsMenu = [
                                    {
                                        "id": 1,
                                        "title": "Food",
                                    },
                                    {
                                        "id": 2,
                                        "title": "Chips",
                                    },
                                    {
                                        "id": 3,
                                        "title": "Coke",
                                    },
                                                ];
                                                */
                    $cookieStore.put('rackSetsMenu', $rootScope.rackSetsMenu); 



                    persistence.add(shift);
                    shift.markDirty('SD_checklist'); // special for JSON fields
                    shift.markDirty('shiftsInformations');
                    persistence.flush(function () { console.log("Shift created from Zero");}); // save to LS
                	// $location.path('/cash_open_drawer');
                    if (!$rootScope.is_simpleplan)
                    	$location.path('/cash_open_drawer');
                    else
                    	$location.path('/shift_details');

                    
                    
                }
                else {
                	// $location.path('/cash_open_drawer');
                	if ($rootScope.is_simpleplan)
                		$location.path('/cash_open_drawer');
                	else
                		$location.path('/shift_details');

                 }
            });


            return APIData;

        }).error(function (data, status, headers, config) {
            // TODO Error message can't connect to server 
            console.log("error");
        });

        
    };


    return {
        set: set
    };


});



 
 
ShiftReportsApp.factory('Shift', function ($q) {

  /**
  * The Constructor change to the persistence.js manner
  */
    var Shift = persistence.define('Shift', {
        shiftID :   'INT',
        shiftNO: 'INT', /* 1,2,3 ...*/
        cachierID: 'INT',
        shiftDate: 'TEXT', /* format 1/11/2016 */
        cachierName: 'TEXT',
        shiftTimeBegins:  'TEXT', /* format 2AM */
        shiftTimeEnds: 'TEXT', /* format 6AM */
        shiftBeginEndTime: 'TEXT', //  from create shift API
        /* Dynamic shifts*/
        shiftsInformations: 'JSON',
        /*## Opening Drawer prefix OC*/
        /* Bills prefix B */
        OC_B_Fifties: 'INT',
        OC_B_Twenties: 'INT',
        OC_B_Tens: 'INT',
        OC_B_Fives: 'INT',
        OC_B_Singles: 'INT',
        /* Loose Coins prefix L*/
        OC_L_Dollars:   'INT',
        OC_L_Quarters:   'INT',
        OC_L_Dimes:   'INT',
        OC_L_Nickels:   'INT',
        OC_L_Pennies:   'INT',
        /* Rolled Coins prefix R*/
        OC_R_Quarters:   'INT',
        OC_R_Dimes:   'INT',
        OC_R_Nickels:   'INT',
        OC_R_Pennies: 'INT',
        /*## Closing Drawer prefix CC*/
        /* Bills prefix B */
        CC_B_Fifties: 'INT',
        CC_B_Twenties: 'INT',
        CC_B_Tens: 'INT',
        CC_B_Fives: 'INT',
        CC_B_Singles: 'INT',
        /* Loose Coins prefix L*/
        CC_L_Dollars: 'INT',
        CC_L_Quarters: 'INT',
        CC_L_Dimes: 'INT',
        CC_L_Nickels: 'INT',
        CC_L_Pennies: 'INT',
        /* Rolled Coins prefix R*/
        CC_R_Quarters: 'INT',
        CC_R_Dimes: 'INT',
        CC_R_Nickels: 'INT',
        CC_R_Pennies: 'INT',
        /*## SafeDrops prefix SD*/
        SD_SafeDrops: 'JSON',
        /*## MOP SALES prefix MS*/
        /* Register 1 */
        MS_R1_Credit: 'TEXT',
        MS_R1_Debit: 'TEXT',
        MS_R1_Cash: 'TEXT',
        /* Register 2 */
        MS_R2_Credit: 'TEXT',
        MS_R2_Debit: 'TEXT',
        MS_R2_Cash: 'TEXT',
        /* Register 3 is all registers */
        MS_R3_Credit: 'TEXT',
        MS_R3_Debit: 'TEXT',
        MS_R3_Cash: 'TEXT',
        /*## Shift Details prefix SD*/
        /* Shift Time */
        SD_Shift_Opened_Time : 'INT',
        SD_Shift_Close_Time: 'INT',
        SD_checklist: 'JSON',
        /* Beginning your shift tasks prefix BG*/
        SD_BG_Count_Opening_CD: 'BOOL',
        SD_BG_Clean_Work_Area: 'BOOL',
        /* During your shift tasks prefix DR*/
        SD_DR_Make_Customers_Smile: 'BOOL',
        SD_DR_Sweep_Mop: 'BOOL',
        /* Closing your shift tasks prefix CL*/
        SD_CL_count_closing_CD: 'BOOL',
        SD_CL_Close_shift_register: 'BOOL',
        /* Rack Data*/
        RackSets: 'JSON' // array of rack sets

    });

    /* Table index */
    Shift.index(['shiftID'], { unique: true });
 
    persistence.schemaSync();
    return Shift;
   
});

/* return shift from storage if exist */
ShiftReportsApp.factory('LSOperation', function ($q,Shift) {

    var getShift = function (shiftID) {
        var deferred = $q.defer();
        var result = Shift.all().filter('shiftID', '=', shiftID);
        result.count(null, function (countResult) { // if no result found return null
            if (countResult == 0) { deferred.resolve(null); }
        });

        result.list(null, function (result) {

            result.forEach(function (t) {
                deferred.resolve(t); // return one row
            });

        });
        return deferred.promise;

    };
    
    return {
        getShift: getShift
    };

});
 
/* ############################# Routing ###################################### */
//Define Routing for app
ShiftReportsApp.config(['$routeProvider', '$locationProvider',

    function ($routeProvider  , $locationProvider  ) {

        /* Routing */
        $routeProvider.
        when('/mobile-cash-business-login-2', {
            templateUrl:  'views/login_signup.html',
            controller: 'LoginSignUpController'
        }).

        when('/mobile-cash-business-login', {
            templateUrl:  'views/login.html',
            controller: 'LoginController'
        }).
        when('/forgot-password', {
            templateUrl: 'views/forgot-password.html',
            controller: 'ForgotPasswordController'
        }).

        when('/select-shift', {
            templateUrl: 'views/create_shift.html',
            controller: 'CreateShiftController'
        }).

        when('/cash_open_drawer', {
            templateUrl: 'views/open_drawer.html',
            controller: 'OpenDrawerController'
        }).

        when('/cash_close_open', {
            templateUrl:  'views/close_drawer.html',
            controller: 'CloseDrawerController'
        }).

        when('/safe_drops', {
            templateUrl: 'views/safe_drops.html',
            controller: 'SafeDropsController'
        }).

        when('/sales_info', {
            templateUrl:  'views/sales_info.html',
            controller: 'SalesInfoController'
        }).

		  when('/shift_details', {
		  	templateUrl: 'views/shift_details.html',
		  	controller: 'ShiftDetailsController'
		  }).

        when('/closed_shift_details', {
            templateUrl: 'views/shift_details.html',
            controller: 'ShiftDetailsController'
        }).

        when('/review_and_submit', {
            templateUrl:  'views/review_submit.html',
            controller: 'ReviewSubmitController'
        }).
        when('/tutorial/:id', {
            templateUrl: 'views/tutorials/layout.html',
            controller: 'TutorialController'
        }).
        // scheduler Page
        when('/schedule', {
            templateUrl: 'views/scheduler/schedule.html',
            controller: 'ScheduleController'
        }).
        // Racks Page
        when('/rackset/:id', {
            templateUrl: 'views/racks/rack.html',
            controller: 'RacksController'
        }).

        otherwise({
            redirectTo: '/mobile-cash-business-login'
        });

       
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false 
        });
       
  
    }]).run(function ($rootScope, $cookieStore, accessController,$location,$route) {


      
        $rootScope.deviceDate = moment().format('dddd MMMM DD, YYYY');

        console.log("device date time: " + moment().format('YYYY-MM-DD HH:mm:ss'))

        accessController.authentificate();


        // setting globale variable from cookies
        if (!angular.isUndefined($cookieStore.get('cachier_name'))) {
        $rootScope.cashierName = $cookieStore.get('cachier_name');
        $rootScope.shiftTime = $cookieStore.get('shift_time');
        $rootScope.rackSetsMenu = $cookieStore.get('rackSetsMenu');  
      
        }
        $rootScope.currentDate = new Date();
        $rootScope.issimpleplan = true;

        persistence.store.websql.config(persistence, 'ShiftReportsDB10', 'Local storage database', 5 * 1024 * 1024);
        

        // bp server: //bp.shiftreports.com/manager_api
        // QA server: //qa.shiftreports.com/manager_api

        // config
        $rootScope.APIServerURL = "//qa.shiftreports.com/manager_api/";

        

        // global functions
        $rootScope.deActivateMenuButtons = function () {
            
            $(".menuButton").each(function (index) {
                $(this).removeClass("active");
            });

        }
        // global Functions deny user from typing special caracters
        $rootScope.$on('$routeChangeSuccess', function () { // forum validation when the ng-view is loaded
            $rootScope.$on('$viewContentLoaded', function ($evt, data) {
                validateForms();
            });

            $rootScope.inactiveLogout(); //inactive logout after 
        });

        // log user out after 30 minutes of inactivity
        $rootScope.inactiveLogout = function () {

            now = moment(new Date());
            console.log("inactive time: " + now.diff($rootScope.lastActiveTime, 'minutes'));
            if (now.diff($rootScope.lastActiveTime, 'minutes') >= 30) { // logout if inactive time superior or equal to 30
                if (!angular.isUndefined($cookieStore.get('cachier_name'))) {
                    $rootScope.logout();
                }
            }


            $rootScope.lastActiveTime = moment(new Date());

        };

        // logout
        $rootScope.logout = function () {
            $cookieStore.remove('shift_id');
            $cookieStore.remove('shift_no');
            $cookieStore.remove('shift_time');
            $cookieStore.remove('cachier_id');
            $cookieStore.remove('cachier_name');
            $cookieStore.remove('rackSetsMenu');
            $rootScope.cashierName = null;
            $rootScope.shiftTime = null;
            $location.path('/mobile-cash-business-login');
         
          
        };

        $rootScope.checkPaymentStatus = function (payment_status) {
         
            if (payment_status == "paid" ||
                payment_status == "trial_mode_active" ||
                payment_status == "trial_mode_expired_0_thru_14_days" ||
                payment_status == "past_due_0_days" ||
                payment_status == "past_due_1_days" ||
                payment_status == "past_due_2_days" ||
                payment_status == "past_due_7_days" ||
                payment_status == "past_due_14_days" ||
                payment_status == "trial_mode_expired_15_thru_30_days" ||
                payment_status == "past_due_21_days" ||
                payment_status == "past_due_30_days") {

                return 0;
            }
            else if (payment_status == "trial_mode_expired_past_30_days" || payment_status == "cancelled" || payment_status == "" ||
                     payment_status == "past_due_45_days" ||
                     payment_status == "past_due_60_days" ||
                     payment_status == "past_due_60_plus_days") {
                $rootScope.paymentStatusModalMessage = "Sorry, it appears that your account is currently suspended. If you have any questions regarding your account, please contact your manager,  call our Customer Service line at 888.670.4190, send us an email to: support@shiftreports.com requesting your account to be re-activated, or chat with us";
                $('#paymentStatusModal').modal('show');
            }
            return -1;//  0 ok,  -1 not ok
        };

    	// Plan Flag
        $rootScope.is_simpleplan=false;

        //## Rack set ###################################################################################
        // this need to get it from a session

        // setting globale variable from cookies
 

       /*
        $rootScope.rackSets = [
                {
                    "id": 1,
                    "title": "Food",
                },
                {
                    "id": 2,
                    "title": "Chips",
                },
                {
                    "id": 3,
                    "title": "Coke",
                },
                        ];
       */

});




angular.module('ShiftReportsApp.controllers', []);





