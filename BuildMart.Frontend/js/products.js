renderLayout('products');

const params = new URLSearchParams(window.location.search);
const state = {
  search: params.get('search') || '',
  categoryId: params.get('category') || '',
  brand: '',
  minPrice: '',
  maxPrice: '',
  isAvailable: false,
  sortBy: 'newest',
  page: 1,
  pageSize: 12
};

function productCardHtml(p) {
  const hasDiscount = p.discountPrice && p.discountPrice < p.price;
  const outOfStock = !p.isAvailable || p.stockQuantity === 0;
  return `
    <a href="product-details.html?id=${p.id}" class="product-card">
      <div class="product-thumb">
        ${hasDiscount ? `<span class="badge-discount">-${Math.round((1 - p.discountPrice / p.price) * 100)}%</span>` : ''}
        ${outOfStock ? `<span class="badge-outofstock">Out of stock</span>` : ''}
        ${productImageHtml(p.imageUrl, p.categoryName)}
      </div>
      <div class="product-info">
        <div class="brand">${escapeHtml(p.brand || p.categoryName)}</div>
        <div class="name">${escapeHtml(p.name)}</div>
        <div class="price-row">
          <span class="price-now">${money(p.effectivePrice)}</span>
          ${hasDiscount ? `<span class="price-old">${money(p.price)}</span>` : ''}
        </div>
      </div>
    </a>`;
}

async function loadCategoryFilters() {
  const container = document.getElementById('filter-categories');
  try {
    const categories = await Api.getCategories();
    container.innerHTML = `
      <label><input type="radio" name="cat" value="" ${!state.categoryId ? 'checked' : ''} onchange="setCategory('')"> All Categories</label>
      ${categories.map(c => `
        <label><input type="radio" name="cat" value="${c.id}" ${String(c.id) === String(state.categoryId) ? 'checked' : ''} onchange="setCategory(${c.id})"> ${escapeHtml(c.name)} (${c.productCount})</label>
      `).join('')}`;

    const brandSelect = document.getElementById('filter-brand');
    const allProducts = await Api.getProducts('pageSize=100');
    const brands = [...new Set(allProducts.items.map(p => p.brand).filter(Boolean))].sort();
    brandSelect.innerHTML = `<option value="">All brands</option>` + brands.map(b => `<option value="${escapeHtml(b)}">${escapeHtml(b)}</option>`).join('');
  } catch (e) {
    container.innerHTML = 'Could not load categories.';
  }
}

function setCategory(id) {
  state.categoryId = id;
  state.page = 1;
  fetchProducts();
}

function applyFilters() {
  state.brand = document.getElementById('filter-brand').value;
  state.minPrice = document.getElementById('filter-min-price').value;
  state.maxPrice = document.getElementById('filter-max-price').value;
  state.isAvailable = document.getElementById('filter-available').checked;
  state.page = 1;
  fetchProducts();
}

function clearFilters() {
  state.brand = ''; state.minPrice = ''; state.maxPrice = ''; state.isAvailable = false; state.categoryId = '';
  document.getElementById('filter-brand').value = '';
  document.getElementById('filter-min-price').value = '';
  document.getElementById('filter-max-price').value = '';
  document.getElementById('filter-available').checked = false;
  document.querySelector('input[name="cat"][value=""]').checked = true;
  fetchProducts();
}

function onSortChange() {
  state.sortBy = document.getElementById('sort-select').value;
  fetchProducts();
}

function goToPage(page) {
  state.page = page;
  fetchProducts();
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

function buildQueryString() {
  const qp = new URLSearchParams();
  if (state.search) qp.set('search', state.search);
  if (state.categoryId) qp.set('categoryId', state.categoryId);
  if (state.brand) qp.set('brand', state.brand);
  if (state.minPrice) qp.set('minPrice', state.minPrice);
  if (state.maxPrice) qp.set('maxPrice', state.maxPrice);
  if (state.isAvailable) qp.set('isAvailable', 'true');
  qp.set('sortBy', state.sortBy);
  qp.set('page', state.page);
  qp.set('pageSize', state.pageSize);
  return qp.toString();
}

function renderPagination(result) {
  const el = document.getElementById('pagination');
  if (result.totalPages <= 1) { el.innerHTML = ''; return; }
  let html = '';
  for (let i = 1; i <= result.totalPages; i++) {
    html += `<button class="${i === result.page ? 'active' : ''}" onclick="goToPage(${i})">${i}</button>`;
  }
  el.innerHTML = html;
}

async function fetchProducts() {
  const grid = document.getElementById('products-grid');
  grid.innerHTML = `<div class="spinner">Loading products...</div>`;
  try {
    const result = await Api.getProducts(buildQueryString());
    document.getElementById('results-count').textContent = `${result.totalCount} product(s) found`;
    grid.innerHTML = result.items.length
      ? result.items.map(productCardHtml).join('')
      : `<div class="empty-state"><div class="icon">🔍</div>No products match your filters.</div>`;
    renderPagination(result);
  } catch (e) {
    grid.innerHTML = `<div class="empty-state">Could not load products. Is the API running?</div>`;
  }
}

loadCategoryFilters();
fetchProducts();
