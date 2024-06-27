import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductBrandFilterComponent } from './product-brand-filter.component';

describe('ProductBrandFilterComponent', () => {
  let component: ProductBrandFilterComponent;
  let fixture: ComponentFixture<ProductBrandFilterComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProductBrandFilterComponent]
    });
    fixture = TestBed.createComponent(ProductBrandFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
