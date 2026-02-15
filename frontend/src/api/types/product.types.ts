import type { Status } from './common.types';

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  categoryName: string;
  status: Status;
  createdAt: string;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  status: Status;
}

export type UpdateProductDto = CreateProductDto;
