import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminPropertyDisplayComponent } from './admin-property-display.component';

describe('AdminPropertyDisplayComponent', () => {
  let component: AdminPropertyDisplayComponent;
  let fixture: ComponentFixture<AdminPropertyDisplayComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AdminPropertyDisplayComponent]
    });
    fixture = TestBed.createComponent(AdminPropertyDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
