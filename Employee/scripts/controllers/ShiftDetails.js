ShiftReportsApp.controller('ShiftDetailsController', function ($scope, $http, Shift, $rootScope, LSOperation, $cookieStore, accessController,$location) {

    accessController.authentificate();
   
    
    console.log("id shift from cookies" + $cookieStore.get('shift_id'));

    LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {

        if (data != null) {
            console.log("Shift found in LS loading data");
            $scope.shift = data;

            // form validation
            $scope.$on('$viewContentLoaded', setTimeout(function () {
                validateForms()
            }, 2000));

            //console.log($scope.shift.SD_checklist);
 
            //console.log(JSON.stringify($scope.shift.SD_checklist));

            /* moved to get info in the app.js
            angular.forEach(data.SD_checklist, function (value, key) {
                if (value.q_answer == null) {
                    if (value.q_type.toLowerCase() == "checkbox") {
                        value.q_answer = false;
                    }
                    else {
                        value.q_answer = "";
                    }
                }
            });
        */
        }
 
    });



    $scope.updateField = function () {

        console.log("shift_id before adding" + $scope.shift.shift_id);
        persistence.add($scope.shift); // Modify shift  

        $scope.shift.markDirty('SD_checklist'); // special for JSON fields

        // commit changes.
        persistence.flush(function () {
            // commit done
            console.debug('Done Adding shift  shif_id=' + $scope.shift.shiftID);
        });

    };


	/////                Added by hytham to enable the scheduler


    $scope.createJsonSubmitRequest = function () {


    	console.log("Create json submit request");


    	$scope.dataToSubmit = [{}];

    	// Shift_info
    	$scope.dataToSubmit[0].shift_info = [{
    		"shift_id": $scope.shift.shiftID,
    		"cashier_id": $scope.shift.cachierID,
    		"shift_no": $scope.shift.shiftNO,
    		"shift_opened": $scope.shift.SD_Shift_Opened_Time,
    		"shift_closed": $scope.shift.SD_Shift_Close_Time
    	}];

    	// Open Drawer
    	$scope.dataToSubmit[0].drawer_open_mst = [{
    		"fifties": $scope.shift.OC_B_Fifties,
    		"twenties": $scope.shift.OC_B_Twenties,
    		"tens": $scope.shift.OC_B_Tens,
    		"fives": $scope.shift.OC_B_Fives,
    		"singles": $scope.shift.OC_B_Singles,
    		"dollars": $scope.shift.OC_L_Dollars,
    		"quarters": $scope.shift.OC_L_Quarters,
    		"dimes": $scope.shift.OC_L_Dimes,
    		"nickels": $scope.shift.OC_L_Nickels,
    		"pennies": $scope.shift.OC_L_Pennies,
    		"rolled_quarters": $scope.shift.OC_R_Quarters,
    		"rolled_dimes": $scope.shift.OC_R_Dimes,
    		"rolled_nickels": $scope.shift.OC_R_Nickels,
    		"rolled_pennies": $scope.shift.OC_R_Pennies
    	}];

    	// Close Drawer
    	$scope.dataToSubmit[0].drawer_close_mst = [{
    		"fifties": $scope.shift.CC_B_Fifties,
    		"twenties": $scope.shift.CC_B_Twenties,
    		"tens": $scope.shift.CC_B_Tens,
    		"fives": $scope.shift.CC_B_Fives,
    		"singles": $scope.shift.CC_B_Singles,
    		"dollars": $scope.shift.CC_L_Dollars,
    		"quarters": $scope.shift.CC_L_Quarters,
    		"dimes": $scope.shift.CC_L_Dimes,
    		"nickels": $scope.shift.CC_L_Nickels,
    		"pennies": $scope.shift.CC_L_Pennies,
    		"rolled_quarters": $scope.shift.CC_R_Quarters,
    		"rolled_dimes": $scope.shift.CC_R_Dimes,
    		"rolled_nickels": $scope.shift.CC_R_Nickels,
    		"rolled_pennies": $scope.shift.CC_R_Pennies
    	}];


    	// Safe Drops
    	if ($scope.shift.SD_SafeDrops == null) {
    		$scope.dataToSubmit[0].safe_drops = [];
    	}
    	else {
    		$scope.dataToSubmit[0].safe_drops = JSON.parse(angular.toJson($scope.shift.SD_SafeDrops));
    	}

    	// shift session time
    	$scope.dataToSubmit[0].shift_details_mst = {
    		/* "shift_session_started": moment($cookieStore.get('shift_session_started')).format('HHmm'),*/
    		"shift_session_ended": moment().format('HHmm'),
    		/*"shift_date": moment($cookieStore.get('shift_session_started')).format('MM/DD/YYYY')*/
    	};


    	// MOP Sales
    	if ($scope.shift.MS_R1_Credit == "") $scope.shift.MS_R1_Credit = "0.00";
    	if ($scope.shift.MS_R1_Debit == "") $scope.shift.MS_R1_Debit = "0.00";
    	if ($scope.shift.MS_R1_Cash == "") $scope.shift.MS_R1_Cash = "0.00";

    	if ($scope.shift.MS_R2_Credit == "") $scope.shift.MS_R2_Credit = "0.00";
    	if ($scope.shift.MS_R2_Debit == "") $scope.shift.MS_R2_Debit = "0.00";
    	if ($scope.shift.MS_R2_Cash == "") $scope.shift.MS_R2_Cash = "0.00";

    	if ($scope.shift.MS_R3_Credit == "") $scope.shift.MS_R3_Credit = "0.00";
    	if ($scope.shift.MS_R3_Debit == "") $scope.shift.MS_R3_Debit = "0.00";
    	if ($scope.shift.MS_R3_Cash == "") $scope.shift.MS_R3_Cash = "0.00";

    	persistence.add($scope.shift); persistence.flush(function () { }); // apply change to LS

    	$scope.dataToSubmit[0].mop_sales = [
			{
				"_id": 1,
				"label": "REGISTER 1",
				"credit": $scope.shift.MS_R1_Credit,
				"debit": $scope.shift.MS_R1_Debit,
				"cash": $scope.shift.MS_R1_Cash
			},
			{
				"_id": 2,
				"label": "REGISTER 2",
				"credit": $scope.shift.MS_R2_Credit,
				"debit": $scope.shift.MS_R2_Debit,
				"cash": $scope.shift.MS_R2_Cash
			},
			{
				"_id": 0, /* id 0 for all registers*/
				"label": "ALL REGISTERS",
				"credit": $scope.shift.MS_R3_Credit,
				"debit": $scope.shift.MS_R3_Debit,
				"cash": $scope.shift.MS_R3_Cash
			}];

    	// check list
    	$scope.dataToSubmit[0].shift_checklist_mst = [] // init check list

    	angular.forEach($scope.shift.SD_checklist, function (value, key) {

    		if (value.shift_no == $scope.shift.shiftNO) {   // return only question for that shift_no


    			var checkListObj = new Object();
    			checkListObj.q_id = value.q_id;
    			checkListObj.q_type = value.q_type;
    			checkListObj.q_uuid = value.q_uuid;

    			if (angular.isUndefined(value.q_answer)) {

    				if (value.q_type === "checkbox") {

    					checkListObj.q_answer = false;

    				}
    				else if (value.q_type === "textbox") {
    					checkListObj.q_answer = "";

    				}

    			}
    			else {
    				checkListObj.q_answer = value.q_answer;

    			}

    			console.log($scope.checkListObj);
    			$scope.dataToSubmit[0].shift_checklist_mst.push(checkListObj);


    		}
    	});


    	//## Racksets


    	//## Create Rackset Output json ###############################################################################################
    	var rackSets = [];

    	angular.forEach($scope.shift.RackSets, function (rackset, racksetKey) {
    		var rack_set = new Object();
    		rack_set.racks_vals = [];
    		rack_set.shift_racks = [];

    		rack_set.rackset_id = rackset.id;

    		//## get Elements
    		//## get header elements
    		angular.forEach(rackset.header.Sections, function (section, hSectionKey) {
    			angular.forEach(section.elemnts, function (headerElement, hElementKey) {
    				rack_set.racks_vals.push({
    					"elem_id": headerElement.id,
    					"elem_uuid": headerElement.uuid,
    					"value": headerElement.def
    				});
    			});

    		});
    		//## end get header elements

    		//## get footer elements
    		angular.forEach(rackset.footer.Sections, function (section, hSectionKey) {
    			angular.forEach(section.elemnts, function (footerElement, hFooterKey) {
    				rack_set.racks_vals.push({
    					"elem_id": footerElement.id,
    					"elem_uuid": footerElement.uuid,
    					"value": footerElement.def
    				});
    			});

    		});
    		//## end get footer elements
    		//## end getting elements

    		//## Getting rack values
    		angular.forEach(rackset.rack, function (rack, rackKey) {

    			angular.forEach(rack.cell, function (row, rowKey) {

    				angular.forEach(row, function (col, colKey) {

    					rack_set.shift_racks.push(
							{
								"rackset_id": rackset.id,
								"rack_no": rack.rack_no,
								"row_no": rowKey,
								"col_no": colKey,
								"added_val": col.added_val,
								"started_val": col.started_val,
								"ended_val": col.ended_val
							});

    				});// cols loop
    			});// rows loop
    		});// rack loop


    		rackSets.push(rack_set);
    	});

    	$scope.dataToSubmit[0].rack_set = rackSets;

    	//## end #################################################################################################################


    };



    $scope.submit = function () {

    	// creating the submit json
    	$scope.createJsonSubmitRequest();

    	// console.log("shift id: " + $scope.shift.shiftID);


    	// $scope.stringJson = JSON.stringify($scope.dataToSubmit);

    	console.log(angular.toJson($scope.dataToSubmit));


    	$http.post($rootScope.APIServerURL + "api/Shift/SubmitShiftReport_cashier", {
    		shiftkey: $scope.shift.shiftID,
    		report: angular.toJson($scope.dataToSubmit)
    	}).success(function (data, status, headers, config) {

    		if (data.result == "success") {
    			persistence.remove($scope.shift);
    			$cookieStore.remove('shift_id');
    			$cookieStore.remove('shift_no');
    			$cookieStore.remove('shift_time');
    			$cookieStore.remove('cachier_id');
    			$cookieStore.remove('cachier_name');
    			$cookieStore.remove('rackSetsMenu');
    			$rootScope.cashierName = null;
    			$rootScope.shiftTime = null;
    			$rootScope.shiftSuccessfullySubmited = true;
    			$location.path('/mobile-cash-business-login');

    		}


    	}).error(function (data, status, headers, config) {
    		// TODO Error message can't connect to server 
    		$('#errorSubmitModal').modal('show');
    	});




    	// console.log($scope.dataToSubmit);

    	// alert(JSON.stringify($scope.dataToSubmit));


    };







	/////////////////////////////////////////////

});