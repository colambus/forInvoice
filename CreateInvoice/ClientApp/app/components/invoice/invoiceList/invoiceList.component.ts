import { Component, OnInit } from '@angular/core';
import { InvoiceModel } from '../../../models/invoice.model';
import { InvoiceService } from '../../../services/invoice.service';
import { DialogService } from '@progress/kendo-angular-dialog';


@Component({
    selector: 'invoiceList',
    templateUrl: './invoiceList.component.html',
    providers: [InvoiceService]
})

export class InvoiceListComponent implements OnInit {
    gridInvoices: InvoiceModel[];
    private opened: boolean = false;

    constructor(private invoiceService: InvoiceService,
        private dialogService: DialogService) {
    }

    ngOnInit() {
        this.invoiceService.getAll().subscribe(
            resultArray => this.gridInvoices = resultArray,
            error => console.log("Error :: " + error));
    }

    public addHandler({ sender }: any) {
        console.log("Add invoice");
        this.opened = true;
        const dialogRef = this.dialogService.open({
            title: 'Please confirm',

            // Show component
            content: '<h1>Invoices</h1>',

            actions: [
                { text: 'Cancel' },
                { text: 'Delete', primary: true }
            ]
        });
    }

    public close() {
        this.opened = false;
    }
}