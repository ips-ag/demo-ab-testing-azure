import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[appDynamicComponentLoader]',
})
export class DynamicComponentLoaderDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
