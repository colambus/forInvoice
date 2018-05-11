import { Organization } from './organization.model';
import { NamedIdObject } from './NamedIdObject.model';

export class InvoiceModel {
    id: number;
    date: Date;
    invoiceNo: string;
    seller: Organization;
    buyer: Organization;
    contract: string;
    deliveryType: NamedIdObject;
    termsOfDelivery: NamedIdObject;
    termOfPayment: NamedIdObject;
    paymentIdentification: string;
    orderNo: string;
}