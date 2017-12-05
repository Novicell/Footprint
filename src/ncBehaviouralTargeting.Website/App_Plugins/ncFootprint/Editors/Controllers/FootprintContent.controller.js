angular.module('umbraco')
    .controller('Novicell.Footprint.Editors.FootprintContent.Controller',
    function ($scope, dialogService, ncbtPropertyEditorResource, ncbtSegmentResource) {

        // Check for editor conversions
        if ($scope.model.value != undefined && $scope.model.value.isNcbt == undefined) {
            // Editor of property was changed, we have old data we need to convert
            var oldValue = angular.copy($scope.model.value);
            $scope.model.value = {};
            $scope.model.value.isNcbt = true;
            $scope.model.value.segments = [
                {
                    alias: 'default',
                    sortOrder: 9999,
                    value: oldValue
                }
            ];
        }

        // Check if model is unset
        if (!$scope.model.value) {
            $scope.model.value = {};
            $scope.model.value.isNcbt = true;
            $scope.model.value.segments = [
                {
                    alias: 'default',
                    sortOrder: 9999
                }
            ];
        }

        // Segment list with default
        $scope.segments = [
            {
                DisplayName: 'Default segment',
                Alias: 'default',
                Id: 1
            }
        ];

        // Initialize scope variables
        $scope.propertyEditor = null;
        $scope.editors = {};
        $scope.selectedEditor = null;
        $scope.selectedSegmentAlias = null;
        $scope.loaded = false;
        $scope.hideLabel = false;

        // Get model by segment alias
        $scope.getModelBySegmentAlias = function (segmentAlias) {
            return $scope.model.value.segments.filter(function (obj) {
                return obj.alias === segmentAlias;
            })[0];
        };

        // Get segment by alias
        $scope.getSegmentByAlias = function (segmentAlias) {
            return $scope.segments.filter(function (obj) {
                return obj.Alias === segmentAlias;
            })[0];
        };

        // Set segment by alias
        $scope.setSelectedSegment = function (segmentAlias) {
            $scope.selectedSegmentAlias = segmentAlias;
        };

        // Update property editors, add missing
        $scope.updatePropertyEditors = function () {
            for (var i = 0; i < $scope.model.value.segments.length; i++) {
                if ($scope.model.value.segments.hasOwnProperty(i)) {
                    var editorModel = $scope.model.value.segments[i];
                    // Check if editor model exists
                    if ($scope.editors[editorModel.alias] == undefined) {
                        // Create editor
                        $scope.editors[editorModel.alias] = {};
                        angular.copy($scope.propertyEditor, $scope.editors[editorModel.alias]);
                        // Pull value if available
                        $scope.editors[editorModel.alias].value = editorModel.value;
                    }
                }
            }
        };

        $scope.openSettings = function () {
            // Pull list of active segments
            dialogService.open({
                template: '/App_Plugins/ncFootprint/Dialogs/Views/SegmentSettingsDialog.html',
                show: true,
                dialogData: {
                    segments: $scope.model.value.segments
                },
                callback: function (data) {
                    if (data != undefined) {
                        var defaultSegment = angular.copy($scope.getModelBySegmentAlias('default'));

                        // Copy old model and create new
                        var oldSegments = angular.copy($scope.model.value.segments);
                        var newSegments = [];

                        // Clear old model
                        $scope.model.value.segments = [];

                        // Go through new segments
                        for (var i = 0; i < data.length; i++) {
                            // Get new segment
                            var segment = data[i];
                            // Get old segment
                            var oldSegment = oldSegments.filter(function (obj) {
                                return obj.alias === this.alias;
                            }, segment)[0];
                            // Set value to persist if we got one
                            if (oldSegment != undefined) {
                                segment.value = oldSegment.value;
                            } else {
                                // No value found, must be a new segment, copy from default
                                segment.value = defaultSegment.value;
                            }
                            // Add segment
                            newSegments.push(segment);
                        };

                        // Add default segment
                        newSegments.push(defaultSegment);

                        // Assign new model to model
                        $scope.model.value.segments = newSegments;

                        // Update editors
                        $scope.updatePropertyEditors();

                        // Select first segment
                        $scope.selectedSegmentAlias = $scope.model.value.segments[0].alias;
                        $scope.selectedEditor = $scope.editors[$scope.selectedSegmentAlias];
                    }
                }
            });
        };

        // Get segments
        ncbtSegmentResource.GetAllLight().then(function (response) {
            response.data.forEach(function (segment) {
                $scope.segments.push(segment);
            });
        });

        // Get property editor details and initialize
        ncbtPropertyEditorResource.GetByAlias($scope.model.config.contentType.selectedEditor).then(function (response) {
            // Create property editor model
            $scope.propertyEditor = {
                alias: response.data.alias,
                dataType: response.data.dataType,
                view: response.data.view,
                config: {}
            };
            // Add prevalues
            $scope.model.config.contentType.preValues.forEach(function (prevalue) {
                // Check if multivalues
                if (prevalue.view == 'multivalues') {
                    $scope.propertyEditor.config[prevalue.alias] = {};
                    prevalue.value.forEach(function(item) {
                        $scope.propertyEditor.config[prevalue.alias][item.id] = item;
                    });
                } else {
                    $scope.propertyEditor.config[prevalue.alias] = prevalue.value;
                }
            });
            // Get datatype id
            $scope.model.value.dataTypeId = $scope.model.config.contentType.dataTypeId;

            // Fixes for inbuilt type
            if ($scope.propertyEditor.alias === 'Umbraco.MultiNodeTreePicker') {
                // How to eradicate: Find out where Umbraco sets multiPicker = true in their native editor
                $scope.propertyEditor.config.multiPicker = '1';
            }
            if ($scope.propertyEditor.alias === 'Umbraco.Grid') {
                // How to eradicate: None
                $scope.hideLabel = true;
            }

            // Update editors
            $scope.updatePropertyEditors();

            $scope.selectedSegmentAlias = $scope.model.value.segments[0].alias;
            $scope.selectedEditor = $scope.editors[$scope.selectedSegmentAlias];

            $scope.loaded = true;
        });

        // Watch for selection changes
        $scope.$watch('selectedSegmentAlias', function (newVal, oldVal) {
            if ($scope.loaded) {
                if (newVal && (newVal !== oldVal)) {
                    $scope.selectedEditor = $scope.editors[newVal];
                }
            }
        });

        // Watch for editor changes
        $scope.$watch('editors', function (newVal, oldVal) {
            if ($scope.loaded) {
                if (newVal && (newVal !== oldVal)) {
                    for (var segmentAlias in $scope.editors) {
                        if ($scope.editors.hasOwnProperty(segmentAlias)) {
                            var model = $scope.getModelBySegmentAlias(segmentAlias);
                            // Check if segment is in model
                            if (model != null) {
                                // Update segment value
                                model.value = $scope.editors[segmentAlias].value;
                            }
                        }
                    }
                }
            }
        }, true);
    });