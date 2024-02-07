import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmbientSensorDeviceComponent } from './ambient-sensor-device.component';

describe('AmbientSensorDeviceComponent', () => {
  let component: AmbientSensorDeviceComponent;
  let fixture: ComponentFixture<AmbientSensorDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AmbientSensorDeviceComponent]
    });
    fixture = TestBed.createComponent(AmbientSensorDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
