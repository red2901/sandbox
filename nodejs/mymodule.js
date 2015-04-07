var fs = require('fs');
var path = require('path');

var EXPORTED_SYMBOLS = { 'filterdir' };

function filterdir (dirname, extname, callback) {

    var flist;
    fs.readdir(dirname, function (err, list) {
        if (err) throw err;
    
        list.forEach(function(file) {
            /// equal without type comparison
            if (path.extname(file) === '.' + extname)
                flist.add(file);
        })
    });
    callback(null, flist)
}

