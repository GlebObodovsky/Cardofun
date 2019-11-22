import { UserFilterParams } from './userFilterParams';
import { FriendshipStatus } from './friendshipStatus';

export interface FriendFilterParams extends UserFilterParams {
    status?: FriendshipStatus;
}
