import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeFirstPasswordComponent } from './change-first-password.component';

describe('ChangeFirstPasswordComponent', () => {
  let component: ChangeFirstPasswordComponent;
  let fixture: ComponentFixture<ChangeFirstPasswordComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ChangeFirstPasswordComponent]
    });
    fixture = TestBed.createComponent(ChangeFirstPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
