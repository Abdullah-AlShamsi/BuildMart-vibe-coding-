renderLayout('');

async function handleRegister(event) {
  event.preventDefault();
  const errorBox = document.getElementById('register-error');
  errorBox.style.display = 'none';

  const password = document.getElementById('reg-password').value;
  const confirm = document.getElementById('reg-confirm').value;
  if (password !== confirm) {
    errorBox.style.display = 'block';
    errorBox.textContent = 'Passwords do not match.';
    return false;
  }

  const btn = document.getElementById('register-btn');
  btn.disabled = true;
  btn.textContent = 'Creating account...';

  try {
    const result = await Api.register({
      fullName: document.getElementById('reg-fullname').value.trim(),
      email: document.getElementById('reg-email').value.trim(),
      phoneNumber: document.getElementById('reg-phone').value.trim() || null,
      password,
      confirmPassword: confirm
    });
    Auth.setSession(result.token, result.user);
    window.location.href = 'index.html';
  } catch (e) {
    errorBox.style.display = 'block';
    errorBox.textContent = e.message;
    btn.disabled = false;
    btn.textContent = 'Create Account';
  }
  return false;
}
