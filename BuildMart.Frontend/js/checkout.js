requireAuth(['Customer']);
renderLayout('cart');

async function loadSummary() {
  const el = document.getElementById('order-summary');
  try {
    const cart = await Api.getCart();
    if (!cart.items.length) {
      el.innerHTML = `<div class="empty-state">Your cart is empty. <a href="products.html">Go shopping</a>.</div>`;
      document.getElementById('place-order-btn').disabled = true;
      return;
    }
    el.innerHTML = `
      <div class="summary-card">
        <h3>Order Summary</h3>
        ${cart.items.map(i => `
          <div class="summary-row"><span>${escapeHtml(i.productName)} × ${i.quantity}</span><span>${money(i.subtotal)}</span></div>
        `).join('')}
        <div class="summary-row"><span>Subtotal</span><span>${money(cart.subtotal)}</span></div>
        <div class="summary-row"><span>Discount</span><span>-${money(cart.discount)}</span></div>
        <div class="summary-row total"><span>Total</span><span>${money(cart.total)}</span></div>
      </div>`;
  } catch (e) {
    el.innerHTML = `<div class="empty-state">Could not load your order summary.</div>`;
  }
}

async function submitOrder(event) {
  event.preventDefault();
  const errorBox = document.getElementById('checkout-error-box');
  errorBox.style.display = 'none';

  const dto = {
    shippingAddress: document.getElementById('shipping-address').value.trim(),
    phoneNumber: document.getElementById('phone-number').value.trim(),
    paymentMethod: document.getElementById('payment-method').value
  };

  const btn = document.getElementById('place-order-btn');
  btn.disabled = true;
  btn.textContent = 'Placing order...';

  try {
    const order = await Api.createOrder(dto);
    window.location.href = `order-details.html?id=${order.id}&placed=1`;
  } catch (e) {
    errorBox.className = 'alert alert-error';
    errorBox.style.display = 'block';
    errorBox.textContent = e.message;
    btn.disabled = false;
    btn.textContent = 'Place Order';
  }
  return false;
}

loadSummary();
