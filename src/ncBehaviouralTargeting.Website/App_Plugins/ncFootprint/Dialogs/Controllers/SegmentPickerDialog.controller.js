angular.module('umbraco')
    .controller('Novicell.Footprint.Dialogs.SegmentPickerDialog.Controller',
        function ($scope, localizationService, ncbtSegmentResource) {

            $scope.nodes = [];

            $scope.model = null;

            // Load segments
            ncbtSegmentResource.GetAllLight().then(function (response) {
                if ($scope.dialogData.hideAliases != undefined) {
                    $scope.nodes = response.data.filter(function (obj) {
                        return $scope.dialogData.hideAliases.indexOf(obj.Alias) === -1;
                    });
                } else {
                    $scope.nodes = response.data;
                }

                // Preselect if possible
                if ($scope.dialogData.selectedId != undefined) {
                    $scope.model = $scope.nodes.filter(function (obj) {
                        return $scope.dialogData.selectedId === obj.Id;
                    })[0];
                }
            });

            $scope.select = function (node) {
                $scope.model = node;
            };
        });