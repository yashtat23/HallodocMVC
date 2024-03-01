// Check if dark mode preference is stored and apply it
if (localStorage.getItem('darkMode') === 'true') {
    document.body.classList.add('dark-mode');
}

function toggleDarkMode() {
    const body = document.body;
    body.classList.toggle('dark-mode');

    // Store the dark mode preference in local storage
    const isDarkMode = body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDarkMode);
    var img = element.querySelector(".light")
    img.src.includes("dark") ? img.src = "../img/light.png" : img.src = "../img/dark.png";

}