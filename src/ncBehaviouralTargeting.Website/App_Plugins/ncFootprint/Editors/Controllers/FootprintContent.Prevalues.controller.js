angular.module('umbraco')
    .controller('Novicell.Footprint.Editors.FootprintContent.Prevalues.Controller',
    function ($scope, $routeParams, $location, appState, navigationService, treeService, dataTypeResource) {

        if ($scope.model.value == undefined) {
            $scope.model.value = {};
        }

        //method used to configure the pre-values when we retrieve them from the server
        function createPreValueProps(preVals) {
            $scope.model.value.preValues = [];
            for (var i = 0; i < preVals.length; i++) {
                $scope.model.value.preValues.push({
                    hideLabel: preVals[i].hideLabel,
                    alias: preVals[i].key,
                    description: preVals[i].description,
                    label: preVals[i].label,
                    view: preVals[i].view,
                    value: preVals[i].value
                });
            }
        }

        // Set up the standard data type props
        $scope.properties = {
            selectedEditor: {
                alias: "selectedEditor",
                description: "Select the property editor that the segmented content should use.",
                label: "Content property editor"
            },
            selectedEditorId: {
                alias: "selectedEditorId",
                label: "Content property editor alias"
            }
        };


        function resourceCallback(data) {
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.content = data;

            // Remove self from list
            $scope.content.availableEditors.forEach(function (editor) {
                // Check if self
                if (editor.alias === 'Novicell.Footprint.FootprintContent') { // Remove own editors
                    // Remove from list
                    var index = $scope.content.availableEditors.indexOf(editor);
                    $scope.content.availableEditors.splice(index, 1);
                }
            });
        }

        if ($routeParams.create) {
            // We are creating so get an empty data type item
            dataTypeResource.getScaffold().then(resourceCallback);
        } else {
            // Get own content type
            dataTypeResource.getById($routeParams.id).then(resourceCallback);
            $scope.model.value.dataTypeId = $routeParams.id;
        }

        $scope.$watch("model.value.selectedEditor", function (newVal, oldVal) {
            // When the value changes, we need to dynamically load in the new editor
            if (newVal && (newVal != oldVal)) {
                // We are editing so get the content item from the server
                var currDataTypeId = undefined;//$routeParams.create ? undefined : $routeParams.id;
                dataTypeResource.getPreValues(newVal, currDataTypeId)
                    .then(function (data) {
                        $scope.preValuesLoaded = true;
                        $scope.preValues = data;
                        createPreValueProps($scope.preValues);
                    });
            }
        });
    });