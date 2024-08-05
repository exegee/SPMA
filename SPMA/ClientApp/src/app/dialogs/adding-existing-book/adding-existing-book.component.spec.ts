import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddingExistingBookComponent } from './adding-existing-book.component';

describe('AddingExistingBookComponent', () => {
  let component: AddingExistingBookComponent;
  let fixture: ComponentFixture<AddingExistingBookComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddingExistingBookComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddingExistingBookComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
