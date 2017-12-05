angular.module('umbraco')
    .controller('ncFootprint.Backoffice.SettingsOverview.Controller',
        function ($scope, $routeParams, $controller) {
            // Sync tree
            $scope.treeSyncPath = ['settingsoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseOverview.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Settings');

            // -------------------------- Config --------------------------

            // -------------------------- Initial data fetching --------------------------

        }
    );