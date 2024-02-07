import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartPropertyRequestsListComponent } from './smart-property-requests-list.component';

describe('SmartPropertyRequestsListComponent', () => {
  let component: SmartPropertyRequestsListComponent;
  let fixture: ComponentFixture<SmartPropertyRequestsListComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SmartPropertyRequestsListComponent]
    });
    fixture = TestBed.createComponent(SmartPropertyRequestsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
