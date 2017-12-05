angular.module('umbraco')
    .controller('ncFootprint.Backoffice.ActionCreate.Controller',
        function ($scope, $routeParams, $controller, notificationsService, ncbtActionResource) {
            // Sync tree
            $scope.treeSyncPath = ['actionoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseEdit.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Actions', 'actionoverview');
            $scope.$parent.pushBreadcrumb('Create Action');

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
            // Action methods
            $scope.saveAction = function () {
                // Save Action
                ncbtActionResource.Save($scope.data.node).then(function (response) {
                    // Populate Action
                    if (response.data != null) {
                        // Redirect to edit page
                        var url = "#/ncbt/ncbt/base/actionedit-id-" + response.data.Id;
                        window.location = url;
                    } else {
                        notificationsService.error("Error", "Could not save action.");
                    }
                });
            }
        }
    );