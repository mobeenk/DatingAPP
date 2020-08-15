import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'// root is the app module and we must add it there
})
// register and login methods here are used in register component ts file
// becauyse this service is injected in there
export class AuthService {
  baseURL = environment.apiURL + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;

  constructor(private http: HttpClient) { }

  login(model: any) {
    // model is the body in request , whenever going to this link it will store it
    return this.http.post(this.baseURL + 'login', model).pipe
    (
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token );
          localStorage.setItem('user', JSON.stringify(user.user));
          // using jwt auth0 package to get user name
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          console.log(this.decodedToken);
        }
      })
    );
  }
  register(model: any) {
    return this.http.post(this.baseURL + 'register', model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

}
