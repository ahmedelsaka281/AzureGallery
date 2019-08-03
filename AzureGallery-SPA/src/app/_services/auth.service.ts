import { AlertifyService } from './alertify.service';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserDTO } from '../_models/UserDTO';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { UserLoginDTO } from '../_models/UserLoginDTO';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authUrl = environment.baseUrl + '/JwtAuth/';
  private jwtHelper = new JwtHelperService();

  public currentUser: UserDTO;
  public isLoggedIn: boolean;

  constructor(private http: HttpClient, private alertify: AlertifyService) {
    this.currentUser = this.getUser();
    this.isLoggedIn = this.userIsLogged();
  }

  login(userDTO: UserLoginDTO) {
    return this.http.post(this.authUrl + 'login', userDTO).pipe(
      map((response: any) => {
        if (response) {
          localStorage.setItem('token', response.token.toString());
          localStorage.setItem('user', JSON.stringify(response.user));
          this.currentUser = this.getUser();
          this.isLoggedIn = this.userIsLogged();
        }
      })
    );
  }

  getToken() {
    return this.userIsLogged() ? localStorage.getItem('token') : '';
  }

  logout() {
    localStorage.setItem('token', '');
    localStorage.setItem('user', '');
    this.isLoggedIn = this.userIsLogged();
    this.currentUser = this.getUser();
    this.alertify.success('logged out successfully');
  }

  private getUser(): UserDTO {
    return localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')) as UserDTO : null;
  }

  private userIsLogged(): boolean {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

}
