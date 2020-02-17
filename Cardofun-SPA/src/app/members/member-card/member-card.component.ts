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
      this.alerifyService.success('Friend request to ' + this.user + ' has been successfully sent');
    }, error => {
      this.alerifyService.error(error);
    });
  }

  acceptFriendship(id: number) {
    this.friendService.changeFriendshipStatus(id, FriendshipStatus.accepted).subscribe(next => {
      this.user.friendship.status = FriendshipStatus.accepted;
      this.addedToFriendlist.emit(this.user);
      this.alerifyService.success('Friendship request from ' + this.user.name + ' has been successfully accepted');
    }, error => {
      this.alerifyService.error(error);
    });
  }

  deleteUserFromFriends(id: number, isOwner?: boolean) {
    const currentList = this.user.friendship.status !== FriendshipStatus.accepted
      ? 'list of your subscribers'
      : 'your friend list';
    this.alerifyService.confirm('Are you sure you want to delete ' + this.user.name + ' from ' + currentList + '?', () => {
      if (isOwner) {
        this.friendService.changeFriendshipStatus(id, FriendshipStatus.declined).subscribe(() => {
          this.alerifyService.success('Friendship request from ' + this.user.name + ' has been declined');
        }, error => {
          this.alerifyService.error(error);
        });
      } else {
        this.friendService.deleteFriendship(id).subscribe(() => {
          this.alerifyService.success(this.user.name + ' has been removed from ' + currentList);
        }, error => {
          this.alerifyService.error(error);
        });
      }

      this.excludedFromFriendlist.emit(this.user);
    });

  }
}
