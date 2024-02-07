import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GateDeviceComponent } from './gate-device.component';

describe('GateDeviceComponent', () => {
  let component: GateDeviceComponent;
  let fixture: ComponentFixture<GateDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GateDeviceComponent]
    });
    fixture = TestBed.createComponent(GateDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
