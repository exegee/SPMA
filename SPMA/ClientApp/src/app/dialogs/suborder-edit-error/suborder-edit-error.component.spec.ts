import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SuborderEditErrorComponent } from './suborder-edit-error.component';

describe('SuborderEditErrorComponent', () => {
  let component: SuborderEditErrorComponent;
  let fixture: ComponentFixture<SuborderEditErrorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SuborderEditErrorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SuborderEditErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
