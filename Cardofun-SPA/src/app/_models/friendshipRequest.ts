import { FriendshipStatus } from './friendshipStatus';

export interface FriendshipRequest {
    isOwner: boolean;
    status: FriendshipStatus;
}
