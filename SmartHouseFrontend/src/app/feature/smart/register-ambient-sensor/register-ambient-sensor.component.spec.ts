import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterAmbientSensorComponent } from './register-ambient-sensor.component';

describe('RegisterAmbientSensorComponent', () => {
  let component: RegisterAmbientSensorComponent;
  let fixture: ComponentFixture<RegisterAmbientSensorComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterAmbientSensorComponent]
    });
    fixture = TestBed.createComponent(RegisterAmbientSensorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
