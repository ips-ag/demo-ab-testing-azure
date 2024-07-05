import { Inject, Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { ABTESTING_GTAG_ID } from "../../injection-tokens";

@Injectable({
  providedIn: "root",
})
export class GoogleAnalyticsService extends BaseService {
  private gtagId: string = "";
  private get gtagContent() {
    return `
    window.dataLayer = window.dataLayer || [];
    function gtag(){dataLayer.push(arguments);}
    gtag('js', new Date());

    gtag('config', '${this.gtagId}');`;
  }
  private gtagSrc = "https://www.googletagmanager.com/gtag/js?id=" + this.gtagId;
  private gtag = (window as typeof window & { gtag: any }).gtag;
  private engagementTimeout = 30000; // 30 seconds
  constructor(@Inject(ABTESTING_GTAG_ID) gtagId: string) {
    super();
    this.gtagId = gtagId;
    this.trackBounceRate = this.trackBounceRate.bind(this);
    this.trackLogin = this.trackLogin.bind(this);
    this.triggerEvent = this.triggerEvent.bind(this);
  }
  init() {
    this.addScriptToHead(this.gtagSrc);
    this.addScriptToHead(undefined, this.gtagContent);
  }
  ensureGAExists() {
    if (!this.gtag) {
      if (!(window as typeof window & { gtag: any }).gtag) {
        this.init();
      }
      this.gtag = (window as typeof window & { gtag: any }).gtag;
    }
  }
  triggerEvent(eventName: string, params: Record<string, any>) {
    this.gtag("event", eventName, params);
  }
  trackBounceRate(featureName: string) {
    // Wait for 30 seconds before marking the visitor as engaged.
    setTimeout(() => {
      // Check if gtag is available.
      this.ensureGAExists();
      // Send an event to Google Analytics to indicate the user is engaged with a specific feature.
      // This can help in adjusting the bounce rate as it indicates the user has interacted with the page.
      this.gtag("event", "engagement", {
        event_category: "User Engagement",
        event_label: `Engaged User - ${featureName}`,
        non_interaction: true, // Set to true to not affect bounce rate.
      });
    }, this.engagementTimeout); // Use the engagementTimeout property
  }
  async trackLogin(userId: string) {
    // Check if gtag is available

    this.ensureGAExists();

    // Generate a hashed version of the real user ID
    const hashedUserId = await this.calculateSHA256(userId);

    // Send a login event to Google Analytics with the hashed user ID
    this.gtag("event", "login", {
      event_category: "User Authentication",
      event_label: "Login",
      non_interaction: true, // Set to true to not affect bounce rate
      user_id: hashedUserId, // Use the hashed version of the user ID
    });
  }
}
