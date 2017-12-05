angular.module('umbraco')
    .controller('ncFootprint.Backoffice.PropertyEdit.Controller',
        function ($scope, $routeParams, $controller, notificationsService, dialogService, ncbtPropertyResource, ncbtSegmentUtilityResource) {
            // Sync tree
            $scope.treeSyncPath = ['propertyoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseEdit.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Properties', 'propertyoverview');

            // -------------------------- Config --------------------------
            // Set up tabs
            $scope.tabs = [
                { id: 1, label: "General" }
            ];

            // Set up data
            $scope.data = {
                showTips: true,
                dataTypes: [],
                node: null
            };


            // -------------------------- Manipulation methods  --------------------------
            // Property methods
            $scope.loadProperty = function (property) {
                $scope.data.node = property;

                // Push breadcrumb
                $scope.$parent.pushBreadcrumb('Edit Property - ' + property.DisplayName);
            }

            $scope.saveProperty = function () {
                // Save property
                ncbtPropertyResource.Save($scope.data.node).then(function (response) {
                    // Populate segment
                    if (response.data != null) {
                        $scope.loadProperty(response.data);
                        notificationsService.success("Success", "Property saved.");
                    } else {
                        notificationsService.error("Error", "Could not save property.");
                    }
                });
            }

            // Data type methods
            $scope.loadDataTypes = function (dataTypeList) {
                $scope.data.dataTypes = dataTypeList;
            }

            $scope.getDataType = function (dataTypeId) {
                return $scope.data.dataTypes[dataTypeId];
            };


            // -------------------------- Initial data fetching --------------------------
            // Fetch all data types
            ncbtSegmentUtilityResource.GetDataTypes().then(function (response) {
                // Save data types
                $scope.loadDataTypes(response.data);
            });

            // Fetch current property
            ncbtPropertyResource.GetById($routeParams.id).then(function (response) {
                // Populate property
                $scope.loadProperty(response.data);
            });
        }
    );