document.getElementById("uploadFields").parentElement.outerHTML +=
    "<fieldset id=\"uploadScreens\" style=\"display: none;\"><legend>Screenshot hinzufügen</legend><ul id=\"listScreens\"></ul></fieldset>";
var field = document.getElementById("uploadScreens");
var list = document.getElementById("listScreens");
var update = new window.XMLHttpRequest();
var get = new window.XMLHttpRequest();
var forms = document.getElementsByTagName("form");
for (var j = 0; j < forms.length; j++) {
    if (forms[j].enctype == "multipart/form-data") {
        var form = forms[j];
        break;
    }
}
get.responseType = "arraybuffer";
update.onreadystatechange = function () {
    if (update.readyState == 4 && update.status == 200) {
        var s = update.responseText.split("\n");
        list.innerHTML = "";
        if (s[0] == "") {
            field.style.display = "none";
        } else {
            field.style.display = "";
            for (i in s) {
                var li = document.createElement("li");
                li.style.cursor = "pointer";
                li.textContent = s[i];
                li.title = "\"" + s[i] + "\" hochladen";
                li.onclick = function () {
                    document.body.style.cursor = "wait";
                    get.onreadystatechange = function () {
                        if (get.readyState == 4 && get.status == 200) {
                            var dat = new FormData(form);
                            var blob = new Blob([get.response], { type: "image/png" });
                            dat.append("upload[]", blob, s[i]);
                            var up = new XMLHttpRequest();
                            up.open(form.method, form.action, true);
                            up.onreadystatechange = function () {
                                if (up.readyState == 4 && up.status == 200) {
                                    attachmentL = document.getElementById("attachmentList");
                                    attachments = new Array();
                                    if (attachmentL != null) {
                                        for (var at = 0; at < attachmentL.children; at++) {
                                            attachments.push(attachmentL.children[at].id.split("_")[1]);
                                        }
                                    }
                                    s = /attachmentListPositions\[([0-9]+)\]/g;
                                    while (at = s.exec(up.responseText)) {
                                        if (attachments.indexOf(at[1]) == -1) {
                                            attachments.push(at);
                                            input = document.createElement("input");
                                            input.name = "attachmentListPositions[" + at + "]";
                                            input.value = "0";
                                            input.type = "hidden";
                                            form.appendChild(input);
                                        }
                                    }
                                    form.submit();
                                }
                            }
                            up.send(dat);
                        }
                    }
                    get.open("GET", "http://localhost:7410/getImg?id=" + s[i]);
                    get.send();
                }
                list.appendChild(li);
            }
        }
        update.abort();
        update.open("GET", "http://localhost:7410/list", true);
        update.send();
    }
}
update.open("GET", "http://localhost:7410/list?first", true);
update.send();