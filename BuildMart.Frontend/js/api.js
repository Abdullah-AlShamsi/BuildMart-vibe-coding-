/**
 * BuildMart frontend — API client.
 * Change BASE_URL if your API runs on a different port.
 */
const API_BASE_URL = 'https://buildmart.runasp.net/api';

const Auth = {
  getToken() { return localStorage.getItem('buildmart_token'); },
  setSession(token, user) {
    localStorage.setItem('buildmart_token', token);
    localStorage.setItem('buildmart_user', JSON.stringify(user));
  },
  getUser() {
    const raw = localStorage.getItem('buildmart_user');
    return raw ? JSON.parse(raw) : null;
  },
  isLoggedIn() { return !!this.getToken(); },
  isAdmin() { return this.getUser()?.role === 'Admin'; },
  logout() {
    localStorage.removeItem('buildmart_token');
    localStorage.removeItem('buildmart_user');
    window.location.href = 'login.html';
  }
};

/**
 * Thin fetch wrapper: adds the JWT header, unwraps the ApiResponse
 * envelope, and throws a readable Error on failure (with .status).
 */
async function apiRequest(path, { method = 'GET', body, auth = false } = {}) {
  const headers = { 'Content-Type': 'application/json' };
  if (auth) {
    const token = Auth.getToken();
    if (token) headers['Authorization'] = `Bearer ${token}`;
  }

  let response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined
    });
  } catch (networkError) {
    throw new Error('Could not reach the BuildMart API. Is the backend running?');
  }

  if (response.status === 204) return null;

  let payload = null;
  try { payload = await response.json(); } catch { /* empty body */ }

  if (!response.ok) {
    const message = payload?.message || `Request failed (${response.status}).`;
    const errors = payload?.errors?.length ? ` ${payload.errors.join(' ')}` : '';
    const error = new Error(message + errors);
    error.status = response.status;
    if (response.status === 401 && auth) {
      Auth.logout();
    }
    throw error;
  }

  return payload?.data ?? payload;
}

const Api = {
  // Auth
  register: (dto) => apiRequest('/auth/register', { method: 'POST', body: dto }),
  login: (dto) => apiRequest('/auth/login', { method: 'POST', body: dto }),
  me: () => apiRequest('/auth/me', { auth: true }),
  updateProfile: (dto) => apiRequest('/auth/me', { method: 'PUT', body: dto, auth: true }),

  // Categories
  getCategories: () => apiRequest('/categories'),
  getCategory: (id) => apiRequest(`/categories/${id}`),
  createCategory: (dto) => apiRequest('/categories', { method: 'POST', body: dto, auth: true }),
  updateCategory: (id, dto) => apiRequest(`/categories/${id}`, { method: 'PUT', body: dto, auth: true }),
  deleteCategory: (id) => apiRequest(`/categories/${id}`, { method: 'DELETE', auth: true }),

  // Products
  getProducts: (queryString) => apiRequest(`/products?${queryString}`),
  getProduct: (id) => apiRequest(`/products/${id}`),
  createProduct: (dto) => apiRequest('/products', { method: 'POST', body: dto, auth: true }),
  updateProduct: (id, dto) => apiRequest(`/products/${id}`, { method: 'PUT', body: dto, auth: true }),
  deleteProduct: (id) => apiRequest(`/products/${id}`, { method: 'DELETE', auth: true }),

  // Cart
  getCart: () => apiRequest('/cart', { auth: true }),
  addToCart: (dto) => apiRequest('/cart/items', { method: 'POST', body: dto, auth: true }),
  updateCartItem: (id, dto) => apiRequest(`/cart/items/${id}`, { method: 'PUT', body: dto, auth: true }),
  removeCartItem: (id) => apiRequest(`/cart/items/${id}`, { method: 'DELETE', auth: true }),
  clearCart: () => apiRequest('/cart', { method: 'DELETE', auth: true }),

  // Orders
  createOrder: (dto) => apiRequest('/orders', { method: 'POST', body: dto, auth: true }),
  getOrders: () => apiRequest('/orders', { auth: true }),
  getOrder: (id) => apiRequest(`/orders/${id}`, { auth: true }),
  updateOrderStatus: (id, dto) => apiRequest(`/orders/${id}/status`, { method: 'PUT', body: dto, auth: true }),

  // Admin
  getDashboard: () => apiRequest('/admin/dashboard', { auth: true }),
  getCustomers: () => apiRequest('/admin/customers', { auth: true }),
  getCustomerOrders: (id) => apiRequest(`/admin/customers/${id}/orders`, { auth: true })
};
