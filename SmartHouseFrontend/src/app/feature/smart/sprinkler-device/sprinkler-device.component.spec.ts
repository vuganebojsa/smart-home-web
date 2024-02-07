import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SprinklerDeviceComponent } from './sprinkler-device.component';

describe('SprinklerDeviceComponent', () => {
  let component: SprinklerDeviceComponent;
  let fixture: ComponentFixture<SprinklerDeviceComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SprinklerDeviceComponent]
    });
    fixture = TestBed.createComponent(SprinklerDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
