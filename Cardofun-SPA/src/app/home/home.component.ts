import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  isInRegistryMode = false;

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  setRegistrationVisibility(showRegForm: boolean = true) {
    this.isInRegistryMode = showRegForm;
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
