<h1 align="center">Welcome to @ips-ag/abtesting 👋</h1>
<p>
  <a href="https://www.npmjs.com/package/@ips-ag/abtesting" target="_blank">
    <img alt="Version" src="https://img.shields.io/npm/v/@ips-ag/abtesting.svg">
  </a>
  <a href="https://github.com/ips-ag/demo-ab-testing-azure#main#readme" target="_blank">
    <img alt="Documentation" src="https://img.shields.io/badge/documentation-yes-brightgreen.svg" />
  </a>
  <a href="https://github.com/ips-ag/demo-ab-testing-azure/activity" target="_blank">
    <img alt="Maintenance" src="https://img.shields.io/badge/Maintained%3F-yes-green.svg" />
  </a>
  <a href="https://github.com/ips-ag/demo-ab-testing-azure?tab=MIT-1-ov-file" target="_blank">
    <img alt="License: MIT" src="https://img.shields.io/badge/license-MIT-yellow.svg" />
  </a>
</p>

> The A/B Testing NPM is a library designed to facilitate A/B testing experiments in a controlled environment

### 🏠 [Homepage](https://github.com/ips-ag/demo-ab-testing-azure/tree/main/packages/npm/abtesting#readme)

### ✨ [Demo](https://abtesting-playground.nicebeach-03e1bf22.westeurope.azurecontainerapps.io)

## Dependencies

Latest version available for each version of Angular

| @ips-ag/abtesting | Angular |
|---------------------|---------|
| 0.x.x               | 17.x    |

## Installation

To install this library, run:

```bash
$ npm install @ips-ag/abtesting --save
```
## Usage

In your `app.module.ts`

```typescript
import { Component } from '@angular/core';
...
import { ABAnalyticsModule } from '@ips-ag/abtesting';
...
@NgModule({
  declarations: [...],
  imports: [
    ...
    ABAnalyticsModule.forRoot({
      analytics: {
        google: {
          trackingId: // add your googleAnalyticsMeasurementId,
        },
        clarity: {
          trackingId: // add your MS clarityId,
        },
      },
    }),
    ...
  ],

  providers: [
    ...
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
```

In your `app.component.ts` where you want to initialize the A/B Testing service

```typescript
import { Component, OnInit } from '@angular/core';
...
import { ClarityService, GoogleAnalyticsService } from '@ips-ag/abtesting';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  ...
  constructor(
    ...
    private clarityService: ClarityService,
    private googleAnalyticsService: GoogleAnalyticsService
  ) {
    this.googleAnalyticsService.init();
    this.clarityService.init();
  }
  ...
}
```

In your components/services where you want to collect user information
```typescript
...
import { GoogleAnalyticsService, RenderService } from '@ips-ag/abtesting';
...
export class MyComponentOrService{
    constructor(
        ...
        private featureRateService: FeatureRateService,
        private renderService: RenderService,
        private gaService: GoogleAnalyticsService // inject GoogleAnalyticsService
    ) {
        // #region GoogleAnalyticsService
        // Track user by userId
        this.gaService.trackLogin('userId');
        // Track user by user group
        this.gaService.trackUserGroup('userGroup');
        // Track bounce rate
        this.gaService.trackBounceRate();
        // #endregion
        // Register version of component to render
        this.renderService.registerComponent(
            version, // experimental version
            component // Component instance
            );
        // Rating your experimental for a feature     
         this.featureRateService?.rateFeature(
            rating, // rating score
            this.feature, // feature name
            this.version // feature version
        );
        // Check if a feature has already rated
        this.featureRateService?.isRated(
            this.feature, // feature name
            this.version // feature version
            );
    }
}

```

## Author

👤 **IPS-AG**

* Website: https://www.ips-ag.com
* Github: [@ips-ag](https://github.com/ips-ag)

## 🤝 Contributing

Contributions, issues and feature requests are welcome!<br />Feel free to check [issues page](https://github.com/ips-ag/demo-ab-testing-azure/issues). You can also take a look at the [contributing guide](https://github.com/ips-ag/demo-ab-testing-azure/tree/main/packages/npm/CONTRIBUTING.md).

## Show your support

Give a ⭐️ if this project helped you!

## 📝 License

Copyright © 2024 [IPS-AG](https://github.com/ips-ag).<br />
This project is [MIT](https://github.com/ips-ag/demo-ab-testing-azure/blob/main/LICENSE) licensed.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.