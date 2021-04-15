import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SortV2Component } from './sort-v2.component';

describe('SortV2Component', () => {
  let component: SortV2Component;
  let fixture: ComponentFixture<SortV2Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SortV2Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SortV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
