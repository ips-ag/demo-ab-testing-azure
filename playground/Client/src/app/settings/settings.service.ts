import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { FeatureFlags } from '../shared/models/featureFlag';

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
    return this.http.get<FeatureFlags>(this.apiUrl, {
      headers,
    });
  }
}
