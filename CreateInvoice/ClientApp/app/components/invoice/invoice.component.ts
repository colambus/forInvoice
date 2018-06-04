import { Component, OnInit, ViewChild, Inject, ElementRef, Renderer2 } from '@angular/core';
import { Data } from '@angular/router/src/config';
import { getToday } from '@progress/kendo-angular-dateinputs/dist/es2015/util';
import { InvoiceModel } from '../../models/invoice.model';
import { NamedIdObject } from '../../models/NamedIdObject.model';
import { Organization } from '../../models/organization.model';
import { DataNamedService } from '../../services/dataNamed.service';
import { Validators, FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { EditService } from '../../services/edit.service';
import { InvoiceProductModel } from '../../models/invoiceProduct.model';
import { ProductModel } from '../../models/product.model';
import { Observable } from 'rxjs/Observable';
import { GridDataResult, GridComponent, EditEvent } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators/map';
import { ProductService } from '../../services/products.service';
import { InvoiceProductService } from '../../services/invoiceProduct.service';
import { DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { DialogAction } from '@progress/kendo-angular-dialog/dist/es2015/dialog/dialog-settings';

// common constants
const hasClass = (el: any, className: any) => new RegExp(className).test(el.className)

const isChildOf = (el: any, className: any) => {
    while (el && el.parentElement) {
        if (hasClass(el.parentElement, className)) {
            return true;
        }
        el = el.parentElement;
    }
    return false;
};

const createFormGroup = (dataItem: any) => new FormGroup({
    'Position': new FormControl(dataItem.productPosition),
    'CountryOfOrigin': new FormControl(dataItem.product!.countryOfOrigin!.id),
    'CodeNo': new FormControl(dataItem!.product!.codeNo),
    'Product': new FormControl(dataItem!.product, Validators.required),
    'Unit': new FormControl(dataItem!.product!.unit),
    'UnitPrice': new FormControl(dataItem!.unitPrice),
    'Quantity': new FormControl(dataItem!.quantity)
});

@Component({
    selector: 'invoice',
    templateUrl: './invoice.component.html',
    providers: [DataNamedService, EditService, ProductService, InvoiceProductService]
})
export class InvoiceComponent implements OnInit {

    invoice: InvoiceModel = {
        id: 0,
        date: getToday(),
        invoiceNo: "",
        seller: { id: 0, name: "" },
        buyer: { id: 0, name: "" },
        contract: "",
        termOfPayment: { id: 0, name: "" },
        deliveryType: { id: 0, name: "" },
        termsOfDelivery: { id: 0, name: "" },
        paymentIdentification: "",
        orderNo: ""
    };
    termsOfDelivery: NamedIdObject[];
    deliveryTypes: NamedIdObject[];
    termsOfPayment: NamedIdObject[];
    contracts: NamedIdObject[];
    products: ProductModel[];
    gridProducts: InvoiceProductModel[];
    currDate: Date = getToday();
    public organizations: Array<Organization>;
    private isNew = false;
    private editedRowIndex: number | undefined;
    public formGroup: FormGroup | undefined;
    @ViewChild(GridComponent) private grid: GridComponent;


    constructor(private dataNamedService: DataNamedService,
        private formBuilder: FormBuilder,
        public editService: EditService,
        private productService: ProductService,
        private dialogService: DialogService,
        private renderer: Renderer2,
        private invoiceProduct: InvoiceProductService) {
    }

    ngOnInit() {
        this.invoiceProduct.getAll(this.invoice.id)
            .subscribe(
            result => this.gridProducts = result,
            error => console.log("Error :: " + error));

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

        this.renderer.listen(
            "document",
            "click",
            ({ target }) => {
                if (!isChildOf(target, "k-grid-content") && !isChildOf(target, "k-grid-toolbar")) {
                    this.saveClick();
                }
            });
    }

    onDateChange(value: any) {
        console.log(value);
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

    public addHandler({ sender }: any) {
        this.closeEditor(sender);
        this.formGroup = this.createFormGroup(new InvoiceProductModel());
        sender.addRow(this.formGroup);
        this.isNew = true;
    }

    public editHandler({ sender, rowIndex, dataItem }: EditEvent): void {
        if (this.formGroup && !this.formGroup.valid) {
            return;
        }

        if (!dataItem) return;

        this.saveRow();
        this.formGroup = this.createFormGroup(dataItem);
        this.editedRowIndex = rowIndex;
        sender.editRow(rowIndex, this.formGroup);
    }

    public editClick({ dataItem, rowIndex, isEdited }: any): void {
        if (!isEdited) {
            this.editHandler({
                dataItem: dataItem,
                rowIndex: rowIndex,
                sender: this.grid,
                isNew: this.isNew
            });
        }
    }

    handleValueChange(value: any) {
        if (value) {
            this.formGroup!.get('Product')!.setValue(value);
            this.formGroup!.get('CountryOfOrigin')!.setValue(value.countryOfOrigin.id);
            this.formGroup!.get('CodeNo')!.setValue(value.codeNo);
        }
    }

    public saveClick(): void {
        if (this.formGroup && !this.formGroup.valid) {
            return;
        }

        this.saveRow();
    }

    private saveRow(): void {
        if (this.isInEditingMode) {
            if (this.formGroup ) {
                this.formGroup.value.invoice = this.invoice;
                this.invoiceProduct.save(this.formGroup.value, this.isNew)
                    .subscribe(result => this.load());
                this.isNew = false;
            }
        }

        this.closeEditor(this.grid);
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
        this.invoiceProduct.remove(id)
            .subscribe(data => this.load());
    }

    load() {
        this.invoiceProduct.getAll(this.invoice.id)
            .subscribe(result => this.gridProducts = result);
    }

    public get isInEditingMode(): boolean {
        return this.editedRowIndex !== undefined || this.isNew;
    }

    public productHandleFilter(value: any) {
        this.productService.getBySeq(value)
            .subscribe(
            resultArray => this.products = resultArray,
            error => console.log("Error :: " + error));
    }

    private closeEditor(grid: GridComponent, rowIndex: number | undefined = this.editedRowIndex): void {
        if (this.formGroup != undefined) {
            this.isNew = false;
            grid.closeRow(rowIndex);
            this.editedRowIndex = undefined;
            this.formGroup = undefined;
        }
    }

    private createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'Position': new FormControl(dataItem.productPosition),
            'CountryOfOrigin': new FormControl(dataItem.product!.countryOfOrigin!.id),
            'CodeNo': new FormControl(dataItem!.product!.codeNo, Validators.required),
            'Product': new FormControl(dataItem!.product, Validators.required),
            'unit': new FormControl(dataItem!.product!.unit),
            'unitPrice': new FormControl(dataItem!.unitPrice),
            'quantity': new FormControl(dataItem!.quantity),
            'id': dataItem.id
        });
    }
}

