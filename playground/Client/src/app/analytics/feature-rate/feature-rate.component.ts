import { Component, Input, OnInit } from '@angular/core';
import { FeatureRateService } from '../services/feature-rate.service';
import { STABLE_SOFTWARE_VERSION } from 'src/app/account/constants';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-feature-rate',
  templateUrl: './feature-rate.component.html',
  styleUrls: ['./feature-rate.component.scss'],
})
export class FeatureRateComponent {
  @Input() rating: number = 5;
  @Input() feature: string = 'feature';
  @Input() version: string = STABLE_SOFTWARE_VERSION;
  private destroy$ = new Subject<void>();

  constructor(private featureRateService: FeatureRateService) {}
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onRatingChange(rating: number) {
    this.featureRateService.rateFeature(rating, this.feature, this.version);
  }
}
