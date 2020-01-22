import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit-resolver';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { FriendListResolver } from './_resolvers/friend-list-resolver';
import { MessageDialoguesResolver } from './_resolvers/message-dialogues-resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent, resolve: { users: MemberListResolver } },
            { path: 'members/:id', component: MemberDetailComponent, resolve: { user: MemberDetailResolver }},
            { path: 'member/edit', component: MemberEditComponent, resolve: {user: MemberEditResolver},
                canDeactivate: [PreventUnsavedChangesGuard] },
            { path: 'messages', component: MessagesComponent, resolve: { messages: MessageDialoguesResolver } },
            { path: 'messages/:container', component: MessagesComponent, resolve: { messages: MessageDialoguesResolver } },
            { path: 'friends', component: MemberListComponent, resolve: { users: FriendListResolver }  },
            { path: 'friends/:state', component: MemberListComponent, resolve: { users: FriendListResolver }  },
            { path: 'admin', component: AdminPanelComponent, data: { roles: ['Admin', 'Moderator'] }}
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
