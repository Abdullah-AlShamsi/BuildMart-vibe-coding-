/**
 * Renders the shared header/nav/footer and wires up the search bar +
 * cart badge. Called at the top of every page.
 */
function renderLayout(activePage) {
  const user = Auth.getUser();

  const headerHtml = `
    <header class="site-header">
      <div class="container header-top">
        <a href="index.html" class="logo">Build<span>Mart</span></a>
        <form class="search-bar" onsubmit="return handleHeaderSearch(event)">
          <input type="text" id="header-search-input" placeholder="Search tools, materials, brands...">
          <button type="submit">🔍</button>
        </form>
        <div class="header-actions">
          ${user
            ? `<a href="profile.html">👤 ${user.fullName.split(' ')[0]}</a>
               <a href="my-orders.html">📦 My Orders</a>
               ${user.role === 'Admin' ? '<a href="admin.html">⚙️ Admin</a>' : ''}
               <a href="#" onclick="Auth.logout(); return false;">Logout</a>`
            : `<a href="login.html">👤 Login</a>
               <a href="register.html">Register</a>`
          }
          ${user && user.role !== 'Admin'
            ? `<a href="cart.html">🛒 Cart<span class="cart-badge" id="cart-badge">0</span></a>`
            : ''}
        </div>
      </div>
      <nav class="main-nav">
        <div class="container">
          <ul>
            <li><a href="index.html" class="${activePage === 'home' ? 'active' : ''}">Home</a></li>
            <li><a href="products.html" class="${activePage === 'products' ? 'active' : ''}">All Products</a></li>
            <li><a href="products.html?category=1" >Power Tools</a></li>
            <li><a href="products.html?category=3">Construction Materials</a></li>
            <li><a href="products.html?category=4">Safety Equipment</a></li>
            <li><a href="products.html?category=5">Painting Tools</a></li>
          </ul>
        </div>
      </nav>
    </header>`;

  const footerHtml = `
    <footer class="site-footer">
      <div class="container">
        <div>
          <h4>BuildMart</h4>
          <p>Your trusted source for construction tools and materials.</p>
        </div>
        <div>
          <h4>Shop</h4>
          <p><a href="products.html">All Products</a></p>
          <p><a href="my-orders.html">My Orders</a></p>
        </div>
        <div>
          <h4>Account</h4>
          <p><a href="login.html">Login</a></p>
          <p><a href="register.html">Register</a></p>
        </div>
      </div>
      <div class="footer-bottom container">&copy; ${new Date().getFullYear()} BuildMart. Built for demonstration purposes.</div>
    </footer>`;

  document.getElementById('app-header').innerHTML = headerHtml;
  document.getElementById('app-footer').innerHTML = footerHtml;

  if (user && user.role !== 'Admin') refreshCartBadge();
}

function handleHeaderSearch(event) {
  event.preventDefault();
  const value = document.getElementById('header-search-input').value.trim();
  window.location.href = `products.html?search=${encodeURIComponent(value)}`;
  return false;
}

async function refreshCartBadge() {
  const badge = document.getElementById('cart-badge');
  if (!badge) return;
  try {
    const cart = await Api.getCart();
    badge.textContent = cart.totalItems;
  } catch { /* ignore — user may not have a cart yet */ }
}

function requireAuth(rolesAllowed) {
  if (!Auth.isLoggedIn()) {
    window.location.href = 'login.html';
    return false;
  }
  const user = Auth.getUser();
  if (rolesAllowed && !rolesAllowed.includes(user.role)) {
    window.location.href = 'index.html';
    return false;
  }
  return true;
}

function money(amount) {
  // Omani Rial (OMR) is conventionally shown with 3 decimal places (baisa subunit),
  // unlike most currencies which use 2.
  return `${Number(amount).toFixed(3)} OMR`;
}

function escapeHtml(str) {
  const div = document.createElement('div');
  div.textContent = str ?? '';
  return div.innerHTML;
}

function productIcon(categoryName) {
  const icons = {
    'Power Tools': '🔌', 'Hand Tools': '🔨', 'Construction Materials': '🧱',
    'Safety Equipment': '🦺', 'Painting Tools': '🎨', 'Electrical Tools': '💡',
    'Plumbing Tools': '🔧', 'Hardware': '🔩'
  };
  return icons[categoryName] || '📦';
}

/**
 * Renders a product's real photo (imageUrl) inside its container, falling
 * back to the category emoji if the image is missing or fails to load
 * (e.g. offline, or the source image was moved/removed).
 */
function productImageHtml(imageUrl, categoryName) {
  if (!imageUrl) return productIcon(categoryName);
  const fallback = productIcon(categoryName).replace(/'/g, "\\'");
  return `<img src="${escapeHtml(imageUrl)}" alt="${escapeHtml(categoryName || 'Product')}"
    style="width:100%;height:100%;object-fit:cover;"
    onerror="this.onerror=null; this.replaceWith(Object.assign(document.createElement('span'), {textContent: '${fallback}'}));">`;
}
