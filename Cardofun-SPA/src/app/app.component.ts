import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Cardofun';

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.refreshDecodedToken();
  }
}