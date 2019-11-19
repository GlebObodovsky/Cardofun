import { City } from './City';
import { Language } from './language';
import { Photo } from './photo';
import { FriendshipRequest } from './friendshipRequest';

export interface User {
    id: number;
    login: string;
    name: string;
    age: number;
    sex: string;
    city: City;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    languagesTheUserSpeaks: Language[];
    languagesTheUserLearns: Language[];
    birthDate?: Date;
    cityId?: number;
    introduction?: string;
    email?: string;
    friendship?: FriendshipRequest;
    photos?: Photo[];
}
