import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FeatureRateComponent } from "./feature-rate.component";
import { ComponentFixture, TestBed } from "@angular/core/testing";

describe("FeatureRateComponent", () => {

  let fixture: ComponentFixture<FeatureRateComponent>;
  let component: FeatureRateComponent;
  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
      ],
      declarations: [FeatureRateComponent]
    });

    fixture = TestBed.createComponent(FeatureRateComponent);
    component = fixture.componentInstance;

  });

  it("should be able to create component instance", () => {
    expect(component).toBeDefined();
  });
  
});
