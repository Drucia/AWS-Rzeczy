import { TestBed } from '@angular/core/testing';

import { AWSService } from './aws.service';

describe('AWSService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AWSService = TestBed.get(AWSService);
    expect(service).toBeTruthy();
  });
});
