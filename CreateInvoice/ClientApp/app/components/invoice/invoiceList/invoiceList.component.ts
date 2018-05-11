import { Component, OnInit } from '@angular/core';
import { InvoiceModel } from '../../../models/invoice.model';
import { InvoiceService } from '../../../services/invoice.service';
import { DialogService } from '@progress/kendo-angular-dialog';
import { InvoiceComponent } from '../invoice.component';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { EditService } from '../../../services/edit.service';

@Component({
    selector: 'invoiceList',
    templateUrl: './invoiceList.component.html',
    providers: [InvoiceService, EditService]
})

export class InvoiceListComponent implements OnInit {
    gridInvoices: InvoiceModel[];
    private confirmDelete: boolean = false;
    private isNew: boolean = false;
    private currInvoice: InvoiceModel;
    public formGroup: FormGroup | undefined;

    constructor(private invoiceService: InvoiceService,
        private dialogService: DialogService,
        private formBuilder: FormBuilder,
        private editService: EditService) {
    }

    ngOnInit() {
        this.load();
    }

    load() {
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

        dialogRef.content.instance.invoice = this.currInvoice;
    }

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'invoiceNumber': dataItem.invoiceNo,
            'date': dataItem.date
        });
    }

    public removeHandler({ dataItem }: any) {
        this.confirmDelete = true;

    }

    onDeleteData() {
        this.editService.remove(dataItem);
        this.invoiceService.deleteItem(dataItem.id)
            .subscribe(data => this.load());
    }

    public close() {
        this.confirmDelete = false;
    }
}