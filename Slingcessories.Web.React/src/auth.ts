const TOKEN_KEY = 'authToken';

export const getAuthToken = (): string | null => localStorage.getItem(TOKEN_KEY);

export const setAuthToken = (token: string): void => localStorage.setItem(TOKEN_KEY, token);

export const clearAuthToken = (): void => localStorage.removeItem(TOKEN_KEY);

export const getAuthHeader = (): Record<string, string> => {
  const token = getAuthToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
};
