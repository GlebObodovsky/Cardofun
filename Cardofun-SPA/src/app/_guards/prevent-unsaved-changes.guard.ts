import { Injectable } from '@angular/core';
import { Router, CanDeactivate } from '@angular/router';
import { AuthService } from '../_services/auth/auth.service';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  constructor(private authService: AuthService, private router: Router) {}

  canDeactivate(component: MemberEditComponent): boolean {
    if (component.editForm.dirty) {
        return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }
}
