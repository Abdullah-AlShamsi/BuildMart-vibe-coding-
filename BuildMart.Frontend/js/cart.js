requireAuth(['Customer']);
renderLayout('cart');

async function changeQuantity(itemId, delta, currentQty, maxStock) {
  const newQty = currentQty + delta;
  if (newQty < 1 || newQty > maxStock) return;
  try {
    await Api.updateCartItem(itemId, { quantity: newQty });
    loadCart();
    refreshCartBadge();
  } catch (e) { alert(e.message); }
}

async function setQuantity(itemId, input, maxStock) {
  let qty = parseInt(input.value, 10);
  if (isNaN(qty) || qty < 1) qty = 1;
  if (qty > maxStock) qty = maxStock;
  try {
    await Api.updateCartItem(itemId, { quantity: qty });
    loadCart();
    refreshCartBadge();
  } catch (e) { alert(e.message); }
}

async function removeItem(itemId) {
  try {
    await Api.removeCartItem(itemId);
    loadCart();
    refreshCartBadge();
  } catch (e) { alert(e.message); }
}

async function loadCart() {
  const container = document.getElementById('cart-container');
  try {
    const cart = await Api.getCart();

    if (!cart.items.length) {
      container.innerHTML = `
        <div class="empty-state">
          <div class="icon">🛒</div>
          Your cart is empty.
          <p><a href="products.html" class="btn btn-primary" style="margin-top:14px;">Browse Products</a></p>
        </div>`;
      return;
    }

    const itemsHtml = cart.items.map(i => `
      <div class="cart-item">
        <div class="thumb">${productIcon('')}</div>
        <div>
          <div class="name">${escapeHtml(i.productName)}</div>
          <div class="unit-price">${money(i.unitPrice)} / unit</div>
        </div>
        <div class="qty-mini">
          <button onclick="changeQuantity(${i.id}, -1, ${i.quantity}, ${i.availableStock})">−</button>
          <input type="number" value="${i.quantity}" min="1" max="${i.availableStock}"
            onchange="setQuantity(${i.id}, this, ${i.availableStock})">
          <button onclick="changeQuantity(${i.id}, 1, ${i.quantity}, ${i.availableStock})">+</button>
        </div>
        <div style="font-weight:700;">${money(i.subtotal)}</div>
        <button class="remove-link" onclick="removeItem(${i.id})">Remove</button>
      </div>`).join('');

    container.innerHTML = `
      <div class="cart-layout">
        <div>${itemsHtml}</div>
        <div class="summary-card">
          <h3>Order Summary</h3>
          <div class="summary-row"><span>Subtotal</span><span>${money(cart.subtotal)}</span></div>
          <div class="summary-row"><span>Discount</span><span>-${money(cart.discount)}</span></div>
          <div class="summary-row total"><span>Total</span><span>${money(cart.total)}</span></div>
          <a href="checkout.html" class="btn btn-primary btn-block" style="margin-top:16px;">Proceed to Checkout</a>
        </div>
      </div>`;
  } catch (e) {
    container.innerHTML = `<div class="empty-state">Could not load your cart.</div>`;
  }
}

loadCart();
