import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserLogin } from '../shared/models/account/user-login.model';
import { UserRegister } from '../shared/models/account/user-register.model';
import { User } from '../shared/models/account/user.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User>(null);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  loadCurrentUser(token: string) {
    if (!token) {
      this.currentUserSource.next(null);
      return of(null);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`);

    return this.http.get<User>(this.baseUrl + '/api/auth', { headers }).pipe(
      map((user) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

  login(userLogin: UserLogin) {
    console.log(this.baseUrl);
    return this.http
      .post<User>(this.baseUrl + '/api/auth/login', userLogin)
      .pipe(
        map((user) => {
          if (user) {
            localStorage.setItem('token', user.token);
            this.currentUserSource.next(user);
          }
        })
      );
  }

  register(userRegister: UserRegister) {
    return this.http
      .post<User>(this.baseUrl + '/api/auth/register', userRegister)
      .pipe(
        map((user) => {
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

  checkEmailExists(email: string) {
    return this.http.get(this.baseUrl + '/api/auth/userexists/' + email);
  }
}
