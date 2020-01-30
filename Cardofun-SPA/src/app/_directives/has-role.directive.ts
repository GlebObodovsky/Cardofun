import { Directive, Input, ViewContainerRef, TemplateRef, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth/auth.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  isVisible: boolean;

  constructor(private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>, private authService: AuthService) { }

    ngOnInit(): void {
      const userRoles = this.authService.currentUser.roles;

      // if there are no roles - clear the view container reference
      if (!userRoles) {
        this.viewContainerRef.clear();
      }

      // if the user has the needed role - render the element
      if (this.authService.roleMatch(this.appHasRole)) {
        if (!this.isVisible) {
          this.isVisible = true;
          this.viewContainerRef.createEmbeddedView(this.templateRef);
        } else {
          this.isVisible = false;
          this.viewContainerRef.clear();
        }
      }
    }
}
