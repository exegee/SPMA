import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SuborderEditComponent } from './suborder-edit.component';

describe('SuborderEditComponent', () => {
  let component: SuborderEditComponent;
  let fixture: ComponentFixture<SuborderEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SuborderEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SuborderEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
