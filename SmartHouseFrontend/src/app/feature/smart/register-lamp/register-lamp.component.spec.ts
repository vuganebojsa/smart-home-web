import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterLampComponent } from './register-lamp.component';

describe('RegisterLampComponent', () => {
  let component: RegisterLampComponent;
  let fixture: ComponentFixture<RegisterLampComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterLampComponent]
    });
    fixture = TestBed.createComponent(RegisterLampComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
