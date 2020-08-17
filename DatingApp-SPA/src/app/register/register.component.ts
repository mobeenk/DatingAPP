import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  // injecting the angular service
  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.registerForm = new FormGroup({
      username: new FormControl('',Validators.required),
      password: new FormControl('',
        [Validators.required, Validators.minLength(4),Validators.maxLength(8)]),
      confirmPassword: new FormControl('',Validators.required)
    }, this.passwordMatchValidator);  // custom validator to match both passwords
  }

  passwordMatchValidator(g: FormControl)
  {
    return g.get('password').value === g.get('confirmPassword').value ? null: {'mismatch': true};
  }

  register() {
    // this.authService.register(this.model).subscribe(() => {
    //    this.alertify.success('registeration succeful');
    // }, error => {
    //     this.alertify.error(error);
    //    }
    // );
    console.log(this.registerForm.value);
  }
  cancel() {
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
