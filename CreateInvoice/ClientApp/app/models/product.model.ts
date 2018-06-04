import { UnitModel } from './unit.model';
import { CountryOfOriginModel } from './countryOfOrigin.model';
import { NamedIdObject } from './NamedIdObject.model';

export class ProductModel {
    id: number;
    descriptionEn: string;
    descriptionUa: string;
    certificate: NamedIdObject;
    codeNo: string;
    countryOfOrigin: CountryOfOriginModel;

    constructor() {
        this.countryOfOrigin = new CountryOfOriginModel();
        this.descriptionEn = "";
        this.descriptionUa = "";
        this.codeNo = "";
        this.certificate = new NamedIdObject();
    }
}