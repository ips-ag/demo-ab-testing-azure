import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
  selector: '[AppDynamicComponentLoader]',
})
export class DynamicComponentLoaderDirective {
  constructor(public viewContainerRef: ViewContainerRef) {}
}
