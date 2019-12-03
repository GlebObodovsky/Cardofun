export interface Message {
    id: string;
    senderId: number;
    recipientId: number;
    sentAt: Date;
    isRead: Boolean;
    senderName?: string;
    senderPhotoUrl?: string;
    recipientName?: string;
    recipientPhotoUrl?: string;
    text?: string;
    photoUrl?: string;
    readAt?: Date;
}
