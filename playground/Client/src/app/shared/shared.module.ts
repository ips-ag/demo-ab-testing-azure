import { NgModule } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { PaginationHeaderComponent } from './components/pagination-header/pagination-header.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { OrderTotalsComponent } from './order-totals/order-totals.component';
import { SettingsModule } from '../settings/settings.module';
import { NgxSliderModule } from '@angular-slider/ngx-slider';
import { DynamicComponentLoaderDirective } from './directives/dynamic-component-loader.directive';

@NgModule({
  declarations: [
    PaginationHeaderComponent,
    PaginationComponent,
    OrderTotalsComponent,
    DynamicComponentLoaderDirective,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CarouselModule.forRoot(),
    PaginationModule.forRoot(),
    MatInputModule,
    SettingsModule,
  ],
  exports: [
    PaginationHeaderComponent,
    PaginationComponent,
    OrderTotalsComponent,
    PaginationModule,
    CarouselModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    SettingsModule,
    NgxSliderModule,
    DynamicComponentLoaderDirective,
  ],
})
export class SharedModule {}
