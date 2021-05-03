

var x = document.getElementById("searchClose");
x.addEventListener("click", myBlurFunction, true);
//x.addEventListener("blur", myBlurFunction, true);

function myFocusFunction() {
    document.getElementById("search-dropdown").style.display = "block";
}

function myBlurFunction() {
    document.getElementById("search-dropdown").style.display = "none";
}

function searchMain(form) {
    document.getElementById("mdb-5-search-count").textContent = "0"
    document.getElementById("mdb-5-search-list").innerHTML = "";
    document.getElementById("search-dropdown").style.display = "block";

    let formData = new FormData(form);

    var submitButton = form.querySelector('button[type="submit"]');
    var inputs = form.getElementsByTagName("input");

    let currentButtonValue = submitButton.innerHTML;

    submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

    fetch(form.action, {
        method: form.method,
        body: formData
    })
        .then(function (response) {
            return response.json();
        })
        .then(function (data) {
            var membersCount = data.length;

            if (!(membersCount <= 0)) {
                for (var i = 0, len = membersCount; i < len; i++) {
                    var ID = data[i]['id']
                    var Display = data[i]['display']
                    var path = data[i]['path']

                    var li = document.createElement("li")
                    var a = document.createElement("a")
                    a.className = "pt-2 px-2 text-muted d-block"
                    a.href = path

                    var p1 = document.createElement("p")
                    p1.className = "text-uppercase mb-0"
                    p1.textContent = Display

                    var p2 = document.createElement("p")
                    p2.className = "small font-weight-bold mb-0 pb-2"
                    p2.textContent = path.split("/")[1] + " ID: " + ID

                    var hr = document.createElement("hr")
                    hr.className = "m-0 p-0"

                    a.appendChild(p1)
                    a.appendChild(p2)

                    li.appendChild(a)
                    li.appendChild(hr)

                    document.getElementById("mdb-5-search-list").appendChild(li)
                }

                document.getElementById("mdb-5-search-count").textContent = membersCount
            }
            else {
                var li = document.createElement("li")
                var a = document.createElement("a")
                a.className = "pt-2 px-2 text-muted d-block"
                a.href = "#"

                var p1 = document.createElement("p")
                p1.className = "text-uppercase mb-0"
                p1.textContent = "No Items Found"

                var p2 = document.createElement("p")
                p2.className = "small font-weight-bold mb-0 pb-2"
                p2.textContent = "Search Again"

                var hr = document.createElement("hr")
                hr.className = "m-0 p-0"

                a.appendChild(p1)
                a.appendChild(p2)

                li.appendChild(a)
                li.appendChild(hr)

                document.getElementById("mdb-5-search-list").append(li)
            }
            
            document.getElementById("search-dropdown").style.display = "block";        

            submitButton.innerHTML = currentButtonValue;
        });
}