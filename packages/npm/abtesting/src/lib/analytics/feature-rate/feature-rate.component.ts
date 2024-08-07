import { Component, Input, OnDestroy, OnInit } from "@angular/core";
import { FeatureRateService } from "../services/feature-rate.service";
import { ReplaySubject, Subject, takeUntil } from "rxjs";
import { ClickEvent } from "angular-star-rating";

@Component({
  selector: "ab-feature-rate",
  templateUrl: "./feature-rate.component.html",
  styleUrls: ["./feature-rate.component.scss"],
})
export class FeatureRateComponent implements OnInit, OnDestroy {
  @Input() rating: number = 0;
  @Input() feature: string = "feature";
  @Input() version?: string;
  isRated = false;
  isHidden = false;
  private ratingChange$ = new ReplaySubject<number>();
  private destroy$ = new Subject<void>();

  constructor(private featureRateService: FeatureRateService) {
    this.ratingChange$.pipe(takeUntil(this.destroy$)).subscribe((rating) => {
      if (this.version) {
        this.featureRateService.rateFeature(rating, this.feature, this.version);
        this.isRated = true;
      }
    });
  }
  ngOnInit() {
    if (this.version) {
      this.isHidden = this.featureRateService.isRated(this.feature, this.version);
    }
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onRatingChange(e: ClickEvent) {
    this.rating = e.rating;
    this.ratingChange$.next(e.rating);
  }
  revise(e: Event) {
    e.preventDefault();
    this.isRated = false;
  }
}
