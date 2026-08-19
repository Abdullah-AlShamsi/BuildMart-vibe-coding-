requireAuth(['Admin']);
renderLayout('');

let categoriesCache = [];

function switchTab(tab) {
  document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  document.querySelectorAll('.admin-panel').forEach(p => p.classList.remove('active'));
  event.target.classList.add('active');
  document.getElementById(`panel-${tab}`).classList.add('active');

  if (tab === 'dashboard') loadDashboard();
  if (tab === 'products') loadProducts();
  if (tab === 'categories') loadCategories();
  if (tab === 'orders') loadOrders();
  if (tab === 'customers') loadCustomers();
}

// ---------- Dashboard ----------
async function loadDashboard() {
  const el = document.getElementById('stats-container');
  try {
    const s = await Api.getDashboard();
    el.innerHTML = `
      <div class="stats-grid">
        <div class="stat-card accent"><div class="label">Total Sales</div><div class="value">${money(s.totalSales)}</div></div>
        <div class="stat-card"><div class="label">Total Orders</div><div class="value">${s.totalOrders}</div></div>
        <div class="stat-card"><div class="label">Total Products</div><div class="value">${s.totalProducts}</div></div>
        <div class="stat-card"><div class="label">Total Customers</div><div class="value">${s.totalCustomers}</div></div>
        <div class="stat-card"><div class="label">Pending Orders</div><div class="value">${s.pendingOrders}</div></div>
      </div>
      <div class="cart-layout">
        <div class="summary-card">
          <h3>Low Stock Products</h3>
          ${s.lowStockProducts.length
            ? `<table class="data-table"><thead><tr><th>Product</th><th>SKU</th><th>Stock</th></tr></thead><tbody>
                ${s.lowStockProducts.map(p => `<tr><td>${escapeHtml(p.name)}</td><td>${escapeHtml(p.sku)}</td><td>${p.stockQuantity}</td></tr>`).join('')}
               </tbody></table>`
            : '<p style="color:var(--color-text-muted);">All products are well stocked.</p>'}
        </div>
        <div class="summary-card">
          <h3>Recent Orders</h3>
          ${s.recentOrders.length
            ? `<table class="data-table"><thead><tr><th>#</th><th>Customer</th><th>Total</th><th>Status</th></tr></thead><tbody>
                ${s.recentOrders.map(o => `<tr><td>#${o.id}</td><td>${escapeHtml(o.customerName)}</td><td>${money(o.totalAmount)}</td><td><span class="status-pill status-${o.orderStatus}">${o.orderStatus}</span></td></tr>`).join('')}
               </tbody></table>`
            : '<p style="color:var(--color-text-muted);">No orders yet.</p>'}
        </div>
      </div>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">Could not load dashboard stats.</div>`;
  }
}

// ---------- Products ----------
async function loadProducts() {
  const el = document.getElementById('products-table');
  try {
    const result = await Api.getProducts('pageSize=100&sortBy=newest');
    el.innerHTML = `
      <table class="data-table">
        <thead><tr><th>Name</th><th>SKU</th><th>Category</th><th>Price</th><th>Stock</th><th>Status</th><th></th></tr></thead>
        <tbody>
          ${result.items.map(p => `
            <tr>
              <td>${escapeHtml(p.name)}</td>
              <td>${escapeHtml(p.sku)}</td>
              <td>${escapeHtml(p.categoryName)}</td>
              <td>${money(p.effectivePrice)}</td>
              <td>${p.stockQuantity}</td>
              <td>${p.isAvailable ? '<span class="status-pill status-Delivered">Active</span>' : '<span class="status-pill status-Cancelled">Hidden</span>'}</td>
              <td>
                <button class="btn btn-outline btn-sm" onclick='openProductModal(${JSON.stringify(p)})'>Edit</button>
                <button class="btn btn-danger btn-sm" onclick="deleteProduct(${p.id})">Delete</button>
              </td>
            </tr>`).join('')}
        </tbody>
      </table>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">Could not load products.</div>`;
  }
}

async function populateCategorySelect() {
  categoriesCache = await Api.getCategories();
  document.getElementById('pm-category').innerHTML =
    categoriesCache.map(c => `<option value="${c.id}">${escapeHtml(c.name)}</option>`).join('');
}

async function openProductModal(product) {
  if (!categoriesCache.length) await populateCategorySelect();
  document.getElementById('product-modal-error').style.display = 'none';
  document.getElementById('product-modal-title').textContent = product ? 'Edit Product' : 'Add Product';
  document.getElementById('pm-id').value = product?.id || '';
  document.getElementById('pm-name').value = product?.name || '';
  document.getElementById('pm-description').value = product?.description || '';
  document.getElementById('pm-price').value = product?.price ?? '';
  document.getElementById('pm-discount').value = product?.discountPrice ?? '';
  document.getElementById('pm-stock').value = product?.stockQuantity ?? '';
  document.getElementById('pm-sku').value = product?.sku || '';
  document.getElementById('pm-category').value = product?.categoryId || categoriesCache[0]?.id || '';
  document.getElementById('pm-brand').value = product?.brand || '';
  document.getElementById('pm-unit').value = product?.unit || 'Piece';
  document.getElementById('pm-weight').value = product?.weight ?? '';
  document.getElementById('pm-image').value = product?.imageUrl || '';
  document.getElementById('pm-available').checked = product ? product.isAvailable : true;
  document.getElementById('product-modal-backdrop').classList.add('open');
}

function closeProductModal() {
  document.getElementById('product-modal-backdrop').classList.remove('open');
}

async function saveProduct() {
  const id = document.getElementById('pm-id').value;
  const dto = {
    name: document.getElementById('pm-name').value.trim(),
    description: document.getElementById('pm-description').value.trim() || null,
    price: parseFloat(document.getElementById('pm-price').value),
    discountPrice: document.getElementById('pm-discount').value ? parseFloat(document.getElementById('pm-discount').value) : null,
    stockQuantity: parseInt(document.getElementById('pm-stock').value, 10),
    sku: document.getElementById('pm-sku').value.trim(),
    categoryId: parseInt(document.getElementById('pm-category').value, 10),
    brand: document.getElementById('pm-brand').value.trim() || null,
    unit: document.getElementById('pm-unit').value,
    weight: document.getElementById('pm-weight').value ? parseFloat(document.getElementById('pm-weight').value) : null,
    imageUrl: document.getElementById('pm-image').value.trim() || null,
    isAvailable: document.getElementById('pm-available').checked
  };

  const errorBox = document.getElementById('product-modal-error');
  try {
    if (id) await Api.updateProduct(id, dto);
    else await Api.createProduct(dto);
    closeProductModal();
    loadProducts();
  } catch (e) {
    errorBox.style.display = 'block';
    errorBox.textContent = e.message;
  }
}

async function deleteProduct(id) {
  if (!confirm('Delete this product?')) return;
  try {
    await Api.deleteProduct(id);
    loadProducts();
  } catch (e) { alert(e.message); }
}

// ---------- Categories ----------
async function loadCategories() {
  const el = document.getElementById('categories-table');
  try {
    const categories = await Api.getCategories();
    categoriesCache = categories;
    el.innerHTML = `
      <table class="data-table">
        <thead><tr><th>Name</th><th>Description</th><th>Products</th><th></th></tr></thead>
        <tbody>
          ${categories.map(c => `
            <tr>
              <td>${escapeHtml(c.name)}</td>
              <td>${escapeHtml(c.description || '')}</td>
              <td>${c.productCount}</td>
              <td>
                <button class="btn btn-outline btn-sm" onclick='openCategoryModal(${JSON.stringify(c)})'>Edit</button>
                <button class="btn btn-danger btn-sm" onclick="deleteCategory(${c.id})">Delete</button>
              </td>
            </tr>`).join('')}
        </tbody>
      </table>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">Could not load categories.</div>`;
  }
}

function openCategoryModal(category) {
  document.getElementById('category-modal-error').style.display = 'none';
  document.getElementById('category-modal-title').textContent = category ? 'Edit Category' : 'Add Category';
  document.getElementById('cm-id').value = category?.id || '';
  document.getElementById('cm-name').value = category?.name || '';
  document.getElementById('cm-description').value = category?.description || '';
  document.getElementById('cm-image').value = category?.imageUrl || '';
  document.getElementById('category-modal-backdrop').classList.add('open');
}

function closeCategoryModal() {
  document.getElementById('category-modal-backdrop').classList.remove('open');
}

async function saveCategory() {
  const id = document.getElementById('cm-id').value;
  const dto = {
    name: document.getElementById('cm-name').value.trim(),
    description: document.getElementById('cm-description').value.trim() || null,
    imageUrl: document.getElementById('cm-image').value.trim() || null
  };
  const errorBox = document.getElementById('category-modal-error');
  try {
    if (id) await Api.updateCategory(id, dto);
    else await Api.createCategory(dto);
    closeCategoryModal();
    loadCategories();
  } catch (e) {
    errorBox.style.display = 'block';
    errorBox.textContent = e.message;
  }
}

async function deleteCategory(id) {
  if (!confirm('Delete this category? This only works if it has no products.')) return;
  try {
    await Api.deleteCategory(id);
    loadCategories();
  } catch (e) { alert(e.message); }
}

// ---------- Orders ----------
const ORDER_STATUSES = ['Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

async function loadOrders() {
  const el = document.getElementById('orders-table');
  try {
    const orders = await Api.getOrders();
    el.innerHTML = `
      <table class="data-table">
        <thead><tr><th>#</th><th>Customer</th><th>Date</th><th>Total</th><th>Status</th></tr></thead>
        <tbody>
          ${orders.map(o => `
            <tr>
              <td><a href="order-details.html?id=${o.id}">#${o.id}</a></td>
              <td>${escapeHtml(o.customerName)}<br><small style="color:var(--color-text-muted);">${escapeHtml(o.customerEmail)}</small></td>
              <td>${new Date(o.orderDate).toLocaleDateString()}</td>
              <td>${money(o.totalAmount)}</td>
              <td>
                <select onchange="updateOrderStatus(${o.id}, this.value)">
                  ${ORDER_STATUSES.map(s => `<option value="${s}" ${s === o.orderStatus ? 'selected' : ''}>${s}</option>`).join('')}
                </select>
              </td>
            </tr>`).join('')}
        </tbody>
      </table>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">Could not load orders.</div>`;
  }
}

async function updateOrderStatus(id, status) {
  try {
    await Api.updateOrderStatus(id, { orderStatus: status });
    loadOrders();
  } catch (e) {
    alert(e.message);
    loadOrders();
  }
}

// ---------- Customers ----------
async function loadCustomers() {
  const el = document.getElementById('customers-table');
  try {
    const customers = await Api.getCustomers();
    el.innerHTML = `
      <table class="data-table">
        <thead><tr><th>Name</th><th>Email</th><th>Phone</th><th>Joined</th><th></th></tr></thead>
        <tbody>
          ${customers.map(c => `
            <tr>
              <td>${escapeHtml(c.fullName)}</td>
              <td>${escapeHtml(c.email)}</td>
              <td>${escapeHtml(c.phoneNumber || '—')}</td>
              <td>${new Date(c.createdAt).toLocaleDateString()}</td>
              <td><button class="btn btn-outline btn-sm" onclick="viewCustomerOrders('${c.id}', '${escapeHtml(c.fullName)}')">View Orders</button></td>
            </tr>`).join('')}
        </tbody>
      </table>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">Could not load customers.</div>`;
  }
}

async function viewCustomerOrders(id, name) {
  try {
    const orders = await Api.getCustomerOrders(id);
    alert(`${name} has placed ${orders.length} order(s). Total spent: ${money(orders.reduce((s, o) => s + o.totalAmount, 0))}`);
  } catch (e) { alert(e.message); }
}

loadDashboard();
