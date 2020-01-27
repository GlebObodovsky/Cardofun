import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin/admin.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { UserForModeration } from 'src/app/_models/user-for-moderation';

@Component({
  selector: 'app-user-manager',
  templateUrl: './user-manager.component.html',
  styleUrls: ['./user-manager.component.css']
})
export class UserManagerComponent implements OnInit {

  constructor(private adminService: AdminService, private alertifyService: AlertifyService) { }

  userName: string;
  user: UserForModeration;

  ngOnInit() {
  }

  loadUser() {
    if (!this.userName) {
      return;
    } else {
      this.adminService.getUserWithRoles(this.userName).subscribe(nextUser => {
        this.user = nextUser;
        console.log(this.user.roles);
      }, error => {
        this.user = null;
        this.alertifyService.error('Cannot retrieve the user\'s info');
      });
    }
  }
}
