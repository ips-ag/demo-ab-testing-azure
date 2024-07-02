import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class GoogleAnalyticsService extends BaseService {
  private gtagId = environment.analytics.googleAnalyticsMeasurementId;
  private gtagContent = ` window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());

  gtag('config', '${this.gtagId}');`;
  private gtagSrc =
    'https://www.googletagmanager.com/gtag/js?id=' + this.gtagId;
  private gtag = (window as typeof window & { gtag: any }).gtag;
  private engagementTimeout = 30000; // 30 seconds
  private formatEventLabel(label: string): string {
    return label
      .toLowerCase() // Convert to lowercase
      .replace(/\s+/g, '_') // Replace spaces with underscores
      .replace(/[^\w.-]+/g, ''); // Allow underscores, hyphens, and periods
  }
  init() {
    this.addScriptToHead(this.gtagSrc);
    this.addScriptToHead(undefined, this.gtagContent);
  }
  triggerEvent(eventName: string, params: Record<string, any>) {
    this.gtag('event', eventName, params);
  }
  trackBounceRate(featureName: string) {
    // Wait for 30 seconds before marking the visitor as engaged.
    setTimeout(() => {
      // Check if gtag is available.
      if (this.gtag) {
        // Send an event to Google Analytics to indicate the user is engaged with a specific feature.
        // This can help in adjusting the bounce rate as it indicates the user has interacted with the page.
        this.gtag('event', 'engagement', {
          event_category: 'User Engagement',
          event_label: `Engaged User - ${featureName}`,
          non_interaction: true, // Set to true to not affect bounce rate.
        });
      }
    }, this.engagementTimeout); // Use the engagementTimeout property
  }
}
