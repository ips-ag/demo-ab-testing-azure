import { Component, OnInit } from '@angular/core';
import { BasketService } from './basket/basket.service';
import { AccountService } from './account/account.service';
import { ClarityService, GoogleAnalyticsService } from '@ips-ag/abtesting';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'Sports Center';

  constructor(
    private basketService: BasketService,
    private accountService: AccountService,
    private clarityService: ClarityService,
    private googleAnalyticsService: GoogleAnalyticsService
  ) {
    this.googleAnalyticsService.init();
    this.clarityService.init();
  }

  ngOnInit() {
    this.loadBasket();
    this.loadUser();
  }

  loadBasket() {
    const basketId = localStorage.getItem('basket_id');
    if (basketId) {
      this.basketService.getBasket(basketId);
    }
  }

  loadUser() {
    const token = localStorage.getItem('token');
    if (token) {
      this.accountService.loadUser(token).subscribe();
    }
  }
}
