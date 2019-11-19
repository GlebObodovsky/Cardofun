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
      this.user.friendship = {
        isOwner: false,
        status: FriendshipStatus.requested
      };
      this.alerifyService.success('Friend request has been successfully sent');
    }, error => {
      this.alerifyService.error(error);
    });
  }

  acceptFriendship(id: number) {
    this.userService.changeFriendshipStatus(id, FriendshipStatus.accepted).subscribe(next => {
      this.user.friendship.status = FriendshipStatus.accepted;
      this.alerifyService.success('Friendship has been successfully accepted');
    }, error => {
      this.alerifyService.error(error);
    });
  }

  deleteUserFromFriends(id: number, isOwner?: boolean) {
    if (isOwner) {
      this.userService.changeFriendshipStatus(id, FriendshipStatus.declined).subscribe(next => {
        this.user.friendship.status = FriendshipStatus.declined;
        this.alerifyService.success('Friendship has been successfully declined');
      }, error => {
        this.alerifyService.error(error);
      });
    } else {
      this.userService.deleteFriendship(id).subscribe(next => {
        this.user.friendship = null;
        this.alerifyService.success('Friendship has been successfully deleted');
      }, error => {
        this.alerifyService.error(error);
      });
    }
  }
}
