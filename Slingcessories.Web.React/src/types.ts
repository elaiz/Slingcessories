export interface Accessory {
  id: number;
  title: string;
  pictureUrl: string | null;
  price: number;
  url: string | null;
  wishlist: boolean;
  categoryId: number;
  subcategoryId: number | null;
  categoryName: string;
  subcategoryName: string | null;
  slingshotIds: number[] | null;
  slingshotDescriptions: string[] | null;
  slingshotQuantities: { [key: number]: number };
}

export interface CreateAccessory {
  title: string;
  pictureUrl: string | null;
  price: number;
  url: string | null;
  wishlist: boolean;
  categoryId: number;
  subcategoryId: number | null;
  slingshotQuantities: { [key: number]: number };
}

export interface Category {
  id: number;
  name: string;
}

export interface Subcategory {
  id: number;
  name: string;
  categoryId: number;
}

export interface Slingshot {
  id: number;
  year: number;
  model: string;
  color: string;
}
