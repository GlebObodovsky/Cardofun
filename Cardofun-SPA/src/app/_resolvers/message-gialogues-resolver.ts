import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../_services/user/user.service';
import { AlertifyService } from '../_services/alertify/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { MessageService } from '../_services/message/message.service';
import { environment } from 'src/environments/environment';
import { MessageContainer } from '../_models/enums/messageContainer';

@Injectable()
export class MessageDialoguesResolver implements Resolve<Message[]> {
    pageNumber = 1;
    pageSize = environment.messageContainerPageSize;

    constructor(private messageService: MessageService, private alertifyService: AlertifyService,
        private router: Router) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        const messageContainer: MessageContainer = route.params['container'];

        return this.messageService.getDialogues(this.pageNumber, this.pageSize, messageContainer).pipe(
            catchError(error => {
                this.alertifyService.error('Problem retreiving data');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
