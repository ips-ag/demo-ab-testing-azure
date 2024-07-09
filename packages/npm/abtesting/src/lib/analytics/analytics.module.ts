import { CommonModule } from "@angular/common";
import { ModuleWithProviders, NgModule } from "@angular/core";
import { ABTESTING_CLARITY_ID, ABTESTING_GTAG_ID } from "../injection-tokens";
import type { AnalyticsForRootOptions } from "../types";
import { FeatureRateService, RenderService, ClarityService, GoogleAnalyticsService } from "./services";

export function provideClarityService(clarityId: string) {
  return new ClarityService(clarityId);
}

export function provideGoogleAnalyticsService(gtagId: string) {
  return new GoogleAnalyticsService(gtagId);
}

export function provideFeatureRateService(gaService: GoogleAnalyticsService) {
  return new FeatureRateService(gaService);
}

@NgModule({
  declarations: [],
  imports: [CommonModule],
  exports: [],
})
export class ABAnalyticsModule {
  static initialized = false;

  static forRoot(options: AnalyticsForRootOptions): ModuleWithProviders<ABAnalyticsModule> {
    const {
      analytics: { google, clarity },
    } = options;

    if (ABAnalyticsModule.initialized) {
      throw new Error("ABAnalyticsModule is already initialized");
    }

    this.initialized = true;

    return {
      ngModule: ABAnalyticsModule,
      providers: [
        { provide: ABTESTING_GTAG_ID, useValue: google?.trackingId },
        { provide: ABTESTING_CLARITY_ID, useValue: clarity?.trackingId },
        { provide: ClarityService, useFactory: provideClarityService, deps: [ABTESTING_CLARITY_ID] },
        { provide: GoogleAnalyticsService, useFactory: provideGoogleAnalyticsService, deps: [ABTESTING_GTAG_ID] },
        { provide: FeatureRateService, useFactory: provideFeatureRateService, deps: [GoogleAnalyticsService] },
        RenderService,
      ],
    };
  }

  static forChild(): ModuleWithProviders<ABAnalyticsModule> {
    if (ABAnalyticsModule.initialized) {
      return {
        ngModule: ABAnalyticsModule,
        providers: [], // Do not re-provide services
      };
    } else {
      throw new Error("ABAnalyticsModule.forRoot() must be called in the root module first.");
    }
  }
}
