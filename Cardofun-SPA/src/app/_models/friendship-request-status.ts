import { FriendshipStatus } from './enums/friendshipStatus';

export interface FriendshipRequestStatus {
        fromUserId: number;
        toUserId: number;
        status: FriendshipStatus;
        isDeleted: boolean;
}
