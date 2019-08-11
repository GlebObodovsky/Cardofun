import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';
import { AlertifyService } from '../_services/alertify/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertifyService.success('Logged in sucessfully');
    }, error => {
      this.alertifyService.error(error);
    });
  }

  logout() {
    this.alertifyService.success('Logged out sucessfully');
    return localStorage.removeItem('token');
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
