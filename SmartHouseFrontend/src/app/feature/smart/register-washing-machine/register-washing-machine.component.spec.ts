import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterWashingMachineComponent } from './register-washing-machine.component';

describe('RegisterWashingMachineComponent', () => {
  let component: RegisterWashingMachineComponent;
  let fixture: ComponentFixture<RegisterWashingMachineComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterWashingMachineComponent]
    });
    fixture = TestBed.createComponent(RegisterWashingMachineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
