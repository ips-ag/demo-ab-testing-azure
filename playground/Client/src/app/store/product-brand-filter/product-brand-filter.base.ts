import { EventEmitter } from '@angular/core';
import { Brand } from 'src/app/shared/models/brand';
import { StoreParams } from 'src/app/shared/models/storeParams';

export interface IProductBrandFilterComponent {
  brands: Brand[];
  params: StoreParams;
  brandSelected: EventEmitter<number[]>;
  selectBrand(brandIds: number[]): void;
}
