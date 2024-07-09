import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreComponent } from './store.component';
import { ProductItemComponent } from './product-item/product-item.component';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { FeatureRateComponent } from '../shared/components/feature-rate';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { StoreRoutingModule } from './store-routing.module';
import { ProductBrandFilterComponent } from './product-brand-filter/product-brand-filter.component';
import { ProductBrandFilterV0Component } from './product-brand-filter/product-brand-filter-v0/product-brand-filter-v0.component';
import { ProductBrandFilterV1Component } from './product-brand-filter/product-brand-filter-v1/product-brand-filter-v1.component';
import { ABAnalyticsModule } from '@ips-ag/abtesting';

@NgModule({
  declarations: [
    StoreComponent,
    ProductItemComponent,
    ProductDetailsComponent,
    ProductBrandFilterComponent,
    ProductBrandFilterV0Component,
    ProductBrandFilterV1Component,
  ],
  imports: [
    CommonModule,
    StoreRoutingModule,
    FormsModule,
    SharedModule,
    ABAnalyticsModule.forChild(),
    FeatureRateComponent,
  ],
})
export class StoreModule {}
