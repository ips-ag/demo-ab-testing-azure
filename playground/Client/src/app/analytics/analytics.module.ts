import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClarityService } from './services/clarity.service';
import { GoogleAnalyticsService } from './services/google-analytics.service';

@NgModule({
  declarations: [],
  imports: [CommonModule],
  providers: [ClarityService, GoogleAnalyticsService],
})
export class AnalyticsModule {}
