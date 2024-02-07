import { TestBed } from '@angular/core/testing';

import { SmartPropertyService } from './smart-property.service';

describe('SmartPropertyService', () => {
  let service: SmartPropertyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SmartPropertyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
