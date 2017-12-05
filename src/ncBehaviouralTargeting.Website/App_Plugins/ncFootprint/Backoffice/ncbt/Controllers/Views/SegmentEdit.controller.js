angular.module('umbraco')
    .controller('ncFootprint.Backoffice.SegmentEdit.Controller',
        function($scope, $routeParams, $controller, notificationsService, dialogService, ncbtSegmentResource, ncbtSegmentUtilityResource, ncbtSegmentOperatorResource) {
            // Sync tree
            $scope.treeSyncPath = ['segmentoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseEdit.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Segments', 'segmentoverview');

            // -------------------------- Config --------------------------
            // Set up tabs
            $scope.tabs = [
                { id: 1, label: "General" },
                { id: 2, label: "Criteria" }
            ];

            // Set up defaults
            $scope.defaults = {
                criterionGroup: {
                    Id: 0,
                    SegmentId: 0,
                    IsInclude: true,
                    SortOrder: 0,
                    Criterions: []
                },
                criterion: {
                    Id: 0,
                    CriterionGroupId: 0,
                    OperatorId: 0,
                    PropertyAlias: null,
                    PropertyValue: '',
                    SortOrder: 0,
                    Operator: null
                },
                operator: {
                    Id: 0,
                    DisplayName: null,
                    IsInverted: false,
                    OperatorType: null,
                    DataType: null
                },
                attribute: {
                    Id: 0,
                    Alias: null,
                    DisplayName: null,
                    DataType: null,
                    Description: null,
                    Examples: null
                }
            }

            $scope.fallbacks = {
                dataType: 'Unknown',
                property: {
                    Alias: null,
                    DisplayName: "Unregistered attribute '%ALIAS%'",
                    Description: "Register property '%ALIAS%' to set description",
                    Examples: "Register property '%ALIAS%' to set examples",
                    DataType: null,
                    Id: null
                }
            };

            // Set up data
            $scope.data = {
                showTips: true,
                chart: {},
                properties: [],
                operators: [],
                dataTypes: [],
                node: null
            };

            // -------------------------- Manipulation methods  --------------------------
            // Segment methods
            $scope.loadSegment = function (segment) {
                $scope.data.node = segment;

                // Push breadcrumb
                $scope.$parent.pushBreadcrumb('Edit Segment - ' + segment.DisplayName);
            }

            $scope.saveSegment = function () {
                // Convert strings to ints
                var node = angular.copy($scope.data.node);
                node.Persistence = node.Persistence * 1;

                // Save segment
                ncbtSegmentResource.Save(node).then(function (response) {
                    // Populate segment
                    if (response.data != null) {
                        $scope.loadSegment(response.data);
                        notificationsService.success("Success", "Segment saved.");
                    } else {
                        notificationsService.error("Error", "Could not save segment.");
                    }
                });
            }

            // Data type methods
            $scope.loadDataTypes = function (dataTypeList) {
                $scope.data.dataTypes = dataTypeList;
            }

            $scope.getDataType = function (dataTypeId) {
                var result = $scope.data.dataTypes[dataTypeId];
                if (result == null) {
                    result = $scope.fallbacks.dataType;
                }
                return result;
            };

            // Operator methods
            $scope.loadOperators = function (operatorList) {
                $scope.data.operators = operatorList;
            }

            $scope.getOperator = function (operatorId) {
                return $scope.data.operators.filter(function (obj) {
                    return obj.Id === operatorId;
                })[0];
            };

            // Property methods
            $scope.loadProperties = function(propertyList) {
                $scope.data.properties = propertyList;
            };

            $scope.getProperty = function (propertyAlias) {
                var result = $scope.data.properties.filter(function(obj) {
                    return obj.Alias === propertyAlias;
                })[0];
                if (result == null) {
                    result = angular.copy($scope.fallbacks.property);
                    result.Alias = propertyAlias;
                    result.DisplayName = result.DisplayName.replace('%ALIAS%', propertyAlias);
                    result.Description = result.Description.replace('%ALIAS%', propertyAlias);
                    result.Examples = result.Examples.replace('%ALIAS%', propertyAlias);
                }
                return result;
            };

            // Pie chart methods
            $scope.updatePieChart = function (numberOfmatchingVisitors, totalNumberOfVisitors) {
                $scope.userShare[1].v = numberOfmatchingVisitors;
                $scope.userShareInverse[1].v = totalNumberOfVisitors - numberOfmatchingVisitors;
            };

            function getMatchingVisitors() {
                ncbtSegmentResource.GetNumberOfVisitors($scope.data.node).then(function (response) {
                    $scope.updatePieChart(response.data.numberOfmatchingVisitors, response.data.totalNumberOfVisitors);
                });
            };

            // Criterion group methods
            $scope.addCriterionGroup = function() {
                // Make new criterion group
                var criterionGroup = angular.copy($scope.defaults.criterionGroup);
                criterionGroup.SegmentId = $scope.data.node.Id;

                $scope.data.node.CriterionGroups.push(criterionGroup);
            }

            $scope.removeCriterionGroup = function (criteriaGroup) {
                var index = $scope.data.node.CriterionGroups.indexOf(criteriaGroup);
                if (index > -1) {
                    $scope.data.node.CriterionGroups.splice(index, 1);
                }
            };

            // Criterion methods
            $scope.addCriterion = function (criteriaGroup) {
                // Make new criterion
                var criterion = angular.copy($scope.defaults.criterion);
                criterion.CriterionGroupId = criteriaGroup.Id;
                // Assign first available property type
                $scope.setCriterionProperty(criterion, $scope.data.properties[0].Alias);

                criteriaGroup.Criterions.push(criterion);
            };

            $scope.removeCriterion = function(criteriaGroup, criterion) {
                var index = criteriaGroup.Criterions.indexOf(criterion);
                if (index > -1) {
                    criteriaGroup.Criterions.splice(index, 1);
                }
            };

            $scope.setCriterionOperator = function (criterion, operator) {
                // Skip if we picked the same
                if (criterion.OperatorId === operator.Id) {
                    return;
                }
                criterion.OperatorId = operator.Id;
                criterion.Operator = operator;
            };

            $scope.setCriterionProperty = function (criterion, propertyAlias) {
                // Skip if we picked the same
                if (criterion.PropertyAlias === propertyAlias) {
                    return;
                }
                // Set property and reset value
                criterion.PropertyAlias = propertyAlias;
                criterion.PropertyValue = null;

                // Pick first available operator
                var dataType = $scope.getProperty(propertyAlias).DataType;
                var operator = $scope.data.operators.filter(function (obj) {
                    return obj.DataType === dataType;
                })[0];
                $scope.setCriterionOperator(criterion, operator);
            };

            // Special criterion methods
            $scope.btnSelectNode = function (criterion) {
                // Open dialog to let the user select a property
                dialogService.contentPicker({
                    multipicker: false,
                    callback: function (data) {
                        if (data != undefined) {
                            criterion.pageName = data.name;
                            criterion.PropertyValue = data.id;
                        }
                    }
                });
            };
            

            // -------------------------- Initial data fetching --------------------------
            // Fetch current segment
            ncbtSegmentResource.GetById($routeParams.id).then(function (response) {
                // Populate segment
                $scope.loadSegment(response.data);
            });

            // Fetch all operators
            ncbtSegmentOperatorResource.GetAllLight().then(function (response) {
                // Save operators
                $scope.loadOperators(response.data);
            });

            // Fetch all properties
            ncbtSegmentUtilityResource.GetProperties().then(function (response) {
                // Save properties
                $scope.loadProperties(response.data);
            });

            // Fetch all data types
            ncbtSegmentUtilityResource.GetDataTypes().then(function (response) {
                // Save data types
                $scope.loadDataTypes(response.data);
            });


            // -------------------------- Chart --------------------------
            $scope.userShare = [
                { v: "Share of users" },
                { v: 0 }
            ];
            $scope.userShareInverse = [
                { v: "Share of users" },
                { v: 0 }
            ];


            $scope.data.chart.data = {
                "cols": [
                    { id: "t", label: "Label", type: "string" },
                    { id: "s", label: "Share", type: "number" }
                ], "rows": [
                    { c: $scope.userShare },
                    { c: $scope.userShareInverse }
                ]
            };

            // $routeParams.chartType == BarChart or PieChart or ColumnChart...
            $scope.data.chart.type = 'PieChart';
            $scope.data.chart.options = {
                'pieHole': '0.8',
                'legend': 'none',
                'pieSliceText': 'none',
                'enableInteractivity': 'false',
                'tooltip': {
                    'trigger': 'none'
                },
                'slices': {
                    '1': { 'color': 'aliceblue' }
                },
                'height': '250',
                'width': '350',
                'backgroundColor': 'none',
            };

            // -------------------------- Watches --------------------------
            $scope.$watch('data.node', function (newValue, oldValue) {
                if (typeof newValue === 'undefined' || newValue === null) return;

                getMatchingVisitors();
            }, true);
        }
    );