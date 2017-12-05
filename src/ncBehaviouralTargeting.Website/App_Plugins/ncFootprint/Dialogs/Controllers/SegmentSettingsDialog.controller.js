angular.module('umbraco')
    .controller('Novicell.Footprint.Dialogs.SegmentSettingsDialog.Controller',
        function ($scope, localizationService, ncbtSegmentResource) {

            $scope.segments = [];

            // Get segment by alias
            $scope.getSegmentByAlias = function (segmentAlias) {
                return $scope.segments.filter(function (obj) {
                    return obj.Alias === segmentAlias;
                })[0];
            };

            // Load segments
            ncbtSegmentResource.GetAllLight().then(function (response) {
                $scope.segments = response.data;
            });

            // Filter inactive segments
            $scope.filterInactiveSegments = function(value, index) {
                return $scope.model.filter(function (obj) {
                    return obj.alias === value.Alias;
                }).length === 0;
            };

            // Add new segment by alias
            $scope.addSegmentByAlias = function (segmentAlias) {
                // Add segment
                $scope.model.push({
                    alias: segmentAlias,
                    sortOrder: $scope.model.length
                });
                // Update sort orders
                $scope.updateSortOrders();
            };

            // Remove segment by alias
            $scope.removeSegmentByAlias = function (segmentAlias) {
                var segment = $scope.model.filter(function(obj) {
                    return obj.alias === segmentAlias;
                })[0];
                if (segment != undefined) {
                    $scope.model.splice($scope.model.indexOf(segment), 1);
                    $scope.updateSortOrders();
                }
            };

            // Mirror passed data, cut away default segment, sort by sort order
            $scope.model = angular.copy($scope.dialogData.segments).filter(function (obj) {
                return obj.alias !== 'default';
            }).sort(function (a, b) {
                return a.sortOrder - b.sortOrder;
            });

            // Sort model by sortOrder
            $scope.updateSortOrders = function() {
                // Update sort orders
                $scope.model.forEach(function (segment, index) {
                    segment.sortOrder = index;
                });
            };
            
            // Set sortable options
            $scope.sortableOptions = {
                handle: ".icon-navigation",
                axis: "y",
                delay: 150,
                distance: 5,
                stop: function (ev, ui) {
                    // Update sort orders
                    $scope.updateSortOrders();
                }
            };
        });