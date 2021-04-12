

var x = document.getElementById("searchMain");
x.addEventListener("focus", myFocusFunction, true);
x.addEventListener("blur", myBlurFunction, true);

function myFocusFunction() {
    document.getElementById("search-dropdown").style.display = "block";
}

function myBlurFunction() {
    document.getElementById("search-dropdown").style.display = "none";
}

function searchMain(form) {
    let inputVal = document.getElementById("SearchData_Search").value;
    let inputOption = document.getElementById('SearchData_SearchType').value;

    debugger;

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
            debugger;
            return response.json();
        })
        .then(function (data) {
            debugger;

            submitButton.innerHTML = currentButtonValue;
        });
}