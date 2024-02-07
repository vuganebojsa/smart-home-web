import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LampDeviceComponent } from './lamp-device.component';

describe('LampDeviceComponent', () => {
  let component: LampDeviceComponent;
  let fixture: ComponentFixture<LampDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LampDeviceComponent]
    });
    fixture = TestBed.createComponent(LampDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
