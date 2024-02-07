import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpsDeviceComponent } from './sps-device.component';

describe('SpsDeviceComponent', () => {
  let component: SpsDeviceComponent;
  let fixture: ComponentFixture<SpsDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SpsDeviceComponent]
    });
    fixture = TestBed.createComponent(SpsDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
