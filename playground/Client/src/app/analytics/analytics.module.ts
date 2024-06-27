import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClarityService } from './services/clarity.service';
import { GoogleAnalyticsService } from './services/google-analytics.service';
import { StarRatingModule } from 'angular-star-rating';
import { FeatureRateComponent } from './feature-rate/feature-rate.component';
import { FeatureRateService } from './services/feature-rate.service';

@NgModule({
  declarations: [FeatureRateComponent],
  imports: [CommonModule, StarRatingModule.forChild()],
  providers: [ClarityService, GoogleAnalyticsService, FeatureRateService],
  exports: [FeatureRateComponent],
})
export class AnalyticsModule {}
