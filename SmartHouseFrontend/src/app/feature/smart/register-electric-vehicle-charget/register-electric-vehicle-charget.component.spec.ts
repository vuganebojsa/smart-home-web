import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterElectricVehicleChargetComponent } from './register-electric-vehicle-charget.component';

describe('RegisterElectricVehicleChargetComponent', () => {
  let component: RegisterElectricVehicleChargetComponent;
  let fixture: ComponentFixture<RegisterElectricVehicleChargetComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterElectricVehicleChargetComponent]
    });
    fixture = TestBed.createComponent(RegisterElectricVehicleChargetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
