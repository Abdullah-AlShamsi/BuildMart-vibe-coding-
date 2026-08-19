renderLayout('home');

function productCardHtml(p) {
  const hasDiscount = p.discountPrice && p.discountPrice < p.price;
  return `
    <a href="product-details.html?id=${p.id}" class="product-card">
      <div class="product-thumb">
        ${hasDiscount ? `<span class="badge-discount">-${Math.round((1 - p.discountPrice / p.price) * 100)}%</span>` : ''}
        ${!p.isAvailable || p.stockQuantity === 0 ? `<span class="badge-outofstock">Out of stock</span>` : ''}
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

async function loadCategories() {
  const grid = document.getElementById('categories-grid');
  try {
    const categories = await Api.getCategories();
    grid.innerHTML = categories.map(c => `
      <a href="products.html?category=${c.id}" class="category-card">
        <div class="icon" style="width:48px;height:48px;margin:0 auto 10px;border-radius:6px;overflow:hidden;">${productImageHtml(c.imageUrl, c.name)}</div>
        <div class="name">${escapeHtml(c.name)}</div>
      </a>`).join('');
  } catch (e) {
    grid.innerHTML = `<div class="empty-state">Could not load categories.</div>`;
  }
}

async function loadFeatured() {
  const grid = document.getElementById('featured-grid');
  try {
    const result = await Api.getProducts('sortBy=newest&pageSize=8');
    grid.innerHTML = result.items.length
      ? result.items.map(productCardHtml).join('')
      : `<div class="empty-state">No products yet.</div>`;
  } catch (e) {
    grid.innerHTML = `<div class="empty-state">Could not load products. Is the API running?</div>`;
  }
}

async function loadOffers() {
  const grid = document.getElementById('offers-grid');
  try {
    const result = await Api.getProducts('pageSize=50');
    const discounted = result.items.filter(p => p.discountPrice && p.discountPrice < p.price).slice(0, 4);
    grid.innerHTML = discounted.length
      ? discounted.map(productCardHtml).join('')
      : `<div class="empty-state">No special offers right now — check back soon!</div>`;
  } catch (e) {
    grid.innerHTML = `<div class="empty-state">Could not load offers.</div>`;
  }
}

loadCategories();
loadFeatured();
loadOffers();
