angular.module('umbraco')
    .controller('Novicell.Footprint.Dialogs.PropertyPickerDialog.Controller',
        function ($scope, localizationService, ncbtPropertyResource) {

            $scope.nodes = [];

            $scope.model = null;

            // Load properties
            ncbtPropertyResource.GetAllLight().then(function (response) {
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