import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertifyService: AlertifyService,
    private router: Router) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertifyService.success('Logged in sucessfully');
    }, error => {
      this.alertifyService.error(error);
    }, () => {
      this.router.navigate(['/messages']);
    });
  }

  logout() {
    this.authService.logout();
    this.alertifyService.success('Logged out sucessfully');
    this.router.navigate(['']);
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
