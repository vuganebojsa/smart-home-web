import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VehicleChargerReportsComponent } from './vehicle-charger-reports.component';

describe('VehicleChargerReportsComponent', () => {
  let component: VehicleChargerReportsComponent;
  let fixture: ComponentFixture<VehicleChargerReportsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [VehicleChargerReportsComponent]
    });
    fixture = TestBed.createComponent(VehicleChargerReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
