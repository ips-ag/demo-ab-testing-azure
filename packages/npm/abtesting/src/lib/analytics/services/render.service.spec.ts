import { RenderService } from "./render.service";
import { TestBed } from "@angular/core/testing";

describe("RenderService", () => {

  let service: RenderService;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        RenderService
      ]
    });
    service = TestBed.get(RenderService);

  });

  it("should be able to create service instance", () => {
    expect(service).toBeDefined();
  });

});
