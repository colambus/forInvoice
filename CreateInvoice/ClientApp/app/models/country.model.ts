import { NamedIdObject} from './NamedIdObject.model';
export class CountryModel extends NamedIdObject {
    id: number;
    descriptionEn: string;
    descriptionUa: string;

    constructor() {
        super();
        this.descriptionEn = "";
        this.descriptionUa = "";
    }
}