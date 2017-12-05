angular.module('umbraco')
    .controller('ncFootprint.Backoffice.OverviewDashboard.Controller',
        function ($scope, $routeParams, $controller, ncbtDashboardResource) {
            // Extend from base dashboard
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseDashboard.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            $scope.content = "";

            ncbtDashboardResource.Get()
                .then(function (result) {
                    $scope.content = result.data.content;
                });
        }
    );