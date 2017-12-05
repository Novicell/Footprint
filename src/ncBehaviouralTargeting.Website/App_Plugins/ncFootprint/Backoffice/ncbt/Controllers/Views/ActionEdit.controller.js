angular.module('umbraco')
    .controller('ncFootprint.Backoffice.ActionEdit.Controller',
        function ($scope, $routeParams, $controller, notificationsService, dialogService, ncbtActionResource, ncbtSegmentResource, ncbtPropertyResource, contentResource) {
            // Sync tree
            $scope.treeSyncPath = ['actionoverview'];

            // Extend from base overview
            angular.extend(this, $controller('ncFootprint.Backoffice.BaseEdit.Controller', {
                $scope: $scope,
                $routeParams: $routeParams
            }));

            // -------------------------- Breadcrumbs --------------------------
            $scope.$parent.pushBreadcrumb('Actions', 'actionoverview');

            // -------------------------- Config --------------------------
            // Set up tabs
            $scope.tabs = [
                { id: 1, label: "General" }
            ];

            // Set up data
            $scope.data = {
                showTips: true,
                node: null,
                actionTypes: [
                    { id: 0, label: 'Inactive' },
                    { id: 1, label: 'Send email' }
                ]
            };

            $scope.fallbacks = {
                segment: {
                    Alias: null,
                    DisplayName: "Deleted segment",
                    Id: null
                }
            };

            // -------------------------- Manipulation methods  --------------------------
            // Action methods
            $scope.loadAction = function (action) {
                $scope.data.node = action;
                if ($scope.data.node.ActionType == null)
                    $scope.data.node.ActionType = 0;

                // Push breadcrumb
                $scope.$parent.pushBreadcrumb('Edit Action - ' + action.DisplayName);

                if ($scope.data.node.SegmentId != undefined && $scope.data.node.SegmentId !== 0) {
                    // Load segment light data
                    ncbtSegmentResource.GetLightById($scope.data.node.SegmentId).then(function (response) {
                        console.dir(response);
                        if (response.data != 'null') {
                            $scope.data.segment = response.data;
                        } else {
                            // Fetch fallback
                            $scope.data.segment = $scope.fallbacks.segment;
                            // Invalidate action
                            $scope.data.node.SegmentId = 0;
                            $scope.data.node.ActionType = 0;
                        }
                    });
                }
                if ($scope.data.node.EmailPropertyId != undefined && $scope.data.node.EmailPropertyId !== 0) {
                    // Load property light data
                    ncbtPropertyResource.GetById($scope.data.node.EmailPropertyId).then(function (response) {
                        $scope.data.emailProperty = response.data;
                    });
                }
                
                if ($scope.data.node.EmailNodeId != undefined && $scope.data.node.EmailNodeId !== 0) {
                    // Load email node
                    contentResource.getById($scope.data.node.EmailNodeId).then(function (response) {
                        $scope.data.emailNode = response;
                    });
                }
                
            }

            $scope.saveAction = function () {
                // Save action
                ncbtActionResource.Save($scope.data.node).then(function (response) {
                    // Populate action
                    if (response.data != null) {
                        $scope.loadAction(response.data);
                        notificationsService.success("Success", "Action saved.");
                    } else {
                        notificationsService.error("Error", "Could not save action.");
                    }
                });
            }

            // Segment methods
            $scope.btnSelectSegment = function () {
                // Open dialog to let the user select a segment
                dialogService.open({
                    template: '/App_Plugins/ncFootprint/Dialogs/Views/SegmentPickerDialog.html',
                    show: true,
                    dialogData: {
                        hideAliases: [],
                        selectedId: $scope.data.node.SegmentId
                    },
                    callback: function (data) {
                        if (data != undefined) {
                            // Extract segment
                            $scope.data.node.SegmentId = data.Id;
                            $scope.data.segment = data;
                        }
                    }
                });
            };

            // Property methods
            $scope.btnSelectEmailProperty = function () {
                // Open dialog to let the user select a property
                dialogService.open({
                    template: '/App_Plugins/ncFootprint/Dialogs/Views/PropertyPickerDialog.html',
                    show: true,
                    dialogData: {
                        hideAliases: [],
                        selectedId: $scope.data.node.EmailPropertyId
                    },
                    callback: function (data) {
                        if (data != undefined) {
                            // Extract segment
                            $scope.data.node.EmailPropertyId = data.Id;
                            $scope.data.emailProperty = data;
                        }
                    }
                });
            };

            // Email methods
            $scope.btnSelectEmailNode = function () {
                // Open dialog to let the user select a property
                dialogService.contentPicker({
                    multipicker: false,
                    callback: function (data) {
                        if (data != undefined) {
                            // Extract node
                            $scope.data.node.EmailNodeId = data.id;
                            $scope.data.emailNode = data;
                        }
                    }
                });
            };


            // -------------------------- Initial data fetching --------------------------
            // Fetch current action
            ncbtActionResource.GetById($routeParams.id).then(function (response) {
                $scope.loadAction(response.data);
            });
        }
    );