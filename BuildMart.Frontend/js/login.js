renderLayout('');

async function handleLogin(event) {
  event.preventDefault();
  const errorBox = document.getElementById('login-error');
  errorBox.style.display = 'none';

  const btn = document.getElementById('login-btn');
  btn.disabled = true;
  btn.textContent = 'Logging in...';

  try {
    const result = await Api.login({
      email: document.getElementById('login-email').value.trim(),
      password: document.getElementById('login-password').value
    });
    Auth.setSession(result.token, result.user);

    const redirect = new URLSearchParams(window.location.search).get('redirect');
    window.location.href = result.user.role === 'Admin' ? 'admin.html' : (redirect || 'index.html');
  } catch (e) {
    errorBox.style.display = 'block';
    errorBox.textContent = e.message;
    btn.disabled = false;
    btn.textContent = 'Login';
  }
  return false;
}
