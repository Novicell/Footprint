angular.module('umbraco')
    .controller('ncFootprint.Backoffice.PropertyCreate.Controller',
        function ($scope, $routeParams, $controller, notificationsService, ncbtPropertyResource) {
            // Sync tree
            $scope.treeSyncPath = ['propertyoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseEdit.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Properties', 'propertyoverview');
            $scope.$parent.pushBreadcrumb('Create Property');

            // -------------------------- Config --------------------------
            // Set up tabs
            $scope.tabs = [
                { id: 1, label: "General" }
            ];

            // Set up data
            $scope.data = {
                node: {}
            };


            // -------------------------- Manipulation methods  --------------------------
            // Property methods
            $scope.saveProperty = function () {
                // Save property
                ncbtPropertyResource.Save($scope.data.node).then(function (response) {
                    if (response.data != null) {
                        // Redirect to edit page
                        var url = "#/ncbt/ncbt/base/propertyedit-id-" + response.data.Id;
                        window.location = url;
                    } else {
                        notificationsService.error("Error", "Could not save property.");
                    }
                });
            }
        }
    );