import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BatteryDeviceComponent } from './battery-device.component';

describe('BatteryDeviceComponent', () => {
  let component: BatteryDeviceComponent;
  let fixture: ComponentFixture<BatteryDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BatteryDeviceComponent]
    });
    fixture = TestBed.createComponent(BatteryDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
