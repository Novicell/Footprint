var CurrentVisitor = {
    SetProperty: function (key, value) {

        var data = JSON.stringify({ 'key': key, 'value': value });

        if (value instanceof Date) {
            var formated = ncbtDate(value);
            data = JSON.stringify({ 'key': key, 'value': value, 'formated': formated });
        }

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/umbraco/ncFootprintApi/CurrentVisitor/SetProperty");
        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
        xhr.send(data);
    },
    
    IsInSegment: function (segmentAlias) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', encodeURI('/umbraco/ncFootprintApi/CurrentVisitor/IsInSegment?segmentAlias=' + segmentAlias));
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send();
    },

    SetVisitorId: function (visitorId) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', encodeURI('/umbraco/ncFootprintApi/CurrentVisitor/SetVisitorId?visitorId=' + visitorId));
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.send();
    }
};

function ncbtDate(date) {
    var year = date.getFullYear().toString();
    var month = (date.getMonth() + 1).toString();
    var day = date.getDate().toString();
    var hh = date.getHours().toString();
    var mm = date.getMinutes().toString();
    var ss = date.getSeconds().toString();
    return (day[1] ? day : '0' + day[0]) + '/' + (month[1] ? month : '0' + month[0]) + '/' + year + ' '
        + (hh[1] ? hh : '0' + hh[0]) + ':' + (mm[1] ? mm : '0' + mm[0]) + ':' + (ss[1] ? ss : '0' + ss[0]);
}