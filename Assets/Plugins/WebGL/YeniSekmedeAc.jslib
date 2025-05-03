mergeInto(LibraryManager.library, {
    YeniSekmedeAc: function(urlPtr) {
        var url = UTF8ToString(urlPtr);
        var win = window.open(url, '_blank');
        if (win) {
            win.focus();
        } else {
            console.log("Popup blocked!");
        }
    }
});
