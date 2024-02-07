import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterSprinklerSystemComponent } from './register-sprinkler-system.component';

describe('RegisterSprinklerSystemComponent', () => {
  let component: RegisterSprinklerSystemComponent;
  let fixture: ComponentFixture<RegisterSprinklerSystemComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RegisterSprinklerSystemComponent]
    });
    fixture = TestBed.createComponent(RegisterSprinklerSystemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
