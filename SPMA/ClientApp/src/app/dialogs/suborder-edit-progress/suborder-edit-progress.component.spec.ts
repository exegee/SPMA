import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SuborderEditProgressComponent } from './suborder-edit-progress.component';

describe('SuborderEditProgressComponent', () => {
  let component: SuborderEditProgressComponent;
  let fixture: ComponentFixture<SuborderEditProgressComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SuborderEditProgressComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SuborderEditProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
