import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from 'src/app/material.module';
import { ReactiveFormsModule } from '@angular/forms';
import { GenericInputComponent } from './components/forms/generic-input/generic-input.component';
import { PasswordInputComponent } from './components/forms/password-input/password-input.component';

@NgModule({
  declarations: [GenericInputComponent, PasswordInputComponent],
  imports: [CommonModule, MaterialModule, ReactiveFormsModule],
  exports: [
    GenericInputComponent,
    PasswordInputComponent,
    MaterialModule,
    ReactiveFormsModule,
  ],
})
export class SharedModule {}
