import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddStockitemComponent } from './add-stockitem.component';

describe('AddStockitemComponent', () => {
  let component: AddStockitemComponent;
  let fixture: ComponentFixture<AddStockitemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddStockitemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddStockitemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
