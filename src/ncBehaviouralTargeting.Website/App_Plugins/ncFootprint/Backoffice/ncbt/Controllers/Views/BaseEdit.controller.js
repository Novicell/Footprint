angular.module('umbraco')
    .controller('ncFootprint.Backoffice.BaseEdit.Controller',
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
            // Set up sortable config
            $scope.sortableOptions = {
                handle: ".icon-navigation",
                axis: "y",
                delay: 150,
                distance: 5
            };
        }
    );