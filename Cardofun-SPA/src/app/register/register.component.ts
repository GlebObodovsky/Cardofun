import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Observable, Subject, concat, of } from 'rxjs';
import { City } from '../_models/City';
import { debounceTime, tap, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { CityService } from '../_services/city/city.service';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { UserService } from '../_services/user/user.service';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;

  citiesLoading = false;
  cities$: Observable<City[]>;
  citiesInput$ = new Subject<string>();

  constructor(private authService: AuthService, private userService: UserService,
    private fb: FormBuilder, private cityService: CityService,
    private alertifyService: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-default',
      dateInputFormat: 'YYYY-MM-DD'
    };
    this.createRegisterForm();
    this.loadCities();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      sex: ['male'],
      userName: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(12)], this.userNameIsTaken.bind(this)],
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email], this.emailIsTaken.bind(this)],
      birthDate: [null, Validators.required],
      cityId: [null, Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(20)]],
      confirmPassword: ['', [Validators.required]]
    }, {validator: this.passwordMatchValidator});
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alertifyService.success('Registration successful. Make sure to confirm your email.');
        this.cancel();
      }, error => {
        this.alertifyService.error(error);
      });
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  userNameIsTaken(c: FormControl) {
    const isTaken = new Promise((resolve, reject) => {
      setTimeout(() => {
        this.userService.checkIfUserNameExists(c.value).subscribe(() => {
          resolve({ 'usernameistaken': true });
        }, () => { resolve(null); });
      }, 1000);
    });
    return isTaken;
  }

  emailIsTaken(c: FormControl) {
    const isTaken = new Promise((resolve, reject) => {
      setTimeout(() => {
        this.userService.checkIfEmailExists(c.value).subscribe(() => {
          resolve({ 'emailistaken': true });
        }, () => { resolve(null); });
      }, 1000);
    });
    return isTaken;
  }

  private loadCities() {
    this.cities$ = concat(
        of([]), // default items
        this.citiesInput$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            tap(() => this.citiesLoading = true),
            switchMap(term => this.getCities(term).pipe(
                catchError(() => of([])), // empty list on error
                tap(() => this.citiesLoading = false)
            ))
        )
    );
  }

  private getCities(term): Observable<City[]> {
    if (term == null) {
      return of([]);
    }
    return this.cityService.getCities(term);
  }
}
