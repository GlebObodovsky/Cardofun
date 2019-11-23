import { FriendshipStatus } from './enums/friendshipStatus';

export interface FriendshipRequest {
    isOwner: boolean;
    status: FriendshipStatus;
}
