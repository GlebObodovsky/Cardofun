import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule, TabsModule, BsDatepickerModule, PaginationModule, ButtonsModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';
import { JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { NgSelectModule } from '@ng-select/ng-select';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeAgoPipe } from 'time-ago-pipe';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AuthService } from './_services/auth/auth.service';
import { UserService } from './_services/user/user.service';
import { FriendService } from './_services/friend/friend.service';
import { MessageService } from './_services/message/message.service';
import { CityService } from './_services/city/city.service';
import { CountryService } from './_services/country/country.service';
import { LanguageService } from './_services/language/language.service';
import { AlertifyService } from './_services/alertify/alertify.service';
import { LocalStorageService } from './_services/local-storage/local-storage.service';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { FriendListResolver } from './_resolvers/friend-list-resolver';
import { MessageDialoguesResolver } from './_resolvers/message-dialogues-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { environment } from 'src/environments/environment';
import { EnumToArrayPipe } from './_pipes/enumToArray/enumToArray.pipe';
import { HasRoleDirective } from './_directives/has-role.directive';

export const jwtOptionsFactory = (localStorageSvc: LocalStorageService) => ({
   tokenGetter: () => localStorageSvc.getToken(),
   whitelistedDomains: environment.sendTokenToPaths,
   blacklistedRoutes: environment.dontSendTokenToPaths
});

export class CustomHammerConfig extends HammerGestureConfig  {
   overrides = {
       pinch: { enable: false },
       rotate: { enable: false }
   };
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      AdminPanelComponent,
      MessagesComponent,
      MemberListComponent,
      MemberCardComponent,
      MemberDetailComponent,
      MemberEditComponent,
      MemberMessagesComponent,
      PhotoEditorComponent,
      TimeAgoPipe,
      EnumToArrayPipe,
      HasRoleDirective
   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      BsDropdownModule.forRoot(),
      BsDatepickerModule.forRoot(),
      TabsModule.forRoot(),
      PaginationModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      ButtonsModule.forRoot(),
      NgxGalleryModule,
      NgSelectModule,
      FileUploadModule,
      JwtModule.forRoot({
         jwtOptionsProvider: {
            provide: JWT_OPTIONS,
            deps: [LocalStorageService],
            useFactory: jwtOptionsFactory
        }
      })
   ],
   providers: [
      AuthService,
      UserService,
      FriendService,
      MessageService,
      CityService,
      CountryService,
      LanguageService,
      AlertifyService,
      LocalStorageService,
      ErrorInterceptorProvider,
      AuthGuard,
      PreventUnsavedChangesGuard,
      MemberDetailResolver,
      MemberListResolver,
      MemberEditResolver,
      FriendListResolver,
      MessageDialoguesResolver,
      { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
