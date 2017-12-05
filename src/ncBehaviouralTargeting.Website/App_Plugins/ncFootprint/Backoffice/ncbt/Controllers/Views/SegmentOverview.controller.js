angular.module('umbraco')
    .controller('ncFootprint.Backoffice.SegmentOverview.Controller',
        function ($scope, $routeParams, $controller, $route, ncbtSegmentResource) {
            // Sync tree
            $scope.treeSyncPath = ['segmentoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseOverview.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Segments');

            // -------------------------- Config --------------------------
            // Overwrite config
            angular.extend($scope.config, {
                idColumn: 'Id',
                sortColumn: 'Id',
                sortDescending: false
            });


            // -------------------------- Initial data fetching --------------------------
            // Get all segments
            ncbtSegmentResource.GetAllLight().then(function (response) {
                // Populate data source
                $scope.populateDataSource(response);
            });

            // Delete node
            $scope.deleteNode = function (nodeId) {
                ncbtSegmentResource.Delete(nodeId);
                $route.reload();
            };
        }
    );