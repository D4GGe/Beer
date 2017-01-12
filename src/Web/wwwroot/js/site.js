$(document).ready(function () {

    var jqxhr = $.getJSON( "/api/beers", function (data) {
        ko.applyBindings(new ViewModel(data));

    })

    var ViewModel = function (data) {
        var self = this;
        self.pubs = ko.observableArray(data);
    }
});