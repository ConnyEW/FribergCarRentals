

// Input current year into footer
currentYear();
function currentYear() {
    const currentYear = new Date().getFullYear();
    document.getElementById('current-year').textContent = currentYear;
}