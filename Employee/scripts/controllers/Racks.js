ShiftReportsApp.controller('RacksController', function ($scope, $http, Shift, $rootScope, LSOperation,$location, $cookieStore,$route) {
    $rootScope.showMenu = false;

    $scope.rackset_ID=$route.current.params.id
     
    
 
    //## Get Rackset Data from LS #################################################################################################
    LSOperation.getShift($cookieStore.get('shift_id')).then(function (data) {
        if (data != null) {
            $scope.shift = data;
            
            // check if rackset is in LS if not get it from API
            if ($scope.shift.RackSets == null) { // Not in LS get primitive data from API

                $http.get($rootScope.APIServerURL + 'api/Racks/get_rack_sets_canvas?cashier_id=' + $cookieStore.get('cachier_id')).success(function (APIData) {
                    $scope.createRackSet(APIData.rackse.store_racks);
                   // $scope.initSelectedRackSet();// select a rackset

                    
                    // store the New format of the rack set to the LS
                    $scope.shift.RackSets = $scope.racksets;
                    $scope.shift.markDirty('RackSets');
                    persistence.add($scope.shift);
                    // commit changes.
                    persistence.flush(function () { });
                     

                });



                /* Test Json
                $http.get('jsons/racksets.json').success(function (APIData) {
                    $scope.createRackSet(APIData);
                    $scope.initSelectedRackSet();// select a rackset

                    // store the New format of the rack set to the LS
                    $scope.shift.RackSets =  $scope.racksets;
                    $scope.shift.markDirty('RackSets');
                    persistence.add($scope.shift);
                    // commit changes.
                    persistence.flush(function () {});

                });
                */

            }
            else { // get Data From LS
                $scope.racksets = $scope.shift.RackSets// init from LS
                console.log($scope.racksets);
               // $scope.initSelectedRackSet();// select a rackset
            }
        }

    });



    //## end  #####################################################################################################################

    //## Create The rackset 
    $scope.createRackSet = function (tmpRackSet) {
        $scope.racksets = tmpRackSet;

        angular.forEach($scope.racksets, function (rackset, racksetKey) { // loop overRackset

            angular.forEach(rackset.rack, function (rack, rackKey) { // loop over racks
                console.log("row:" + rack.rack_row + "col: " + rack.rack_col);
                cell = [];
                for (i = 0; i < rack.rack_row; i++) { // loop over rack rows
                    cell[i] = [];
                    for (v = 0; v < rack.rack_col; v++) { // creating the cells
                        cell[i][v] = {
                            "added_val": "",
                            "started_val": "",
                            "ended_val": "",
                            "product_name": "",
                            "row_no": i,
                            "col_no": v
                        };
                    }
                }
                
                angular.forEach(rack.product_label, function (product, key) {

                    cell[product.row_no][product.col_no].product_name = product.product_name;

 
                });


                rack.cell = cell;

            });

        });


        console.log($scope.racksets);
    };

    //## create a list of alphabet
    $scope.alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.split('');
  


   


    //## Rack Navigation ##########################################################################################################
    $scope.selectedRack = 0;
    $scope.previousRack = function () {
        if ($scope.selectedRack > 0) {
            $scope.selectedRack = $scope.selectedRack - 1;
        }
        
    };
  
    $scope.nextRack = function (rackset) {
 
        if ($scope.selectedRack < rackset.rack.length - 1) {
            $scope.selectedRack = $scope.selectedRack + 1;
        }
        

    };
 
    //## Update Racksets Values ####################################################################################################

    $scope.updateRackSetValues = function () {
        console.log($scope.racksets);
        // store Modified rack set to the LS
        $scope.shift.RackSets = $scope.racksets;
        $scope.shift.markDirty('RackSets');
        persistence.add($scope.shift);
        // commit changes.
        persistence.flush(function () { });


    };



 
    $scope.generateOutPut = function () {


//## Create Rackset Output json ###############################################################################################
        var rackSets = [];
      
        angular.forEach($scope.racksets, function (rackset, racksetKey) {
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

                angular.forEach(rack.cell, function (row ,rowKey) {

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

        console.log(rackSets);

 //## end #################################################################################################################
    };

    /*
    {
  "rackset_col": [
    {
      "rackset_id": "",
      "racks_vals": [
        {
          "elem_id": 0,
          "elem_uuid": "",
          "value": 0,
        }
      ],
      shift_racks: [
        {
          "rack_no": 0,
          "row_no": 0,
          "col_no": 0,
          "started_val": 0.0,
          "added_val": 0.0,
          "ended_val": 0.0
        }
      ]
    }
  ]
}

    
    */
 



    //####################################
    $scope.goBackToTheApp = function () {
    
        if (!angular.isUndefined($cookieStore.get('shift_id'))) {

            shift_id = $cookieStore.get('shift_id');

            if (shift_id != -1) {
                $location.path("/cash_open_drawer");
            }
            else {
                $location.path("/select-shift");
            }

        }
    };



    // jquery

    $(".nav-tabs").delegate("a", "click", function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
 

});