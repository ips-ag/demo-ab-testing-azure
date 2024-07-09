export type FeatureFlagKey = 'ShopFilterVersion';
export type FeatureFlags = Record<FeatureFlagKey, string | boolean>;
export class FeatureFlagModel {
  name!: string;
  value?: string;
  isVariant!: boolean;
  constructor(init?: Partial<FeatureFlagModel>) {
    Object.assign(this, init);
  }
}
