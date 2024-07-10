import { ComponentRef, Injectable, Type, ViewContainerRef } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class RenderService {
  componentMaps: Record<string, unknown> = {};
  constructor() {}
  registerComponent<T>(version: string, component: T) {
    this.componentMaps[version] = component;
  }
  loadComponent<T>(viewContainerRef: ViewContainerRef, version: string, args: Record<keyof T, T[keyof T]>): ComponentRef<T> {
    const componentClass: T = this.componentMaps[version] as T;
    viewContainerRef.clear();

    // Create the component and get the instance
    const componentRef = viewContainerRef.createComponent(componentClass as Type<T>);

    // Set input properties on the component instance
    Object.keys(args).forEach((key) => {
      (componentRef.instance as Record<string, unknown>)[key] = args[key as keyof T];
    });
    return componentRef;
  }
}
