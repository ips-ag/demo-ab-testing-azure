import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductBrandFilterV0Component } from './product-brand-filter-v0.component';

describe('ProductBrandFilterV0Component', () => {
  let component: ProductBrandFilterV0Component;
  let fixture: ComponentFixture<ProductBrandFilterV0Component>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProductBrandFilterV0Component]
    });
    fixture = TestBed.createComponent(ProductBrandFilterV0Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
