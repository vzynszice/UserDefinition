document.addEventListener('DOMContentLoaded', function () {
    // Form validasyonu
    const form = document.getElementById('loginFormValidation');
    const usernameInput = document.querySelector('input[name="Username"]');
    const passwordInput = document.querySelector('input[name="Password"]');

    form.addEventListener('submit', function (event) {
        let isValid = true;

        if (usernameInput.value.length < 3) {
            usernameInput.classList.add('is-invalid');
            isValid = false;
        } else {
            usernameInput.classList.remove('is-invalid');
        }

        if (passwordInput.value.length < 6) {
            passwordInput.classList.add('is-invalid');
            isValid = false;
        } else {
            passwordInput.classList.remove('is-invalid');
        }

        if (!isValid) {
            event.preventDefault();
        }
    });

    // Input validasyonu
    usernameInput.addEventListener('input', function () {
        if (this.value.length >= 3) {
            this.classList.remove('is-invalid');
        }
    });

    passwordInput.addEventListener('input', function () {
        if (this.value.length >= 6) {
            this.classList.remove('is-invalid');
        }
    });
});

// Şifre görünürlük kontrolü
function togglePassword() {
    const passwordInput = document.getElementById('Password');
    const toggleIcon = document.getElementById('toggleIcon');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.remove('bi-eye');
        toggleIcon.classList.add('bi-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.remove('bi-eye-slash');
        toggleIcon.classList.add('bi-eye');
    }
}