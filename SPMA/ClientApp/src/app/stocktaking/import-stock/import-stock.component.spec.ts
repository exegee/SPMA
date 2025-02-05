import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportStockComponent } from './import-stock.component';

describe('ImportStockComponent', () => {
  let component: ImportStockComponent;
  let fixture: ComponentFixture<ImportStockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportStockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportStockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
