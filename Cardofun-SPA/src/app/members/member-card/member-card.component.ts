import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';
import { FriendshipStatus } from 'src/app/_models/enums/friendshipStatus';
import { FriendService } from 'src/app/_services/friend/friend.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  @Output()
  excludedFromFriendlist = new EventEmitter<User>();
  @Output()
  addedToFriendlist = new EventEmitter<User>();

  constructor(private friendService: FriendService, private alerifyService: AlertifyService) { }

  ngOnInit() {
  }

  addUserAsFriend(id: number) {
    this.friendService.requestFriendship(id).subscribe(next => {
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
    this.friendService.changeFriendshipStatus(id, FriendshipStatus.accepted).subscribe(next => {
      this.user.friendship.status = FriendshipStatus.accepted;
      this.addedToFriendlist.emit(this.user);
      this.alerifyService.success('Friendship has been successfully accepted');
    }, error => {
      this.alerifyService.error(error);
    });
  }

  deleteUserFromFriends(id: number, isOwner?: boolean) {
    this.alerifyService.confirm('Are you sure you want to delete ' + this.user.name + ' from friends', () => {
      if (isOwner) {
        this.friendService.changeFriendshipStatus(id, FriendshipStatus.declined).subscribe(next => {
          this.user.friendship.status = FriendshipStatus.declined;
          this.alerifyService.success('Friendship has been successfully declined');
        }, error => {
          this.alerifyService.error(error);
        });
      } else {
        this.friendService.deleteFriendship(id).subscribe(next => {
          this.user.friendship = null;
          this.alerifyService.success('Friendship has been successfully deleted');
        }, error => {
          this.alerifyService.error(error);
        });
      }

      this.excludedFromFriendlist.emit(this.user);
    });

  }
}
