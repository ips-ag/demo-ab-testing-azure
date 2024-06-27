import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Brand } from 'src/app/shared/models/brand';
import { StoreParams } from 'src/app/shared/models/storeParams';
import { IProductBrandFilterComponent } from '../product-brand-filter.base';

@Component({
  selector: 'app-product-brand-filter-v0',
  templateUrl: './product-brand-filter-v0.component.html',
  styleUrls: ['./product-brand-filter-v0.component.scss'],
})
export class ProductBrandFilterV0Component
  implements IProductBrandFilterComponent
{
  @Input() brands: Brand[] = [];
  @Input() params: StoreParams = new StoreParams();
  @Output() brandSelected: EventEmitter<number[]> = new EventEmitter<
    number[]
  >();
  selectBrand(brandIds: number[]) {
    this.brandSelected.emit(brandIds);
  }
}
