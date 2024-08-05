import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BookExistComponent } from './book-exist.component';

describe('BookExistComponent', () => {
  let component: BookExistComponent;
  let fixture: ComponentFixture<BookExistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BookExistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BookExistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
