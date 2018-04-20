import { Organization } from './organization.model';

export class InvoiceModel {
    id: number;
    date: Date;
    invoiceNo: string;
    seller: Organization;
    buyer: Organization;
    contract: string;
    deliveryType: string;
    termsOfDelivery: string;
    paymentIdentification: Array<string>;
    orderNo: Array<string>;
}