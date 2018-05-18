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
        //console.log(newInvoice);

        let newInvoice: InvoiceModel = new InvoiceModel(); 
        this.isNew = true;
        this.invoiceService.create()
            .subscribe(
            result => newInvoice = result,
            error => console.log("Error :: " + error));

        //console.log(newInvoice);

        const dialogRef = this.dialogService.open({
            title: 'New invoice',

            // Show component
            content: InvoiceComponent,

            actions: [
                { text: 'Save', primary: true },
                { text: 'Close' }
            ]
        });

        dialogRef.content.instance.invoice = newInvoice;
    }

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'invoiceNumber': dataItem.invoiceNo,
            'date': dataItem.date
        });
    }

    public removeHandler({ dataItem }: any) {
        //this.confirmDelete = true;

        const dialogRef = this.dialogService.open({
            title: 'Confirmation',

            // Show component
            content: 'Are you sure?',

            actions: [
                { text: 'Yes'},
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