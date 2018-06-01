import { Organization } from './organization.model';
import { InvoiceModel } from './invoice.model';
import { ProductModel } from './product.model';
import { CountryOfOriginModel } from './countryOfOrigin.model';
import { UnitModel } from './unit.model';


export class InvoiceProductModel {
    id: number;
    position: number;
    invoice: InvoiceModel;
    unitPrice: number;
    quantity: number;
    product: ProductModel;
    unit: UnitModel;

    constructor() {
        this.id = 0;
        this.unitPrice = 0;
        this.quantity = 0;
        this.product = new ProductModel();
        this.unit = new UnitModel();
    }
}