import { TestBed } from '@angular/core/testing';

import { ClarityService } from './clarity.service';

describe('ClarityService', () => {
  let service: ClarityService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClarityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
