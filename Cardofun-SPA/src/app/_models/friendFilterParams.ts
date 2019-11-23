import { UserFilterParams } from './userFilterParams';
import { FriendshipStatus } from './enums/friendshipStatus';

export interface FriendFilterParams extends UserFilterParams {
    friendshipStatus?: FriendshipStatus;
    isFriendshipOwned?: Boolean;
}
