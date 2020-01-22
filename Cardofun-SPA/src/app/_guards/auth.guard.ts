import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';
import { AlertifyService } from '../_services/alertify/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router,
    private alertifyService: AlertifyService) {}

  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>;

    if (roles) {
      if (this.authService.roleMatch(roles)) {
        return true;
      } else {
        this.alertifyService.error('You\'re not allowed to navigate to this area');
        this.router.navigate(['/messages']);
      }
    }

    if (this.authService.isLoggedIn()) {
      return true;
    }

    this.router.navigate(['/home']);
  }
}
