import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductBrandFilterV1Component } from './product-brand-filter-v1.component';

describe('ProductBrandFilterV1Component', () => {
  let component: ProductBrandFilterV1Component;
  let fixture: ComponentFixture<ProductBrandFilterV1Component>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProductBrandFilterV1Component]
    });
    fixture = TestBed.createComponent(ProductBrandFilterV1Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
