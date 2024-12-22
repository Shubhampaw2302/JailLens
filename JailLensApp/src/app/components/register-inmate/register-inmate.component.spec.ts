import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterInmateComponent } from './register-inmate.component';

describe('RegisterInmateComponent', () => {
  let component: RegisterInmateComponent;
  let fixture: ComponentFixture<RegisterInmateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RegisterInmateComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterInmateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
