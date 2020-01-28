import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin/admin.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { UserForModeration } from 'src/app/_models/user-for-moderation';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-manager',
  templateUrl: './user-manager.component.html',
  styleUrls: ['./user-manager.component.css']
})
export class UserManagerComponent implements OnInit {

  constructor(private adminService: AdminService,
    private alertifyService: AlertifyService, private modalService: BsModalService) { }

  userName: string;
  user: UserForModeration;
  bsModalRef: BsModalRef;
  roles: string[];

  ngOnInit() {
    this.getExistingRoles();
  }

  loadUser() {
    if (!this.userName) {
      return;
    } else {
      this.adminService.getUserWithRoles(this.userName).subscribe(nextUser => {
        this.user = nextUser;
      }, error => {
        this.user = null;
        this.alertifyService.error(error);
      });
    }
  }

  editRolesModal() {
    const initialState = {
      user: this.user,
      roles: this.getRolesForModal()
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
  }

  private getExistingRoles() {
    if (!this.roles) {
      this.adminService.getRoles().subscribe(roles => {
        this.roles = roles;
      }, error => {
        this.alertifyService.error(error);
      });
    }
  }

  private getRolesForModal() {
    const userRoles = this.user.roles;
    const roles = this.roles.map(role => {
      return {name: role, value: role, checked: false};
    });
    for (let i = 0; i < roles.length; i++) {
      for (let j = 0; j < userRoles.length; j++) {
        if (roles[i].name === userRoles[j]) {
          roles[i].checked = true;
          break;
        }
      }
    }
    return roles;
  }
}
