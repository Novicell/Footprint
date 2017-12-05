angular.module('umbraco')
    .controller('ncFootprint.Backoffice.SegmentCreate.Controller',
        function ($scope, $routeParams, $controller, notificationsService, ncbtSegmentResource) {
            // Sync tree
            $scope.treeSyncPath = ['segmentoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseEdit.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Segments', 'segmentoverview');
            $scope.$parent.pushBreadcrumb('Create Segment');

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
            // Segment methods
            $scope.saveSegment = function () {
                // Save segment
                ncbtSegmentResource.Save($scope.data.node).then(function (response) {
                    // Populate segment
                    if (response.data != null) {
                        // Redirect to edit page
                        var url = "#/ncbt/ncbt/base/segmentedit-id-" + response.data.Id;
                        window.location = url;
                    } else {
                        notificationsService.error("Error", "Could not save segment.");
                    }
                });
            }
        }
    );