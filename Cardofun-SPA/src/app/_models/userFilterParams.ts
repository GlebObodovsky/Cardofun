import { SupscriptionState } from './enums/supscriptionState';

export interface UserFilterParams {
    sex?: string;
    ageMin?: number;
    ageMax?: number;
    cityId?: number;
    countryIsoCode?: string;
    languageSpeakingCode?: string;
    languageLearningCode?: string;
    subscriptionState?: SupscriptionState;
}
