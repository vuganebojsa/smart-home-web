import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OnlineReportComponent } from './online-report.component';

describe('OnlineReportComponent', () => {
  let component: OnlineReportComponent;
  let fixture: ComponentFixture<OnlineReportComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [OnlineReportComponent]
    });
    fixture = TestBed.createComponent(OnlineReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
