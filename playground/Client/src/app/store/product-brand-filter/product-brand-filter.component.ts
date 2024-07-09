import {
  Component,
  EventEmitter,
  Input,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Brand } from 'src/app/shared/models/brand';
import { StoreParams } from 'src/app/shared/models/storeParams';
import {
  IProductBrandFilterComponent,
  PRODUCT_BRAND_FILTER_COMPONENT_MAPS,
  PRODUCT_BRAND_FILTER_FEEDBACK_ENABLED_VERSIONS,
} from './product-brand-filter.base';
import { DynamicComponentLoaderDirective } from 'src/app/shared/directives/dynamic-component-loader.directive';
import { SettingsService } from 'src/app/settings/settings.service';
import { Observable, Subject, share, startWith, takeUntil } from 'rxjs';
import { FeatureFlags } from 'src/app/shared/models/featureFlag';
import { GoogleAnalyticsService } from 'src/app/analytics/services/google-analytics.service';
import { STABLE_SOFTWARE_VERSION } from 'src/app/account/constants';

type ProductBrandFilterVersion =
  keyof typeof PRODUCT_BRAND_FILTER_COMPONENT_MAPS;

@Component({
  selector: 'app-product-brand-filter',
  templateUrl: './product-brand-filter.component.html',
  styleUrls: ['./product-brand-filter.component.scss'],
})
export class ProductBrandFilterComponent
  implements IProductBrandFilterComponent
{
  @Input() brands: Brand[] = [];
  @Input() params: StoreParams = new StoreParams();
  @Output() brandSelected: EventEmitter<number[]> = new EventEmitter<
    number[]
  >();
  @ViewChild(DynamicComponentLoaderDirective, { static: true })
  dynamicComponentLoader!: DynamicComponentLoaderDirective;
  defaultVersion = '0.0.1' as ProductBrandFilterVersion;
  showingVersion = '0.0.1' as ProductBrandFilterVersion;
  get shouldShowFeedback() {
    return PRODUCT_BRAND_FILTER_FEEDBACK_ENABLED_VERSIONS.includes(
      this.showingVersion
    );
  }
  private featureFlags$: Observable<FeatureFlags>;
  private destroy$ = new Subject<void>();
  constructor(
    private settingsService: SettingsService,
    private googleAnalyticsService: GoogleAnalyticsService
  ) {
    this.featureFlags$ = this.settingsService.getFeatureFlags().pipe(
      takeUntil(this.destroy$),
      startWith({
        ShopFilterVersion: this.defaultVersion,
      }),
      share()
    );
  }

  ngOnInit() {
    this.featureFlags$.subscribe((featureFlags) => {
      this.loadComponent(
        featureFlags.ShopFilterVersion as ProductBrandFilterVersion,
        this.brands,
        this.params
      );
    });
  }
  ngOnChanges(changes: SimpleChanges) {
    // Check if the changes are related to the inputs you care about
    if (changes['brands'] || changes['params']) {
      this.featureFlags$.subscribe((featureFlags) => {
        this.loadComponent(
          featureFlags.ShopFilterVersion as ProductBrandFilterVersion,
          changes['brands']?.currentValue ?? this.brands,
          changes['params']?.currentValue ?? this.params
        );
      });
    }
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
  loadComponent(
    version: ProductBrandFilterVersion,
    brands: Brand[],
    params: StoreParams
  ) {
    this.showingVersion = version;
    const componentClass = PRODUCT_BRAND_FILTER_COMPONENT_MAPS[version];
    const viewContainerRef = this.dynamicComponentLoader.viewContainerRef;
    viewContainerRef.clear();

    // Create the component and get the instance
    const componentRef = viewContainerRef.createComponent(componentClass);

    // Set input properties on the component instance
    componentRef.instance.brands = brands;
    componentRef.instance.params = params;

    // If the dynamically loaded component has an output property, subscribe to it
    if (componentRef.instance.brandSelected) {
      componentRef.instance.brandSelected.subscribe((brandIds: number[]) => {
        this.selectBrand(brandIds);
      });
    }

    if (this.shouldShowFeedback) {
      this.googleAnalyticsService.trackBounceRate(
        'ProductBrandFilter@' + this.showingVersion
      );
    }
    //
    this.googleAnalyticsService.trackUserGroup(
      this.showingVersion === STABLE_SOFTWARE_VERSION? 'BaseGroup': 'ControlGroup'
    );
  }

  selectBrand(brandIds: number[]) {
    console.log('Brand selected', brandIds);
    this.brandSelected.emit(brandIds);
  }
}
