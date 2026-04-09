document.addEventListener('DOMContentLoaded', () => {

    const form = document.querySelector('form');
    const username = document.getElementById('username');
    const email = document.getElementById('email');
    const fullName = document.getElementById('fullName');
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirmPassword');
    const submitBtn = form.querySelector('button[type="submit"]');

    const WEAK_PASSWORDS = [
        "1234", "12345", "123456", "1234567", "12345678", "123456789",
        "qwerty", "qwerty123", "qwertyuiop",
        "password", "password1", "pass",
        "admin", "root", "user", "test"
    ];

    const FORBIDDEN_USERNAMES = [
        "admin", "root", "system", "support", "moderator", "owner"
    ];

    const TEMP_EMAIL_DOMAINS = [
        "mailinator", "tempmail", "10minutemail", "guerrillamail", "trashmail"
    ];

    const STRONG_PASSWORD_REGEX =
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=\[\]{};:,.<>/?])\S{8,64}$/;

    const USERNAME_REGEX = /^[A-Za-z][A-Za-z0-9_]{2,18}[A-Za-z0-9]$/;

    const EMAIL_REGEX =
        /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

    function cleanInput(value) {
        return value.trim().replace(/\u200B/g, '');
    }

    function showError(field, message) {
        let error = field.parentNode.querySelector('.field-error');
        if (!error) {
            error = document.createElement('div');
            error.className = 'field-error text-danger small mt-1';
            field.parentNode.appendChild(error);
        }
        error.textContent = message;
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');
    }

    function hideError(field) {
        const error = field.parentNode.querySelector('.field-error');
        if (error) error.remove();
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
    }

    function containsEmoji(str) {
        return /\p{Extended_Pictographic}/u.test(str);
    }

    function isSequence(str) {
        const sequences = [
            "123456", "234567", "345678", "456789",
            "abcdef", "bcdefg", "cdefgh", "defghi",
            "qwerty", "asdfgh", "zxcvbn"
        ];
        return sequences.some(seq => str.toLowerCase().includes(seq));
    }

    function isRepeating(str) {
        return /^(.)\1+$/.test(str);
    }

    function isTempEmail(email) {
        const domain = email.split('@')[1] || '';
        return TEMP_EMAIL_DOMAINS.some(d => domain.includes(d));
    }

    function validateUsername() {
        const value = cleanInput(username.value).toLowerCase();

        if (value.length < 3) {
            showError(username, "Минимум 3 символа");
            return username.setCustomValidity("Слишком короткое имя");
        }

        if (value.length > 20) {
            showError(username, "Максимум 20 символов");
            return username.setCustomValidity("Слишком длинное имя");
        }

        if (FORBIDDEN_USERNAMES.includes(value)) {
            showError(username, "Недопустимое имя");
            return username.setCustomValidity("Имя запрещено");
        }

        if (!USERNAME_REGEX.test(value)) {
            showError(username, "Только латиница, цифры и _");
            return username.setCustomValidity("Неверный формат");
        }

        hideError(username);
        username.setCustomValidity("");
    }

    function validateEmail() {
        let value = cleanInput(email.value).toLowerCase();
        email.value = value;

        if (!EMAIL_REGEX.test(value)) {
            showError(email, "Некорректный email");
            return email.setCustomValidity("Неверный email");
        }

        if (isTempEmail(value)) {
            showError(email, "Временные email запрещены");
            return email.setCustomValidity("Временный email запрещён");
        }

        hideError(email);
        email.setCustomValidity("");
    }

    function validatePassword() {
        let value = cleanInput(password.value);
        password.value = value;

        if (value.length > 64) {
            value = value.slice(0, 64);
            password.value = value;
        }

        if (/\s/.test(value)) {
            showError(password, "Пароль не должен содержать пробелы");
            return password.setCustomValidity("Пробелы запрещены");
        }

        if (value.length < 8) {
            showError(password, "Минимум 8 символов");
            return password.setCustomValidity("Слишком короткий пароль");
        }

        if (isRepeating(value)) {
            showError(password, "Пароль не может состоять из одинаковых символов");
            return password.setCustomValidity("Повторяющиеся символы запрещены");
        }

        if (WEAK_PASSWORDS.includes(value.toLowerCase())) {
            showError(password, "Пароль слишком простой");
            return password.setCustomValidity("Слабый пароль");
        }

        if (isSequence(value)) {
            showError(password, "Пароль не должен быть последовательностью");
            return password.setCustomValidity("Последовательности запрещены");
        }

        if (containsEmoji(value)) {
            showError(password, "Emoji запрещены");
            return password.setCustomValidity("Emoji запрещены");
        }

        if (!STRONG_PASSWORD_REGEX.test(value)) {
            showError(password, "Нужны строчная, заглавная, цифра и спецсимвол");
            return password.setCustomValidity("Недостаточная сложность");
        }

        hideError(password);
        password.setCustomValidity("");

        validateConfirmPassword();
    }

    function validateConfirmPassword() {
        if (password.value !== confirmPassword.value) {
            showError(confirmPassword, "Пароли не совпадают");
            return confirmPassword.setCustomValidity("Пароли не совпадают");
        }

        hideError(confirmPassword);
        confirmPassword.setCustomValidity("");
    }

    function calculateStrength(pwd) {
        let score = 0;
        if (pwd.length >= 8) score++;
        if (/[a-z]/.test(pwd)) score++;
        if (/[A-Z]/.test(pwd)) score++;
        if (/\d/.test(pwd)) score++;
        if (/[^a-zA-Z\d]/.test(pwd)) score++;
        if (!isRepeating(pwd) && !isSequence(pwd)) score++;
        return score;
    }

    function updateStrength() {
        const pwd = password.value;
        let indicator = password.parentNode.querySelector('.password-strength');

        if (!indicator) {
            indicator = document.createElement('div');
            indicator.className = 'password-strength mt-1';
            password.parentNode.appendChild(indicator);
        }

        const score = calculateStrength(pwd);
        const labels = ["Очень слабый", "Слабый", "Средний", "Хороший", "Отличный"];
        const colors = ["#dc3545", "#fd7e14", "#ffc107", "#20c997", "#28a745"];

        if (!pwd) {
            indicator.style.display = "none";
            return;
        }

        indicator.style.display = "block";
        indicator.innerHTML = `
        <small class="text-muted">
            Надёжность пароля:
            <span style="color:${colors[Math.max(score - 1, 0)]}; font-weight:bold;">
                ${labels[Math.max(score - 1, 0)]}
            </span>
        </small>
        `;
    }

    username.addEventListener('input', validateUsername);
    email.addEventListener('input', validateEmail);

    password.addEventListener('input', () => {
        validatePassword();
        updateStrength();
    });

    confirmPassword.addEventListener('input', validateConfirmPassword);

    form.addEventListener('submit', (e) => {
        if (!form.checkValidity()) {
            e.preventDefault();
            username.dispatchEvent(new Event('input'));
            email.dispatchEvent(new Event('input'));
            password.dispatchEvent(new Event('input'));
            confirmPassword.dispatchEvent(new Event('input'));
            return;
        }

        submitBtn.disabled = true;
        submitBtn.innerHTML =
            '<span class=\"spinner-border spinner-border-sm me-2\"></span>Создание аккаунта...';
    });

});