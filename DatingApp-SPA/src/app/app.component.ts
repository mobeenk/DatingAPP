import { Component } from '@angular/core';
import { AuthService } from './_services/auth.service';
import {JwtHelperService} from '@auth0/angular-jwt';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {}

  // title = 'DatingApp-SPA';
 // tslint:disable-next-line: use-lifecycle-interface
 ngOnInit() {
    const token = localStorage.getItem('token');
    //  turn it into string from JSON
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      // this.authService.changeMemberPhoto(user.photoUrl);
    }
  }
}
