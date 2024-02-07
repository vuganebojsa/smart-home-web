import { TestBed } from '@angular/core/testing';
import { SmartDeviceService } from './smart-device.service';


describe('SmartService', () => {
  let service: SmartDeviceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SmartDeviceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
