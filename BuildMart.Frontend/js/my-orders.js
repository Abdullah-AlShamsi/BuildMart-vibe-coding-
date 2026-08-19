requireAuth(['Customer']);
renderLayout('');

async function loadOrders() {
  const container = document.getElementById('orders-container');
  try {
    const orders = await Api.getOrders();
    if (!orders.length) {
      container.innerHTML = `
        <div class="empty-state">
          <div class="icon">📦</div>
          You haven't placed any orders yet.
          <p><a href="products.html" class="btn btn-primary" style="margin-top:14px;">Start Shopping</a></p>
        </div>`;
      return;
    }

    container.innerHTML = `
      <table class="data-table">
        <thead><tr><th>Order #</th><th>Date</th><th>Items</th><th>Total</th><th>Status</th><th></th></tr></thead>
        <tbody>
          ${orders.map(o => `
            <tr>
              <td>#${o.id}</td>
              <td>${new Date(o.orderDate).toLocaleDateString()}</td>
              <td>${o.items.length} item(s)</td>
              <td>${money(o.totalAmount)}</td>
              <td><span class="status-pill status-${o.orderStatus}">${o.orderStatus}</span></td>
              <td><a href="order-details.html?id=${o.id}" class="btn btn-outline btn-sm">View</a></td>
            </tr>`).join('')}
        </tbody>
      </table>`;
  } catch (e) {
    container.innerHTML = `<div class="empty-state">Could not load your orders.</div>`;
  }
}

loadOrders();
