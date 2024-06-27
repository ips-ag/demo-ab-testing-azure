import { EventEmitter } from '@angular/core';
import { Brand } from 'src/app/shared/models/brand';
import { StoreParams } from 'src/app/shared/models/storeParams';
import { ProductBrandFilterV0Component } from './product-brand-filter-v0/product-brand-filter-v0.component';
import { ProductBrandFilterV1Component } from './product-brand-filter-v1/product-brand-filter-v1.component';

export interface IProductBrandFilterComponent {
  brands: Brand[];
  params: StoreParams;
  brandSelected: EventEmitter<number[]>;
  selectBrand(brandIds: number[]): void;
}
export const PRODUCT_BRAND_FILTER_COMPONENT_MAPS = {
  '0.0.1': ProductBrandFilterV0Component,
  '0.1.1': ProductBrandFilterV1Component,
};
export const PRODUCT_BRAND_FILTER_FEEDBACK_ENABLED_VERSIONS = ['0.1.1'];
