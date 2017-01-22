$(document).ready(function () {

    var jqxhr = $.getJSON( "/api/beers", function (data) {
        ko.applyBindings(new ViewModel(data));

    })

    var ViewModel = function (data) {
        var self = this;
        self.filter = ko.observable("");
        data.forEach(function (pub) {
            pub.filteredBeers = ko.computed(function () {
                
                var filter = self.filter();
                if (!filter || filter == "None") {
                    var ret = pub.beers;

                    return ret;
                } else {
                    var ret = ko.utils.arrayFilter(pub.beers, function (i) {
                        //console.log(i);
                        return i.name.toLowerCase().search(filter.toLowerCase()) >= 0 || (i.type != null && i.type.toLowerCase().search(filter.toLowerCase()) >= 0) || (i.brewery != null && i.brewery.toLowerCase().search(filter.toLowerCase())) >= 0;
                    });

                    return ret;
                }
            });

        })
        self.update = function () {
            $.get("api/beers/update", function (data) {
                location.reload();
            });
        }
        self.pubs = ko.observableArray(data);
    }
});