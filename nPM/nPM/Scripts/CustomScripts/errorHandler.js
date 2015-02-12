
function error_handlerXML(from, XMLHttpRequest) {
    //debugger;
    try {
        var str = XMLHttpRequest.responseText;
        var err = str.substring(str.indexOf("<title>") + 7, str.indexOf("</title>"))
        toastr.error(from + " - error_handlerXML", err);
    }
    catch (e) {
        toastr.error("error_handlerXML", e.message);
    }
}