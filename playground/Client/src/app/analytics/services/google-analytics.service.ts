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
  init() {
    this.addScriptToHead(this.gtagSrc);
    this.addScriptToHead(undefined, this.gtagContent);
  }
  triggerEvent(eventName: string, eventCategory: string, eventLabel: string) {
    this.gtag('event', eventName, {
      event_category: eventCategory,
      event_label: eventLabel,
    });
  }
}
