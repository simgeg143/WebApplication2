document.addEventListener("DOMContentLoaded", function() {
        document.getElementById("registerForm").addEventListener("submit", function (e) {
            var username = document.getElementById("username").value.trim();
            var password = document.getElementById("password").value.trim();
            var email = document.getElementById("email").value.trim();
            var birthday = document.getElementById("birthday").value.trim();

            const usernameRegex = /^[a-zA-Z]+$/;
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            if (username === "" || password === "" || email === "" || birthday === "") {
                e.preventDefault();
                alert("All fields are required.");
                return;
            }

            if (!usernameRegex.test(username)) {
                e.preventDefault();
                alert("Username must contain only letters.");
                return;
            }
            if (!emailRegex.test(email)) {
                e.preventDefault();
                alert("Invalid email format.");
                return;
            }
            const date = new Date(birthday);
            const now = new Date();

            if (isNaN(date.getTime()) || (date > now)) {
                e.preventDefault();
                alert("Invalid birthday.");
                return;
            }
        });
});