import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditStockitemComponent } from './edit-stockitem.component';

describe('EditStockitemComponent', () => {
  let component: EditStockitemComponent;
  let fixture: ComponentFixture<EditStockitemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditStockitemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditStockitemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
