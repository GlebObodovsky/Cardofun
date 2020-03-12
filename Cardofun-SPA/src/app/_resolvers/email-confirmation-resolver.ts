import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserService } from '../_services/user/user.service';

@Injectable()
export class EmailConfirmationResolver implements Resolve<Boolean> {

    constructor(private alertifyService: AlertifyService, private router: Router,
        private userService: UserService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Boolean> {
        console.log(route.queryParams);


        const id: string = route.queryParams['id'];
        console.log('Id: ' + id);
        const token: string = route.queryParams['token'];

        console.log('Id: ' + id + '   token: ' + token);

        return this.userService.verifyUser(+id, token).pipe(
            catchError(error => {
                this.alertifyService.error('Problem verifying user');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
