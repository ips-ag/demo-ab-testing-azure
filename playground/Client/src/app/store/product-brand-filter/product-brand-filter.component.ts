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
import { IProductBrandFilterComponent } from './product-brand-filter.base';
import { SettingsService } from 'src/app/settings/settings.service';
import {
  Observable,
  Subject,
  Subscription,
  share,
  startWith,
  takeUntil,
} from 'rxjs';
import { FeatureFlags } from 'src/app/shared/models/featureFlag';
import { GoogleAnalyticsService, RenderService } from '@ips-ag/abtesting';
import { DynamicComponentLoaderDirective } from 'src/app/shared/directives/dynamic-component-loader.directive';
import {
  PRODUCT_BRAND_FILTER_COMPONENT_MAPS,
  PRODUCT_BRAND_FILTER_FEEDBACK_ENABLED_VERSIONS,
  ProductBrandFilterVersion,
} from './constants';

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
  defaultVersion: ProductBrandFilterVersion = '0.0.1';
  showingVersion: ProductBrandFilterVersion = '0.0.1';
  get shouldShowFeedback() {
    return PRODUCT_BRAND_FILTER_FEEDBACK_ENABLED_VERSIONS.includes(
      this.showingVersion
    );
  }
  private featureFlags$: Observable<FeatureFlags>;
  private featureFlagsSubscription?: Subscription;
  private destroy$ = new Subject<void>();
  constructor(
    private settingsService: SettingsService,
    private googleAnalyticsService: GoogleAnalyticsService,
    private renderService: RenderService
  ) {
    Object.keys(PRODUCT_BRAND_FILTER_COMPONENT_MAPS).forEach((version) => {
      this.renderService.registerComponent(
        version as ProductBrandFilterVersion,
        PRODUCT_BRAND_FILTER_COMPONENT_MAPS[
          version as keyof typeof PRODUCT_BRAND_FILTER_COMPONENT_MAPS
        ]
      );
    });
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
      if (this.featureFlagsSubscription) {
        this.featureFlagsSubscription.unsubscribe();
      }
      this.featureFlagsSubscription = this.featureFlags$.subscribe(
        (featureFlags) => {
          this.loadComponent(
            featureFlags.ShopFilterVersion as ProductBrandFilterVersion,
            changes['brands']?.currentValue ?? this.brands,
            changes['params']?.currentValue ?? this.params
          );
        }
      );
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
    if (!version) {
      debugger;
      return;
    }
    const componentRef =
      this.renderService.loadComponent<IProductBrandFilterComponent>(
        this.dynamicComponentLoader.viewContainerRef,
        version,
        {
          brands,
          params,
        } as IProductBrandFilterComponent
      );
    this.showingVersion = version;
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
  }

  selectBrand(brandIds: number[]) {
    this.brandSelected.emit(brandIds);
  }
}
