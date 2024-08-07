import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { User, UserRegister } from '../shared/models/user';
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { GoogleAnalyticsService } from '@ips-ag/abtesting';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  redirectUrl: string | null = null;
  private apiUrl = `${environment.baseUrl}/api/Account`;
  private userSource = new BehaviorSubject<User | null>(null);
  userSource$ = this.userSource.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    private gaService: GoogleAnalyticsService
  ) {}

  isAuthenticated(): boolean {
    // For example, check if there's a valid JWT token in local storage
    const token = localStorage.getItem('token');
    // Add your logic to validate the token if necessary
    return !!token;
  }

  loadUser(token: string) {
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`);
    return this.http.get<User>(`${this.apiUrl}`, { headers }).pipe(
      map((user) => {
        if (user?.token) {
          localStorage.setItem('token', user.token);
          this.userSource.next(user);
        }
        return user;
      }),
      catchError((error) => {
        console.error('Error loading user:', error);

        // Rethrow the error to propagate it to the caller
        return throwError(() => error);
      })
    );
  }

  // Method for user login
  login(loginModel: any): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, loginModel).pipe(
      tap(async (response) => {
        localStorage.setItem('token', response.token);
        this.userSource.next(response);
        await this.gaService.trackLogin(response.email);
      }),
      // Store token in localStorage and notify via userSource
      map((response) => {
        const user: User = {
          displayName: response.displayName,
          email: response.email,
          token: response.token,
        };
        return user;
      })
    );
  }

  // Method for user registration
  register(registerModel: UserRegister): Observable<User> {
    const email = registerModel.email;

    // Check if the email is available
    return this.checkEmail(email).pipe(
      switchMap((isAvailable) => {
        if (isAvailable) {
          // Email is available, proceed with registration
          return this.http
            .post<User>(`${this.apiUrl}/register`, registerModel)
            .pipe(
              map((user) => {
                if (user?.token) {
                  localStorage.setItem('token', user.token);
                  this.userSource.next(user);
                }
                return user;
              })
            );
        } else {
          // Email is not available, return an error
          return throwError(() => new Error('Email is already in use'));
        }
      })
    );
  }

  // Method to check if an email is available
  checkEmail(email: string): Observable<boolean> {
    return this.http.get<boolean>(
      `${this.apiUrl}/check-email-exists?email=${email}`
    );
  }

  // Method to logout the user
  logout() {
    localStorage.removeItem('token');
    this.userSource.next(null);
    this.router.navigate(['/']);
  }
}
