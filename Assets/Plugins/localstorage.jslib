mergeInto(LibraryManager.library, {
  SaveToLocalStorage : function(data) {
    const reader = new FileReader();
    var s = UTF8ToString(data);
    console.log(s);
    localStorage.setItem("hovermania_data", s);
  },

  LoadFromLocalStorage : function() {
    var returnStr = localStorage.getItem("hovermania_data");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
  
  DataExistsInLocalStorage : function() {
    if (localStorage.getItem("hovermania_data")) {
      return 1;
    }
    else {
      return 0;
    }
  },
});;
