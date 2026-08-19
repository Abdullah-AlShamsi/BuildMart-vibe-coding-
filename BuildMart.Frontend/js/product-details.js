renderLayout('products');

const productId = new URLSearchParams(window.location.search).get('id');

function stockLabel(p) {
  if (!p.isAvailable || p.stockQuantity === 0) return `<span class="stock-out">Out of stock</span>`;
  if (p.stockQuantity <= 10) return `<span class="stock-low">Only ${p.stockQuantity} left in stock</span>`;
  return `<span class="stock-ok">In stock (${p.stockQuantity} available)</span>`;
}

function changeQty(delta) {
  const input = document.getElementById('qty-input');
  const max = parseInt(input.max, 10) || 1;
  let value = (parseInt(input.value, 10) || 1) + delta;
  value = Math.max(1, Math.min(value, max));
  input.value = value;
}

async function addToCart(productId, maxStock) {
  if (!Auth.isLoggedIn()) {
    window.location.href = `login.html?redirect=product-details.html?id=${productId}`;
    return;
  }
  const qty = Math.max(1, Math.min(parseInt(document.getElementById('qty-input').value, 10) || 1, maxStock));
  const btn = document.getElementById('add-to-cart-btn');
  btn.disabled = true;
  btn.textContent = 'Adding...';
  try {
    await Api.addToCart({ productId, quantity: qty });
    btn.textContent = '✓ Added to Cart';
    refreshCartBadge();
    setTimeout(() => { btn.disabled = false; btn.textContent = 'Add to Cart'; }, 1500);
  } catch (e) {
    alert(e.message);
    btn.disabled = false;
    btn.textContent = 'Add to Cart';
  }
}

async function loadProduct() {
  const container = document.getElementById('pd-container');
  try {
    const p = await Api.getProduct(productId);
    const hasDiscount = p.discountPrice && p.discountPrice < p.price;
    const outOfStock = !p.isAvailable || p.stockQuantity === 0;

    container.innerHTML = `
      <div class="pd-layout">
        <div class="pd-image">${productImageHtml(p.imageUrl, p.categoryName)}</div>
        <div class="pd-info">
          <div class="pd-meta"><a href="products.html?category=${p.categoryId}">${escapeHtml(p.categoryName)}</a> · SKU: ${escapeHtml(p.sku)} ${p.brand ? '· Brand: ' + escapeHtml(p.brand) : ''}</div>
          <h1>${escapeHtml(p.name)}</h1>
          <div class="pd-price">
            ${money(p.effectivePrice)}
            ${hasDiscount ? `<span class="price-old">${money(p.price)}</span>` : ''}
          </div>
          <div class="pd-stock">${stockLabel(p)}</div>

          <div class="qty-selector">
            <button onclick="changeQty(-1)" ${outOfStock ? 'disabled' : ''}>−</button>
            <input type="number" id="qty-input" value="1" min="1" max="${p.stockQuantity}" ${outOfStock ? 'disabled' : ''}>
            <button onclick="changeQty(1)" ${outOfStock ? 'disabled' : ''}>+</button>
          </div>

          <button id="add-to-cart-btn" class="btn btn-primary" ${outOfStock ? 'disabled' : ''}
            onclick="addToCart(${p.id}, ${p.stockQuantity})">
            ${outOfStock ? 'Out of Stock' : 'Add to Cart'}
          </button>

          <div class="pd-desc">
            <h3>Description</h3>
            <p>${escapeHtml(p.description || 'No description available.')}</p>
            <p><strong>Unit:</strong> ${escapeHtml(p.unit)}${p.weight ? ' · <strong>Weight:</strong> ' + p.weight + ' kg' : ''}</p>
          </div>
        </div>
      </div>`;
  } catch (e) {
    container.innerHTML = `<div class="empty-state"><div class="icon">😕</div>Product not found.</div>`;
  }
}

loadProduct();
