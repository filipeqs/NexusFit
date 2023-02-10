import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, of } from 'rxjs';
import { environment } from 'src/environments/environment.development';
import { UserLogin } from '../shared/models/account/user-login.model';
import { User } from '../shared/models/account/user.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.baseUrl;
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

    return this.http.get(this.baseUrl + 'auth', { headers }).pipe(
      map((user: User) => {
        if (user) {
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
      })
    );
  }

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
