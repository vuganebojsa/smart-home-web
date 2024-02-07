import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterAirConditionerComponent } from './register-air-conditioner.component';

describe('RegisterAirConditionerComponent', () => {
  let component: RegisterAirConditionerComponent;
  let fixture: ComponentFixture<RegisterAirConditionerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterAirConditionerComponent]
    });
    fixture = TestBed.createComponent(RegisterAirConditionerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
