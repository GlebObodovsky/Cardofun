import { Message } from './message';
import { UserForMessage } from './user-for-message';

export class MessageThread {
    users: UserForMessage[];
    messages: Message[];
}
