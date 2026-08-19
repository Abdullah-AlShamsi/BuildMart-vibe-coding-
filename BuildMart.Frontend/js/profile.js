requireAuth();
renderLayout('');

async function loadProfile() {
  try {
    const user = await Api.me();
    document.getElementById('p-fullname').value = user.fullName;
    document.getElementById('p-email').value = user.email;
    document.getElementById('p-phone').value = user.phoneNumber || '';
    document.getElementById('p-address').value = user.address || '';
    document.getElementById('p-city').value = user.city || '';
  } catch (e) {
    document.getElementById('profile-msg').className = 'alert alert-error';
    document.getElementById('profile-msg').style.display = 'block';
    document.getElementById('profile-msg').textContent = 'Could not load your profile.';
  }
}

async function handleProfileUpdate(event) {
  event.preventDefault();
  const msg = document.getElementById('profile-msg');
  const btn = document.getElementById('profile-save-btn');
  btn.disabled = true;
  btn.textContent = 'Saving...';

  try {
    const updated = await Api.updateProfile({
      fullName: document.getElementById('p-fullname').value.trim(),
      phoneNumber: document.getElementById('p-phone').value.trim() || null,
      address: document.getElementById('p-address').value.trim() || null,
      city: document.getElementById('p-city').value.trim() || null
    });
    const user = Auth.getUser();
    user.fullName = updated.fullName;
    localStorage.setItem('buildmart_user', JSON.stringify(user));

    msg.className = 'alert alert-success';
    msg.style.display = 'block';
    msg.textContent = 'Profile updated successfully.';
  } catch (e) {
    msg.className = 'alert alert-error';
    msg.style.display = 'block';
    msg.textContent = e.message;
  } finally {
    btn.disabled = false;
    btn.textContent = 'Save Changes';
  }
  return false;
}

loadProfile();
