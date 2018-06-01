import { UnitModel } from './unit.model';
import { CountryOfOriginModel } from './countryOfOrigin.model';

export class ProductModel {
    id: number;
    descriptionEn: string;
    descriptionUa: string;
    certificate: string;
    codeNo: string;
    countryOfOrigin: CountryOfOriginModel;

    constructor() {
        this.countryOfOrigin = new CountryOfOriginModel();
        this.descriptionEn = "";
        this.descriptionUa = "";
        this.codeNo = "";
    }
}