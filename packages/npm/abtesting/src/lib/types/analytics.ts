export interface AnalyticsForRootOptions {
  analytics: {
    google?: {
      trackingId: string;
    };
    clarity?: {
      trackingId: string;
    };
  };
}
