import { ProductBrandFilterV0Component } from '../product-brand-filter-v0/product-brand-filter-v0.component';
import { ProductBrandFilterV1Component } from '../product-brand-filter-v1/product-brand-filter-v1.component';

export const PRODUCT_BRAND_FILTER_FEEDBACK_ENABLED_VERSIONS = ['0.1.1'];
export const PRODUCT_BRAND_FILTER_COMPONENT_MAPS = {
  '0.0.1': ProductBrandFilterV0Component,
  '0.1.1': ProductBrandFilterV1Component,
};
export type ProductBrandFilterVersion =
  keyof typeof PRODUCT_BRAND_FILTER_COMPONENT_MAPS;
