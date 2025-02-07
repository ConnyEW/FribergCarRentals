

// Client-side validation to check for matching passwords

document.getElementById('passwordForm').addEventListener('submit', function (event) {

    let password = document.getElementById('password').value;
    let repeatPassword = document.getElementById('passwordRepeat').value;

    if (!(password === repeatPassword)) {
        event.preventDefault();
        document.getElementById('passwordRepeatMessage').textContent = "Passwords do not match.";
    }
});
