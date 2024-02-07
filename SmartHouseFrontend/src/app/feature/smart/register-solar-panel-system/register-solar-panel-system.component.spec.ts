import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterSolarPanelSystemComponent } from './register-solar-panel-system.component';

describe('RegisterSolarPanelSystemComponent', () => {
  let component: RegisterSolarPanelSystemComponent;
  let fixture: ComponentFixture<RegisterSolarPanelSystemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterSolarPanelSystemComponent]
    });
    fixture = TestBed.createComponent(RegisterSolarPanelSystemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
