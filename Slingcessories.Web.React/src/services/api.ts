import { Accessory, CreateAccessory } from '../types';
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
};
