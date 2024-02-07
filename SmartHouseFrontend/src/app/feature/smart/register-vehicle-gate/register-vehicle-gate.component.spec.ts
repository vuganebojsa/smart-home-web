import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterVehicleGateComponent } from './register-vehicle-gate.component';

describe('RegisterVehicleGateComponent', () => {
  let component: RegisterVehicleGateComponent;
  let fixture: ComponentFixture<RegisterVehicleGateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterVehicleGateComponent]
    });
    fixture = TestBed.createComponent(RegisterVehicleGateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
