import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CityReportComponent } from './city-report.component';

describe('CityReportComponent', () => {
  let component: CityReportComponent;
  let fixture: ComponentFixture<CityReportComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CityReportComponent]
    });
    fixture = TestBed.createComponent(CityReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
