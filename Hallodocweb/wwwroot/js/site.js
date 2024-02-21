function myFunction() {
    var element = document.body;
    element.classList.toggle("dark-mode");
    var img = element.querySelector(".light")
    img.src.includes("dark") ? img.src = "../img/light.png" : img.src = "../img/dark.png";
}

var modal = document.getElementById("myModal");


var btn = document.getElementById("myBtn");

$(document).ready(function () {
    modal.style.display = "block";
});

var span = document.getElementsByClassName("close")[0];

// When the user clicks the button, open the modal 
btn.onclick = function () {
    modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}


$(window).on('load', function () {
    $('#myModal').modal('show');
})


