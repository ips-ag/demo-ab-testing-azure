import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Brand } from 'src/app/shared/models/brand';
import { min, max } from 'lodash';
import { StoreParams } from 'src/app/shared/models/storeParams';
import { IProductBrandFilterComponent } from '../product-brand-filter.base';
import {
  BehaviorSubject,
  Observable,
  Subject,
  combineLatest,
  debounceTime,
  map,
  takeUntil,
} from 'rxjs';

@Component({
  selector: 'app-product-brand-filter-v1',
  templateUrl: './product-brand-filter-v1.component.html',
  styleUrls: ['./product-brand-filter-v1.component.scss'],
})
export class ProductBrandFilterV1Component
  implements IProductBrandFilterComponent, OnInit, OnDestroy, OnChanges
{
  @Input() brands: Brand[] = [];
  @Input() params: StoreParams = new StoreParams();
  @Output() brandSelected: EventEmitter<number[]> = new EventEmitter<
    number[]
  >();
  minSliderValue$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  maxSliderValue$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  overrideBrands$: Observable<number[]>;
  selectedBrands$: BehaviorSubject<number[]> = new BehaviorSubject<number[]>(
    []
  );
  brands$: BehaviorSubject<Brand[]> = new BehaviorSubject<Brand[]>([]);
  destroy$: Subject<void> = new Subject<void>();
  get sliderOptions() {
    return {
      vertical: true,
      floor: 0,
      ceil: this.brands.length - 1,
      showTicks: true,
      showTicksValues: false,
      rightToLeft: true,
    };
  }
  constructor() {
    this.overrideBrands$ = combineLatest([
      this.brands$,
      this.minSliderValue$,
      this.maxSliderValue$,
    ]).pipe(
      takeUntil(this.destroy$),
      map(([brands, minSliderValue, maxSliderValue]) =>
        brands.slice(minSliderValue, maxSliderValue + 1).map((b) => b.id)
      )
    );

    combineLatest([
      this.overrideBrands$,
      this.minSliderValue$,
      this.maxSliderValue$,
    ])
      .pipe(
        takeUntil(this.destroy$),
        map(([overrideBrands, minSliderValue, maxSliderValue]) => {
          if (
            minSliderValue > (min(this.params?.productBrandIds) ?? 0) ||
            maxSliderValue <
              (max(this.params?.productBrandIds) ?? this.brands.length - 1)
          ) {
            return overrideBrands;
          }
          return [
            ...new Set([
              ...this.params.productBrandIds,
              ...overrideBrands,
            ]).values(),
          ];
        })
      )
      .subscribe(this.selectedBrands$);

    this.selectedBrands$
      .pipe(takeUntil(this.destroy$), debounceTime(100))
      .subscribe((brandIds) => {
        this.brandSelected.emit(brandIds);
      });
  }
  ngOnInit() {
    this.brands$.next(this.brands);
  }
  ngOnChanges(changes: SimpleChanges) {
    if (changes['brands']) {
      this.brands$.next(changes['brands'].currentValue);
    }
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  selectBrand(brandIds: number[]) {
    this.minSliderValue$.next(min(brandIds) ?? 0);
    this.maxSliderValue$.next(max(brandIds) ?? this.brands.length - 1);
  }
}
