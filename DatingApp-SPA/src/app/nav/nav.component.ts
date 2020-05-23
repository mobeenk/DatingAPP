import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};//stores user and pass
  //AuthService is the service class name
  constructor(public authService: AuthService, private alertify: AlertifyService) { }
  ngOnInit() {  }

  login() {
    this.authService.login(this.model).subscribe( next => {
      this.alertify.success('logged in successfully');
    }, error => {
      this.alertify.error(error);
    });
  }

  loggedIn() {
    // from the service we used the 3rd party jwt auth0
    return this.authService.loggedIn();
  }
  logout() {
    localStorage.removeItem('token');
    this.alertify.message('logged out');
  }
}
