import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

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
  bsConfig: Partial<BsDatepickerConfig>;
  // injecting the angular service
  constructor(private authService: AuthService, private alertify: AlertifyService
              ,private fb: FormBuilder) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-orange'
    },


    this.createRegisterForm();
    // this.registerForm = new FormGroup({
    //   username: new FormControl('',Validators.required),
    //   password: new FormControl('',
    //     [Validators.required, Validators.minLength(4),Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('',Validators.required)
    // }, this.passwordMatchValidator);  
  }

  createRegisterForm(){
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['',Validators.required],
      knownAs: ['',Validators.required],
      dateOfBirth: [null,Validators.required],
      city: ['',Validators.required],
      country: ['',Validators.required],
      password: ['',[Validators.required, Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword: ['',Validators.required]
    }, {validator: this.passwordMatchValidator})// custom validator to match both passwords
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
