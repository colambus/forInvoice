import { Component, OnInit } from '@angular/core';
import { InvoiceModel } from '../../../models/invoice.model';
import { InvoiceService } from '../../../services/invoice.service';
import { DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { InvoiceComponent } from '../invoice.component';
import { Validators, FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { EditService } from '../../../services/edit.service';
import { DialogAction } from '@progress/kendo-angular-dialog/dist/es2015/dialog/dialog-settings';

@Component({
    selector: 'invoiceList',
    templateUrl: './invoiceList.component.html',
    providers: [InvoiceService, EditService]
})

export class InvoiceListComponent implements OnInit {
    gridInvoices: InvoiceModel[];
    private confirmDelete: boolean = false;
    private isNew: boolean = false;
    public currInvoice: InvoiceModel;
    public formGroup: FormGroup | undefined;
    public invoiceSelection: number;

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

    onSelectionRowChange(invoice: any) {
        if (invoice.selectedRows[0])
            this.currInvoice = invoice.selectedRows[0].dataItem;
    }

    public addHandler({ sender }: any) {
        //console.log(newInvoice);

        let newInvoice: InvoiceModel = new InvoiceModel();
        this.isNew = true;
        let loaded = false;
        this.invoiceService.create()
            .subscribe(
            result => {
                newInvoice = result;
                this.createNewInvoiceForm(newInvoice);
            },
            error => console.log("Error :: " + error));
    }

    private editSelectedInvoice(invoice: InvoiceModel) {
        const dialogRef = this.dialogService.open({
            title: 'Edit invoice',

            // Show component
            content: InvoiceComponent,

            actions: [
                { text: 'Save', primary: true },
                { text: 'Close' }
            ]
        });

        dialogRef.content.instance.invoice = invoice;

        dialogRef.result.subscribe((result) => {
            if ((result as DialogAction).text === 'Save')
                this.invoiceService.save(dialogRef.content.instance.invoice).subscribe(result => {
                    this.load();
                });
        });
    }

    public editInvoice(sender: any, eventArgs: any) {
        if (this.currInvoice) {
            this.isNew = false;
            this.invoiceService.getById(this.currInvoice)
                .subscribe(
                result => {
                    this.currInvoice = result;
                    this.editSelectedInvoice(this.currInvoice);
                },
                error => console.log("Error :: " + error));
        }
    }

    public createNewInvoiceForm(invoice: InvoiceModel) {
        const dialogRef = this.dialogService.open({
            title: 'New invoice',

            // Show component
            content: InvoiceComponent,

            actions: [
                { text: 'Save', primary: true },
                { text: 'Close' }
            ]
        });

        dialogRef.content.instance.invoice = invoice;

        dialogRef.result.subscribe((result) => {
            if (result instanceof DialogCloseResult) {
                this.onDeleteData(invoice.id)
            } else {
                if ((result as DialogAction).text === 'Save')
                    this.invoiceService.save(dialogRef.content.instance.invoice).subscribe(result => {
                        this.load()
                    });
                else
                    this.onDeleteData(invoice.id);
            }
        })
    }

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'invoiceNumber': dataItem.invoiceNo,
            'date': dataItem.date
        });
    }

    public removeHandler({ dataItem }: any) {
        const dialogRef = this.dialogService.open({
            title: 'Confirmation',

            // Show component
            content: 'Are you sure?',

            actions: [
                { text: 'Yes' },
                { text: 'No', primary: true }
            ]
        });

        dialogRef.result.subscribe((result) => {
            if (result instanceof DialogCloseResult) {
                console.log('close');
            } else {
                if ((result as DialogAction).text === 'Yes')
                    this.onDeleteData(dataItem.id);
                else
                    console.log('Action close');
                ;
            }
        });
    }

    onDeleteData(id: number) {
        this.invoiceService.deleteItem(id)
            .subscribe(data => this.load());
    }
}