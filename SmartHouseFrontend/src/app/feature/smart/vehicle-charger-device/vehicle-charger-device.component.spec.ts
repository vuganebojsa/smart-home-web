import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleChargerDeviceComponent } from './vehicle-charger-device.component';

describe('VehicleChargerDeviceComponent', () => {
  let component: VehicleChargerDeviceComponent;
  let fixture: ComponentFixture<VehicleChargerDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VehicleChargerDeviceComponent]
    });
    fixture = TestBed.createComponent(VehicleChargerDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
