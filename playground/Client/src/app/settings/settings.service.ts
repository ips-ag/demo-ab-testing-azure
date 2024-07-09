import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { FeatureFlags } from '../shared/models/featureFlag';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  private apiUrl = `${environment.baseUrl}/api/ab-testing/feature-flags/`;
  private isVariantMaps: Record<string, boolean> = {};
  constructor(private http: HttpClient) {}

  getFeatureFlags() {
    let headers = new HttpHeaders();
    headers = headers.set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
    return this.http
      .get<
        {
          name: string;
          value: string;
          isVariant: boolean;
        }[]
      >(this.apiUrl, {
        headers,
      })
      .pipe(
        map((res) =>
          res.reduce((acc, curr) => {
            if (curr.isVariant) {
              this.isVariantMaps[curr.name] = curr.isVariant;
            }
            return { ...acc, [curr.name]: curr.value };
          }, {} as FeatureFlags)
        )
      );
  }
  isVariant(name: string) {
    return this.isVariantMaps[name];
  }
}
