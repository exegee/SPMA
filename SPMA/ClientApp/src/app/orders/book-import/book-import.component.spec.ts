import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BookImportComponent } from './book-import.component';

describe('BookImportComponent', () => {
  let component: BookImportComponent;
  let fixture: ComponentFixture<BookImportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BookImportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BookImportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
