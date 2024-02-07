import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartPropertyDevicesDisplayComponent } from './smart-property-devices-display.component';

describe('SmartPropertyDevicesDisplayComponent', () => {
  let component: SmartPropertyDevicesDisplayComponent;
  let fixture: ComponentFixture<SmartPropertyDevicesDisplayComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SmartPropertyDevicesDisplayComponent]
    });
    fixture = TestBed.createComponent(SmartPropertyDevicesDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
