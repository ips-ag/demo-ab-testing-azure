import { Component, Injector, Input, OnDestroy, OnInit } from '@angular/core';
import { ReplaySubject, Subject, takeUntil } from 'rxjs';
import { ClickEvent, StarRatingModule } from 'angular-star-rating';
import { CommonModule } from '@angular/common';
import { FeatureRateService } from '@ips-ag/abtesting';

@Component({
  // eslint-disable-next-line @angular-eslint/component-selector
  selector: 'ab-feature-rate',
  standalone: true,
  templateUrl: './feature-rate.component.html',
  imports: [StarRatingModule, CommonModule],
  styleUrls: ['./feature-rate.component.scss'],
})
export class FeatureRateComponent implements OnInit, OnDestroy {
  @Input() rating: number = 0;
  @Input() feature: string = 'feature';
  @Input() version?: string;
  isRated = false;
  isHidden = false;
  private ratingChange$ = new ReplaySubject<number>();
  private destroy$ = new Subject<void>();
  private featureRateService?: FeatureRateService;

  constructor(private injector: Injector) {
    this.featureRateService = this.injector.get(FeatureRateService);

    this.ratingChange$.pipe(takeUntil(this.destroy$)).subscribe((rating) => {
      if (this.version) {
        this.featureRateService?.rateFeature(
          rating,
          this.feature,
          this.version
        );
        this.isRated = true;
      }
    });
  }
  ngOnInit() {
    if (this.version) {
      this.isHidden =
        this.featureRateService?.isRated(this.feature, this.version) ?? false;
    }
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onRatingChange(e: ClickEvent) {
    this.rating = e.rating;
    this.ratingChange$.next(e.rating);
  }
  revise(e: Event) {
    e.preventDefault();
    this.isRated = false;
  }
}
