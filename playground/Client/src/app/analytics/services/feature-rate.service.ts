import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { GoogleAnalyticsService } from './google-analytics.service';

@Injectable({
  providedIn: 'root',
})
export class FeatureRateService extends BaseService {
  constructor(private gaService: GoogleAnalyticsService) {
    super();
  }
  rateFeature(rating: number, feature: string, version: string) {
    this.gaService.triggerEvent(
      'rate_feature',
      feature,
      `${rating}_${version}`
    );
  }
}
