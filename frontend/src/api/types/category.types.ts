import type { Status } from './common.types';

export interface Category {
  id: string;
  name: string;
  description: string;
  status: Status;
  createdAt: string;
}

export interface CreateCategoryDto {
  name: string;
  description: string;
  status: Status;
}

export type UpdateCategoryDto = CreateCategoryDto;
