import { Inject, Injectable, Optional } from "@angular/core";
import { BaseService } from "./base.service";
import { ABTESTING_GTAG_ID } from "../../injection-tokens";

type GtagEventParams = {
  event_category?: string;
  event_label?: string;
  value?: number;
  non_interaction?: boolean;
  user_id?: string;
  user_group?: string;
};

type SendEvent = (command: "event", eventName: string, params?: GtagEventParams) => void;
type SetProperties = (
  command: "set",
  commandData: {
    user_group?: "BaseGroup" | "ControlGroup";
    user_properties?: Record<string, string>;
  },
  params?: GtagEventParams
) => void;

interface CustomWindow extends Window {
  gtag: SendEvent | SetProperties;
}

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
  private get gtagSrc() {
    return "https://www.googletagmanager.com/gtag/js?id=" + this.gtagId;
  }
  // @eslint-disable-next-line @typescript-eslint/no-explicit-any
  private gtag = (window as unknown as CustomWindow).gtag;
  private engagementTimeout = 30000; // 30 seconds
  constructor(@Optional() @Inject(ABTESTING_GTAG_ID) gtagId: string) {
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
      if (!(window as unknown as CustomWindow).gtag) {
        this.init();
      }

      this.gtag = (window as unknown as CustomWindow).gtag;
    }
  }

  triggerEvent(eventName: string, params: Record<string, unknown>) {
    (this.gtag as SendEvent)("event", eventName, params);
  }
  trackBounceRate(featureName: string) {
    // Wait for 30 seconds before marking the visitor as engaged.
    setTimeout(() => {
      // Check if gtag is available.
      this.ensureGAExists();
      // Send an event to Google Analytics to indicate the user is engaged with a specific feature.
      // This can help in adjusting the bounce rate as it indicates the user has interacted with the page.
      (this.gtag as SendEvent)("event", "engagement", {
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
    (this.gtag as SendEvent)("event", "login", {
      event_category: "User Authentication",
      event_label: "Login",
      non_interaction: true, // Set to true to not affect bounce rate
      user_id: hashedUserId, // Use the hashed version of the user ID
    });
  }
  async trackUserGroup(group: "BaseGroup" | "ControlGroup") {
    // Check if gtag is available
    this.ensureGAExists();
    // Correctly set the user_group custom dimension for the user.
    // This assumes 'user_group' is the parameter name for the custom dimension in Google Analytics.
    (this.gtag as SetProperties)("set", { user_group: group });
    // Now, send an event to indicate the user is using a specific version of the app.
    // The user_group custom dimension will be included with all subsequent events for this user.
    (this.gtag as SendEvent)("event", "user_group", {
      event_category: "User",
      event_label: group,
      non_interaction: true, // Set to true to not affect bounce rate
      user_group: group, // Use the user_group value
    });

    (this.gtag as SetProperties)("set", {
      user_properties: {
        user_group: group, // Custom user property 'user_group'
      },
    });
  }
}
