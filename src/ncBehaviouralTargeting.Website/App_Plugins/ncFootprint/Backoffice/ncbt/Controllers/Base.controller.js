angular.module('umbraco').requires.push('ui.bootstrap');
angular.module('umbraco').requires.push('googlechart');
angular.module('umbraco').requires.push('hljs');

angular.module("umbraco")
    .controller("ncFootprint.Backoffice.Base.Controller",
        function($rootScope, $scope, $routeParams, $location, navigationService) {

            navigationService.hideSearch();

            // ---------------------- Breadcrumbs ----------------------
            $scope.resetBreadcrumbs = function () {
                $scope.breadcrumbs = [
                    {
                        text: 'Footprint',
                        url: '#/ncbt'
                    }
                ];
            };
            $scope.resetBreadcrumbs();

            $scope.pushBreadcrumb = function (text, view, isLocal) {
                isLocal = isLocal == undefined ? true : isLocal;
                var url = view == undefined ? $location.url() : '/ncbt/ncbt/base/' + view;

                $scope.breadcrumbs.push({
                    text: text,
                    url: (isLocal ? '#' : '') + url
                });
            }

            $scope.popBreadcrumb = function () {
                return $scope.breadcrumbs.pop();
            }

            $scope.shiftBreadcrumb = function () {
                return $scope.breadcrumbs.shift();
            }

            // ---------------------- Partial selection ----------------------
            var viewName = $routeParams.id;
            var viewId = viewName.split("-id-");

            if (viewId.length == 2) {
                viewName = viewId[0];
                $routeParams.id = viewId[1];
            } else {
                viewName = viewId;
                $routeParams.id = undefined;
            }
            $scope.templatePartialURL = '../App_Plugins/ncFootprint/Backoffice/ncbt/Views/' + viewName + '.html';
        });