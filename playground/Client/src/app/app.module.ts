import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { HomeModule } from './home/home.module';
import { ErrorInterceptor } from './core/interceptor/error.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './core/interceptor/loading.interceptor';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { StarRatingModule } from 'angular-star-rating';
import { environment } from 'src/environments/environment';
import { ABAnalyticsModule } from '@ips-ag/abtesting';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    CoreModule,
    HomeModule,
    NgxSpinnerModule.forRoot({ type: 'square-jelly-box' }),
    ABAnalyticsModule.forRoot({
      analytics: {
        google: {
          trackingId: environment.analytics.googleAnalyticsMeasurementId,
        },
        clarity: {
          trackingId: environment.analytics.clarityId,
        },
      },
    }),
    StarRatingModule.forRoot(),
  ],

  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true,
    },
    {
      provide: LocationStrategy,
      useClass: HashLocationStrategy,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
