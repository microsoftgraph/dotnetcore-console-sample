import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpellerComponent } from './speller.component';

describe('SpellerComponent', () => {
  let component: SpellerComponent;
  let fixture: ComponentFixture<SpellerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpellerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SpellerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
