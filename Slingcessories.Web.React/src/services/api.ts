import { Accessory, CreateAccessory } from '../types';

const API_BASE_URL = '/api';

export const accessoriesApi = {
  getAll: async (wishlist?: boolean): Promise<Accessory[]> => {
    const params = new URLSearchParams();
    if (wishlist !== undefined) {
      params.append('wishlist', wishlist.toString());
    }
    
    const url = `${API_BASE_URL}/accessories${params.toString() ? '?' + params.toString() : ''}`;
    const response = await fetch(url);
    
    if (!response.ok) {
      throw new Error('Failed to fetch accessories');
    }
    
    return response.json();
  },

  getById: async (id: number): Promise<Accessory> => {
    const response = await fetch(`${API_BASE_URL}/accessories/${id}`);
    
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
    });
    
    if (!response.ok) {
      throw new Error('Failed to delete accessory');
    }
  },
};
