import { Accessory, Category, CreateAccessory, CreateSlingshot, RegisterUser, Slingshot, Subcategory, User } from '../types';
import { getAuthHeader, setAuthToken } from '../auth';

const API_BASE_URL = '/api';
const CLIENT_ID = 'Slingcessories.React';

export const accessoriesApi = {
  getAll: async (wishlist?: boolean): Promise<Accessory[]> => {
    const params = new URLSearchParams();
    if (wishlist !== undefined) {
      params.append('wishlist', wishlist.toString());
    }
    
    const url = `${API_BASE_URL}/accessories${params.toString() ? '?' + params.toString() : ''}`;
    const response = await fetch(url, {
      headers: {
        ...getAuthHeader(),
      },
    });
    
    if (!response.ok) {
      throw new Error('Failed to fetch accessories');
    }
    
    return response.json();
  },

  getById: async (id: number): Promise<Accessory> => {
    const response = await fetch(`${API_BASE_URL}/accessories/${id}`, {
      headers: {
        ...getAuthHeader(),
      },
    });
    
    if (!response.ok) {
      throw new Error('Failed to fetch accessory');
    }
    
    return response.json();
  },

  create: async (accessory: CreateAccessory): Promise<Accessory> => {
    const response = await fetch(`${API_BASE_URL}/accessories`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader(),
      },
      body: JSON.stringify(accessory),
    });
    
    if (!response.ok) {
      throw new Error('Failed to create accessory');
    }
    
    return response.json();
  },

  update: async (id: number, accessory: Partial<Accessory>): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/accessories/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader(),
      },
      body: JSON.stringify(accessory),
    });
    
    if (!response.ok) {
      throw new Error('Failed to update accessory');
    }
  },

  delete: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/accessories/${id}`, {
      method: 'DELETE',
      headers: {
        ...getAuthHeader(),
      },
    });
    
    if (!response.ok) {
      throw new Error('Failed to delete accessory');
    }
  },
};

type AuthResponse = {
  token: string;
  expiresAtUtc: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
};

type ForgotPasswordResponse = {
  message: string;
  resetToken?: string;
};

export type UserInfo = {
  firstName: string;
  lastName: string;
  email: string;
};

export const authApi = {
  login: async (email: string, password: string): Promise<UserInfo | null> => {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password, clientId: CLIENT_ID }),
    });

    if (!response.ok) {
      return null;
    }

    const payload = (await response.json()) as AuthResponse;
    setAuthToken(payload.token);
    return { firstName: payload.firstName, lastName: payload.lastName, email: payload.email };
  },

  forgotPassword: async (email: string): Promise<string | null> => {
    const response = await fetch(`${API_BASE_URL}/auth/forgot-password`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email }),
    });

    if (!response.ok) {
      throw new Error('Failed to request password reset');
    }

    const payload = (await response.json()) as ForgotPasswordResponse;
    return payload.resetToken ?? null;
  },

  resetPassword: async (email: string, token: string, newPassword: string): Promise<boolean> => {
    const response = await fetch(`${API_BASE_URL}/auth/reset-password`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, token, newPassword }),
    });

    return response.ok;
  },

  register: async (dto: RegisterUser): Promise<UserInfo | null> => {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Registration failed');
    }
    const payload = (await response.json()) as AuthResponse;
    setAuthToken(payload.token);
    return { firstName: payload.firstName, lastName: payload.lastName, email: payload.email };
  },
};

export const slingshotsApi = {
  getAll: async (userId?: string): Promise<Slingshot[]> => {
    const params = userId ? `?userId=${encodeURIComponent(userId)}` : '';
    const response = await fetch(`${API_BASE_URL}/slingshots${params}`, { headers: getAuthHeader() });
    if (!response.ok) throw new Error('Failed to fetch slingshots');
    return response.json();
  },

  create: async (dto: CreateSlingshot): Promise<Slingshot> => {
    const response = await fetch(`${API_BASE_URL}/slingshots`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to create slingshot');
    }
    return response.json();
  },

  update: async (id: number, dto: Slingshot): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/slingshots/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to update slingshot');
    }
  },

  delete: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/slingshots/${id}`, {
      method: 'DELETE',
      headers: getAuthHeader(),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to delete slingshot');
    }
  },
};

export const categoriesApi = {
  getAll: async (): Promise<Category[]> => {
    const response = await fetch(`${API_BASE_URL}/categories`, { headers: getAuthHeader() });
    if (!response.ok) throw new Error('Failed to fetch categories');
    return response.json();
  },

  create: async (name: string): Promise<Category> => {
    const response = await fetch(`${API_BASE_URL}/categories`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
      body: JSON.stringify({ name }),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to create category');
    }
    return response.json();
  },

  update: async (id: number, name: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/categories/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
      body: JSON.stringify({ id, name }),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to update category');
    }
  },

  delete: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/categories/${id}`, {
      method: 'DELETE',
      headers: getAuthHeader(),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to delete category');
    }
  },
};

export const subcategoriesApi = {
  getAll: async (): Promise<Subcategory[]> => {
    const response = await fetch(`${API_BASE_URL}/subcategories`, { headers: getAuthHeader() });
    if (!response.ok) throw new Error('Failed to fetch subcategories');
    return response.json();
  },

  create: async (name: string, categoryId: number): Promise<Subcategory> => {
    const response = await fetch(`${API_BASE_URL}/subcategories`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
      body: JSON.stringify({ name, categoryId }),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to create subcategory');
    }
    return response.json();
  },

  update: async (id: number, name: string, categoryId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/subcategories/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', ...getAuthHeader() },
      body: JSON.stringify({ id, name, categoryId }),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to update subcategory');
    }
  },

  delete: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/subcategories/${id}`, {
      method: 'DELETE',
      headers: getAuthHeader(),
    });
    if (!response.ok) {
      const text = await response.text();
      throw new Error(text || 'Failed to delete subcategory');
    }
  },
};

export const usersApi = {
  getAll: async (): Promise<User[]> => {
    const response = await fetch(`${API_BASE_URL}/users`, { headers: getAuthHeader() });
    if (!response.ok) throw new Error('Failed to fetch users');
    return response.json();
  },
};
