import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayTemplateComponent } from './display-template.component';

describe('DisplayTemplateComponent', () => {
  let component: DisplayTemplateComponent;
  let fixture: ComponentFixture<DisplayTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
