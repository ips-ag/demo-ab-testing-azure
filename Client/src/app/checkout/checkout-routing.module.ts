import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CheckoutComponent } from './checkout.component';
import { canActivate } from '../core/guards/auth.guard';


const routes: Routes = [
  {path:'', component: CheckoutComponent, 
  canActivate: [canActivate]}
]

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class CheckoutRoutingModule { }