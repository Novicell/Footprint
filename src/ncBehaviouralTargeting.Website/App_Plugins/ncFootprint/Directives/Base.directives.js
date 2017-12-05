angular.module("umbraco")
    .directive('ncbtConfirmClick', [
        function() {
            return {
                link: function(scope, element, attr) {
                    var msg = attr.ncbtConfirmClick;
                    var clickAction = attr.ncbtConfirmedClick;
                    element.bind('click', function(event) {
                        if (window.confirm(msg)) {
                            scope.$eval(clickAction);
                        }
                    });
                }
            };
        }]);
angular.module('umbraco')
    .directive('ncbtStrToNumber', function() {
        return {
            require: 'ngModel',
            link: function(scope, element, attrs, ngModel) {
                ngModel.$parsers.push(function(value) {
                    return '' + value;
                });
                ngModel.$formatters.push(function(value) {
                    return parseFloat(value, 10);
                });
            }
        };
    });

angular.module('umbraco')
    .factory('ncbtDebounce', ['$timeout', function ($timeout) {
        return function (func, wait, immediate) {
            var timeout, args, context, result;
            function debounce() {
                context = this;
                args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) {
                        result = func.apply(context, args);
                    }
                };
                var callNow = immediate && !timeout;
                if (timeout) {
                    $timeout.cancel(timeout);
                }
                timeout = $timeout(later, wait);
                if (callNow) {
                    result = func.apply(context, args);
                }
                return result;
            }

            debounce.cancel = function () {
                $timeout.cancel(timeout);
                timeout = null;
            };
            return {
                debounce: debounce
            };
        };
    }]);