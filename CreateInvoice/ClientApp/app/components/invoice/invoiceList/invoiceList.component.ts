import { Component, OnInit } from '@angular/core';
import { InvoiceModel } from '../../../models/invoice.model';
import { InvoiceService } from '../../../services/invoice.service';
import { DialogService } from '@progress/kendo-angular-dialog';
import { InvoiceComponent } from '../invoice.component';


@Component({
    selector: 'invoiceList',
    templateUrl: './invoiceList.component.html',
    providers: [InvoiceService]
})

export class InvoiceListComponent implements OnInit {
    gridInvoices: InvoiceModel[];
    private opened: boolean = false;
    private isNew: boolean = false;
    private currInvoice: InvoiceModel;

    constructor(private invoiceService: InvoiceService,
        private dialogService: DialogService) {
    }

    ngOnInit() {
        this.invoiceService.getAll().subscribe(
            resultArray => this.gridInvoices = resultArray,
            error => console.log("Error :: " + error));
    }

    public addHandler({ sender }: any) {
        console.log("New Invoice");
        this.isNew = true;
        this.invoiceService.create().subscribe(
            result => this.currInvoice = result,
            error => console.log("Error :: " + error));

        const dialogRef = this.dialogService.open({
            title: 'New invoice',

            // Show component
            content: InvoiceComponent,

            actions: [
                { text: 'Save', primary: true },
                { text: 'Close' }
            ]
        });
    }

    public close() {
        this.opened = false;
    }
}