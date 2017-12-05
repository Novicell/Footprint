angular.module('umbraco')
    .controller('ncFootprint.Backoffice.BaseOverview.Controller',
        function ($scope, $routeParams, navigationService) {

            // -------------------------- Tree Sync --------------------------
            if ($scope.treeSyncPath != undefined) {
                var treePath = ["-1"];
                $scope.treeSyncPath.forEach(function(path) {
                    treePath.push(path);
                });
                navigationService.syncTree({ tree: 'ncbt', path: treePath, forceReload: false });
            }

            // -------------------------- Routing --------------------------
            // Grab id if any
            $scope.routeId = $.isNumeric($routeParams.id) ? $routeParams.id : '';

            // -------------------------- Config --------------------------
            // Config
            $scope.config = {
                idColumn: 'Id',
                sortColumn: 'SortOrder',
                sortDescending: false,
                paginationItemsPerPage: 20,
                paginationCurrentPage: 1
            };

            // -------------------------- Pagination --------------------------
            // Init node structs
            $scope.nodes = [];
            $scope.visibleNodes = [];

            // Changes pagination page
            $scope.paginationChangedPage = function () {
                var sliceStart = $scope.config.paginationItemsPerPage * ($scope.config.paginationCurrentPage - 1);
                var sliceEnd = sliceStart + $scope.config.paginationItemsPerPage;
                $scope.visibleNodes = $scope.nodes.slice(sliceStart, sliceEnd);
            };
            // Add watch to pagination page changes
            $scope.$watch('config.paginationCurrentPage', function () {
                $scope.paginationChangedPage();
            });
            

            // Resets selections and collections
            $scope.reset = function() {
                $scope.selected = {};
                $scope.allSelected = false;
                $scope.loaded = false;
            };
            // Controls the content to be rendered in the html.
            $scope.populateDataSource = function (response) {
                $scope.nodes = response.data;
                $scope.loaded = true;

                // Reload pagination
                $scope.paginationCurrentPage = 1;
                $scope.paginationChangedPage();
            };
            // Updates the listing based on type-sensitive name of the column (Ie. "Name") and if the sort defaults as descending or ascending 
            $scope.setSort = function (columnName, isDescendantInitial) {
                if ($scope.config.sortColumn.equals(columnName)) {
                    // Same column, flip sorting
                    $scope.config.sortDescending = !$scope.config.sortDescending;
                } else {
                    $scope.config.sortDescending = isDescendantInitial;
                }
                $scope.config.sortColumn = columnName;
            };

            // Initialize by reset
            $scope.reset();
        }
    );