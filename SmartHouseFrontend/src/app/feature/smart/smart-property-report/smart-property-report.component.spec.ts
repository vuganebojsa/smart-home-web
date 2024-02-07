import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartPropertyReportComponent } from './smart-property-report.component';

describe('SmartPropertyReportComponent', () => {
  let component: SmartPropertyReportComponent;
  let fixture: ComponentFixture<SmartPropertyReportComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SmartPropertyReportComponent]
    });
    fixture = TestBed.createComponent(SmartPropertyReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
