ShiftReportsApp.controller('TutorialController', function ($scope, $http, $rootScope, $route, $cookieStore,$location) {
    
    $rootScope.showMenu = false;
 
    $scope.video = $route.current.params.id;
 
    $scope.page = "views/tutorials/" + $scope.video + ".html";

 


    $rootScope.videoUrl = null;

 
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


});