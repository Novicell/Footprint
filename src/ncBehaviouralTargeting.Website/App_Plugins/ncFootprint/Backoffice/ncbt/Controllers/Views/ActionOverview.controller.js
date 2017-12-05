angular.module('umbraco')
    .controller('ncFootprint.Backoffice.ActionOverview.Controller',
        function ($scope, $routeParams, $controller, $route, ncbtActionResource) {
            // Sync tree
            $scope.treeSyncPath = ['actionoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseOverview.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Actions');

            // -------------------------- Config --------------------------
            // Overwrite config
            angular.extend($scope.config, {
                idColumn: 'Id',
                sortColumn: 'Id',
                sortDescending: false
            });


            // -------------------------- Initial data fetching --------------------------
            // Get all segments
            ncbtActionResource.GetAllLight().then(function (response) {
                // Populate data source
                $scope.populateDataSource(response);
            });

            // Delete node
            $scope.deleteNode = function (nodeId) {
                ncbtActionResource.Delete(nodeId);
                $route.reload();
            };
        }
    );