import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root',
})
export class ClarityService extends BaseService {
  private clarityId = environment.analytics.clarityId;
  private scriptContent = `
      (function (c, l, a, r, i, t, y) {
        c[a] =
          c[a] ||
          function () {
            (c[a].q = c[a].q || []).push(arguments);
          };
        t = l.createElement(r);
        t.async = 1;
        t.src = 'https://www.clarity.ms/tag/' + i;
        y = l.getElementsByTagName(r)[0];
        y.parentNode.insertBefore(t, y);
      })(
        window,
        document,
        'clarity',
        'script',
        '${this.clarityId}'
      );
  `;
  init() {
    this.addScriptToHead(undefined, this.scriptContent);
  }
}
