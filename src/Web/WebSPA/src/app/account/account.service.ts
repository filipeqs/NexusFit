import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { UserLogin } from '../shared/models/account/user-login.model';
import { User } from '../shared/models/account/user.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.baseUrl;
  private currentUserSource = new BehaviorSubject<User>(null);

  constructor(private http: HttpClient) {}

  login(userLogin: UserLogin) {
    return this.http.post(this.baseUrl + 'auth/login', userLogin).pipe(
      map((user: User) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUserSource.next(null);
  }
}
