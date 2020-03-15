import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { UserService } from '../_services/user/user.service';
import { AuthService } from '../_services/auth/auth.service';

@Injectable()
export class EmailConfirmationResolver implements Resolve<Boolean> {

    constructor(private alertifyService: AlertifyService, private router: Router,
        private userService: UserService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Boolean> {
        const id: string = route.queryParams['id'];
        const token: string = route.queryParams['token'];
        return this.userService.verifyUser(+id, token).pipe(
            tap(next => {
                this.alertifyService.success('User email is succesfully confirmed!');
            }),
            catchError(error => {
                this.alertifyService.error(`Problem verifying user: ${error}`);
                return of(null);
            })
        );
    }
}
