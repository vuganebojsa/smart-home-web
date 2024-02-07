import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartPropertyListComponent } from './smart-property-list.component';

describe('SmartPropertyListComponent', () => {
  let component: SmartPropertyListComponent;
  let fixture: ComponentFixture<SmartPropertyListComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SmartPropertyListComponent]
    });
    fixture = TestBed.createComponent(SmartPropertyListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
