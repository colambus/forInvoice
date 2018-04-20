import { Component, OnInit } from '@angular/core';
import { Data } from '@angular/router/src/config';
import { getToday } from '@progress/kendo-angular-dateinputs/dist/es2015/util';
import { InvoiceModel } from '../../models/invoice.model';
import { NamedIdObject } from '../../models/NamedIdObject.model';
import { Organization } from '../../models/organization.model';
import { DataNamedService } from '../../services/dataNamed.service';

@Component({
    selector: 'invoice',
    templateUrl: './invoice.component.html',
    providers: [DataNamedService]
})
export class InvoiceComponent implements OnInit {
    invoice: InvoiceModel = {
        id: 0,
        date: getToday(),
        invoiceNo: "",
        seller: { id: 0, name: "" },
        buyer: { id: 0, name: "" },
        contract: "",
        deliveryType: "",
        termsOfDelivery: "",
        paymentIdentification: [],
        orderNo: []
    };
    termsOfDelivery: NamedIdObject[];
    deliveryTypes: NamedIdObject[];
    termsOfPayment: NamedIdObject[];
    contracts: NamedIdObject[];

    currDate: Date = getToday();
    public organizations: Array<Organization>;

    constructor(private dataNamedService: DataNamedService) {
    } 

    ngOnInit() {
        this.dataNamedService.getAll('Organization')
            .subscribe(
            resultArray => this.organizations = resultArray,
            error => console.log("Error :: " + error));

        this.dataNamedService.getAll('DeliveryType')
            .subscribe(
            resultArray => this.deliveryTypes = resultArray,
            error => console.log("Error :: " + error));

        this.dataNamedService.getAll('TermOfPayment')
            .subscribe(
            resultArray => this.termsOfDelivery = resultArray,
            error => console.log("Error :: " + error));

        this.dataNamedService.getAll('TermsOfDelivery')
            .subscribe(
            resultArray => this.termsOfPayment = resultArray,
            error => console.log("Error :: " + error));
    }



    onPaymentIdentificationChange(value: any) {
        this.invoice.paymentIdentification = value;
    }

    onOrderNoChange(value: any) {
        this.invoice.orderNo = value;
    }

    onSellerChange(value: any) {
        this.invoice.orderNo = value;
    }

    onBuyerChange(value: any) {
        this.invoice.orderNo = value;
    }
}

