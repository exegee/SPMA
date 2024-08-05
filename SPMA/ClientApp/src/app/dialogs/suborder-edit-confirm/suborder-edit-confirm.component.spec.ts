import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SuborderEditConfirmComponent } from './suborder-edit-confirm.component';

describe('SuborderEditConfirmComponent', () => {
  let component: SuborderEditConfirmComponent;
  let fixture: ComponentFixture<SuborderEditConfirmComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SuborderEditConfirmComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SuborderEditConfirmComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
