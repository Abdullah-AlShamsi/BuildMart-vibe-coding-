requireAuth();
renderLayout('');

const orderId = new URLSearchParams(window.location.search).get('id');
const justPlaced = new URLSearchParams(window.location.search).get('placed') === '1';

async function loadOrder() {
  const container = document.getElementById('order-details-container');
  try {
    const o = await Api.getOrder(orderId);
    container.innerHTML = `
      ${justPlaced ? `<div class="alert alert-success">🎉 Your order has been placed successfully!</div>` : ''}
      <div class="section-head">
        <h1>Order #${o.id}</h1>
        <span class="status-pill status-${o.orderStatus}">${o.orderStatus}</span>
      </div>
      <div class="cart-layout">
        <div>
          <table class="data-table">
            <thead><tr><th>Product</th><th>Qty</th><th>Unit Price</th><th>Total</th></tr></thead>
            <tbody>
              ${o.items.map(i => `
                <tr>
                  <td>${escapeHtml(i.productName)}</td>
                  <td>${i.quantity}</td>
                  <td>${money(i.unitPrice)}</td>
                  <td>${money(i.totalPrice)}</td>
                </tr>`).join('')}
            </tbody>
          </table>
        </div>
        <div class="summary-card">
          <h3>Order Info</h3>
          <div class="summary-row"><span>Order Date</span><span>${new Date(o.orderDate).toLocaleString()}</span></div>
          <div class="summary-row"><span>Payment Method</span><span>${o.paymentMethod}</span></div>
          <div class="summary-row"><span>Payment Status</span><span>${o.paymentStatus}</span></div>
          <div class="summary-row"><span>Shipping Address</span><span style="text-align:right;">${escapeHtml(o.shippingAddress)}</span></div>
          <div class="summary-row"><span>Phone</span><span>${escapeHtml(o.phoneNumber)}</span></div>
          <div class="summary-row total"><span>Total</span><span>${money(o.totalAmount)}</span></div>
        </div>
      </div>`;
  } catch (e) {
    container.innerHTML = `<div class="empty-state">Could not load this order (${e.message}).</div>`;
  }
}

loadOrder();
