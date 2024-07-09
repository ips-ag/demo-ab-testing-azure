import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import {
  FeatureFlagKey,
  FeatureFlagModel,
  FeatureFlags,
} from '../shared/models/featureFlag';
import { map } from 'rxjs';
import { isBoolean } from 'lodash';

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  private apiUrl = `${environment.baseUrl}/api/ab-testing/feature-flags/`;
  constructor(private http: HttpClient) {}
  getFeatureFlags() {
    let headers = new HttpHeaders();
    headers = headers.set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
    return this.http
      .get<FeatureFlags>(this.apiUrl, {
        headers,
      })
      .pipe(
        map((res: any) => {
          const featureFlags: FeatureFlags = {} as FeatureFlags;
          const resData = res as FeatureFlagModel[];
          for (const featureFlag of resData) {
            featureFlags[featureFlag.name as FeatureFlagKey] =
              featureFlag.isVariant
                ? String(featureFlag.value)
                : Boolean(featureFlag.value);
          }

          return featureFlags;
        })
      );
  }
}
