import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { HomeComponent } from './home/home.component';
import { HttpClientModule } from '@angular/common/http';
import { AccountService } from './account/account.service';
import { lastValueFrom } from 'rxjs';
import { CoreModule } from './core/core.module';

@NgModule({
  declarations: [AppComponent, HomeComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CoreModule,
    BrowserAnimationsModule,
    HttpClientModule,
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializer,
      deps: [AccountService],
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}

export function appInitializer(accountService: AccountService) {
  const token = localStorage.getItem('token');
  return async () => {
    try {
      await lastValueFrom(accountService.loadCurrentUser(token));
      console.log('Loaded user');
    } catch (error) {
      console.log(error);
    }
  };
}
