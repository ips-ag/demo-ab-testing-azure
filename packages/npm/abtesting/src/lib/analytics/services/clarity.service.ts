import { Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { Inject } from "@angular/core";
import { ABTESTING_CLARITY_ID } from "../../injection-tokens";

@Injectable({
  providedIn: "root",
})
export class ClarityService extends BaseService {
  private clarityId = "";
  private get scriptContent() {
    return `
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
  }
  constructor(@Inject(ABTESTING_CLARITY_ID) clarityId: string) {
    super();
    this.clarityId = clarityId;
  }
  init() {
    this.addScriptToHead(undefined, this.scriptContent);
  }
}
