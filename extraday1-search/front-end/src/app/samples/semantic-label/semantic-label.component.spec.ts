import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SemanticLabelComponent } from './semantic-label.component';

describe('SemanticLabelComponent', () => {
  let component: SemanticLabelComponent;
  let fixture: ComponentFixture<SemanticLabelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SemanticLabelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SemanticLabelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
