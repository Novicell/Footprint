angular.module('umbraco')
    .controller('ncFootprint.Backoffice.QueryOverview.Controller',
        function ($scope, $routeParams, $controller, ncbtQueryResource, ncbtSegmentResource) {
            // Sync tree.
            $scope.treeSyncPath = ['queryoverview'];

            // Extend from base overview.
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseOverview.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            $scope.searchForm = (function () {
                // Segment-picker.
                return {
                    segment: {
                        segments: [],
                        selected: []
                    },

                    excludeIncompleteProfiles: true,
                    excludeNcbtProperties: false
                };
            }());
            $scope.searchResults = null;
            $scope.searching = false;

            var search = function (excludeIncompleteProfiles, excludeNcbtProperties, segmentAliases, page) {
                $scope.searching = true;

                ncbtQueryResource.Search(excludeIncompleteProfiles, excludeNcbtProperties, segmentAliases, page)
                    .then(function (result) {
                        var queryResult = result.data;

                        $scope.searchResults = {
                            page: page,
                            pages: Math.ceil(queryResult.Total / queryResult.Take),
                            headers: queryResult.Headers,
                            documents: queryResult.Documents,
                            excludeIncompleteProfiles: excludeIncompleteProfiles,
                            excludeNcbtProperties: excludeNcbtProperties,
                            segmentAliases: segmentAliases
                        };
                    }).always(function() {
                        $scope.searching = false;
                    });
            };

            $scope.searchDisabled = function () {
                return $scope.searching;
            };

            $scope.search = function () {
                var segmentAliases = [];

                if ($scope.searchForm.segment.selected) {
                    segmentAliases = $scope.searchForm.segment.selected;
                }

                search(
                    $scope.searchForm.excludeIncompleteProfiles,
                    $scope.searchForm.excludeNcbtProperties,
                    segmentAliases, 0);
            };

            $scope.seek = function (page) {
                var segmentAliases = [];

                if ($scope.searchResults.segmentAliases) {
                    segmentAliases = $scope.searchResults.segmentAliases;
                }

                search(
                    $scope.searchResults.excludeIncompleteProfiles,
                    $scope.searchResults.excludeNcbtProperties,
                    segmentAliases, page);
            };

            $scope.previousPageDisabled = function () {
                if ($scope.searching) {
                    return true;
                }

                if ($scope.searchResults.page === 0) {
                    return true;
                }

                return false;
            };

            $scope.previousPage = function () {
                $scope.seek($scope.searchResults.page - 1);
            };

            $scope.nextPageDisabled = function () {
                if ($scope.searching) {
                    return true;
                }

                if ($scope.searchResults.page === $scope.searchResults.pages - 1) {
                    return true;
                }

                return false;
            };

            $scope.nextPage = function () {
                $scope.seek($scope.searchResults.page + 1);
            };

            $scope.$parent.pushBreadcrumb('Visitors');

            $scope.toggleSegmentSelection = function (segmentAlias) {
                var idx = $scope.searchForm.segment.selected.indexOf(segmentAlias);

                if (idx > -1) {
                    $scope.searchForm.segment.selected.splice(idx, 1);
                }
                else {
                    $scope.searchForm.segment.selected.push(segmentAlias);
                }
            };

            $scope.exportUrl = function () {
                var segmentAliases = [];

                if ($scope.searchResults.segmentAliases) {
                    segmentAliases = $scope.searchResults.segmentAliases;
                }

                return ncbtQueryResource.GetEndpoint()
                    + "/ExportAsCsv"
                    + "?excludeIncompleteProfiles=" + $scope.searchResults.excludeIncompleteProfiles
                    + "&excludeNcbtProperties=" + $scope.searchResults.excludeNcbtProperties
                    + "&segmentAliases=" + segmentAliases
                    + "&page=" + $scope.searchResults.page
                    + "&cache=" + Date.now();
            };

            // Fill up select-list of segments.
            ncbtSegmentResource.GetAllLight()
                .then(function(result) {
                    for (var i = 0; i < result.data.length; i++) {
                        $scope.searchForm.segment.segments.push(result.data[i]);
                    }
                });
        }
    );
