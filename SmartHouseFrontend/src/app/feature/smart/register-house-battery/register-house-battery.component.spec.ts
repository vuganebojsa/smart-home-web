import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterHouseBatteryComponent } from './register-house-battery.component';

describe('RegisterHouseBatteryComponent', () => {
  let component: RegisterHouseBatteryComponent;
  let fixture: ComponentFixture<RegisterHouseBatteryComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterHouseBatteryComponent]
    });
    fixture = TestBed.createComponent(RegisterHouseBatteryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
