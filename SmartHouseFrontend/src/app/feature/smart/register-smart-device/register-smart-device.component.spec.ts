import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterSmartDeviceComponent } from './register-smart-device.component';

describe('RegisterSmartDeviceComponent', () => {
  let component: RegisterSmartDeviceComponent;
  let fixture: ComponentFixture<RegisterSmartDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterSmartDeviceComponent]
    });
    fixture = TestBed.createComponent(RegisterSmartDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
