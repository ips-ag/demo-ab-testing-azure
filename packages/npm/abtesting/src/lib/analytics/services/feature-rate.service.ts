import { Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { GoogleAnalyticsService } from "./google-analytics.service";

@Injectable({
  providedIn: "root",
})
export class FeatureRateService extends BaseService {
  constructor(private gaService: GoogleAnalyticsService) {
    super();
  }
  rateFeature(rating: number, feature: string, version: string) {
    this.gaService.triggerEvent("rate_feature", {
      feature: feature,
      rating: rating,
      version: version,
    });
    localStorage.setItem(`rate_${feature}_${version}`, `${rating}`);
  }
  isRated(feature: string, version: string) {
    return localStorage.getItem(`rate_${feature}_${version}`) !== null;
  }
}
