angular.module('umbraco')
    .controller('ncFootprint.Backoffice.PropertyOverview.Controller',
        function ($scope, $routeParams, $controller, $route, ncbtPropertyResource, ncbtSegmentUtilityResource) {
            // Sync tree
            $scope.treeSyncPath = ['propertyoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseOverview.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Properties');

            // -------------------------- Config --------------------------
            // Overwrite config
            angular.extend($scope.config, {
                idColumn: 'Id',
                sortColumn: 'Id',
                sortDescending: false
            });

            // Set up data
            $scope.data = {
                dataTypes: []
            };


            // -------------------------- Manipulation methods  --------------------------
            // Data type methods
            $scope.loadDataTypes = function (dataTypeList) {
                $scope.data.dataTypes = dataTypeList;
            }

            $scope.getDataType = function (dataTypeId) {
                return $scope.data.dataTypes[dataTypeId];
            };


            // -------------------------- Initial data fetching --------------------------
            // Get all segments
            ncbtPropertyResource.GetAllLight().then(function (response) {
                // Populate data source
                $scope.populateDataSource(response);
            });

            // Fetch all data types
            ncbtSegmentUtilityResource.GetDataTypes().then(function (response) {
                // Save data types
                $scope.loadDataTypes(response.data);
            });

            // Delete node
            $scope.deleteNode = function (nodeId) {
                ncbtPropertyResource.Delete(nodeId);
                $route.reload();
            };
        }
    );