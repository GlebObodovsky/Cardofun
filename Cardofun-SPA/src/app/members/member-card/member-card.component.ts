import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user/user.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { FriendshipStatus } from 'src/app/_models/friendshipStatus';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(private userService: UserService, private alerifyService: AlertifyService) { }

  ngOnInit() {
  }

  addUserAsFriend(id: number) {
    this.userService.requestFriendship(id).subscribe(next => {
      this.user.friendshipStatus = FriendshipStatus.requested;
      this.alerifyService.success('Friend request has been successfully sent');
    }, error => {
      this.alerifyService.error(error);
    });
  }

  deleteUserFromFriends(id: number) {
    this.userService.deleteFriendship(id).subscribe(next => {
      this.user.friendshipStatus = null;
      this.alerifyService.success('Friendship has been successfully deleted');
    }, error => {
      this.alerifyService.error(error);
    });
  }
}
