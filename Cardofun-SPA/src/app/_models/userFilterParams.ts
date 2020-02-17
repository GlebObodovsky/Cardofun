import { SubscriptionState } from './enums/subscriptionState';

export interface UserFilterParams {
    sex?: string;
    ageMin?: number;
    ageMax?: number;
    cityId?: number;
    countryIsoCode?: string;
    languageSpeakingCode?: string;
    languageLearningCode?: string;
    subscriptionState?: SubscriptionState;
}
