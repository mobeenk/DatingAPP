import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
@Injectable({
  providedIn: 'root'// root is the app module and we must add it there
})
// register and login methods here are used in register component ts file
// becauyse this service is injected in there
export class AuthService {
  baseURL = 'http://localhost:5000/api/auth/';
  constructor(private http: HttpClient) { }

  login(model: any) {
    // model is the body in request , whenever going to this link it will store it
    return this.http.post(this.baseURL + 'login', model).pipe
    (
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token )
        }
      })
    );
  }
  register(model: any) {
    return this.http.post(this.baseURL + 'register', model);
  }

}
