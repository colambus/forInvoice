import { Component, OnInit, ViewChild } from '@angular/core';
import { Data } from '@angular/router/src/config';
import { getToday } from '@progress/kendo-angular-dateinputs/dist/es2015/util';
import { InvoiceModel } from '../../models/invoice.model';
import { NamedIdObject } from '../../models/NamedIdObject.model';
import { Organization } from '../../models/organization.model';
import { DataNamedService } from '../../services/dataNamed.service';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { EditService } from '../../services/edit.service';
import { InvoiceProductModel } from '../../models/invoiceProduct.model';
import { ProductModel } from '../../models/product.model';
import { Observable } from 'rxjs/Observable';
import { GridDataResult, GridComponent, EditEvent } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { map } from 'rxjs/operators/map';
import { ProductService } from '../../services/products.service';

@Component({
    selector: 'invoice',
    templateUrl: './invoice.component.html',
    providers: [DataNamedService, EditService, ProductService]
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
        private productService: ProductService) {
    }

    ngOnInit() {
        this.gridProducts = new Array<InvoiceProductModel>();

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

    public get isInEditingMode(): boolean {
        return this.editedRowIndex !== undefined || this.isNew;
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

    public saveHandler({ sender, formGroup, rowIndex }: any) {
        if (formGroup.valid) {
            this.editService.create(formGroup.value);
            sender.closeRow(rowIndex);
        }
    }

    public cellCloseHandler(args: any) {
        const { formGroup, dataItem } = args;

        if (!formGroup.valid) {
            // prevent closing the edited cell if there are invalid values.
            args.preventDefault();
        } else if (formGroup.dirty) {
            this.editService.assignValues(dataItem, formGroup.value);
            this.editService.update(dataItem);
        }
    }

    public addHandler({ sender }: any) {
        this.closeEditor(sender);
        sender.addRow(this.createFormGroup(new InvoiceProductModel()));
        this.isNew = true;
    }

    public editHandler({ sender, rowIndex, dataItem }: EditEvent): void {
        if (this.formGroup && !this.formGroup.valid) {
            return;
        }


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

    private saveRow(): void {
        
    }

    private closeEditor(grid: GridComponent, rowIndex: number | undefined = this.editedRowIndex): void {
        this.isNew = false;
        grid.closeRow(rowIndex);
        this.editedRowIndex = undefined;
        this.formGroup = undefined;
    }

    public cellClickHandler({ sender, rowIndex, columnIndex, dataItem, isEdited }: any) {
        if (!isEdited) {
            sender.editCell(rowIndex, columnIndex, this.createFormGroup(dataItem));
        }
    }

    public productHandleFilter(value: any) {
        this.productService.getBySeq(value)
            .subscribe(
            resultArray => this.products = resultArray,
            error => console.log("Error :: " + error));
    }

    public productSelectionChange(value: ProductModel) {
        console.log(value);
        let newItem = new InvoiceProductModel();
        newItem.product = value;
        this.gridProducts.push(newItem);
    }

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'Position': dataItem.productPosition,
            'CountryOfOrigin': dataItem.product.countryOfOrigin.id,
            'CodeNo': [dataItem.product.codeNo],
            'Description': [dataItem.product.descriptionEn],
            'Unit': [dataItem.product.unit],
            'UnitPrice': dataItem.unitPrice,
            'Quantity': [dataItem.quantity],
            'Amount': dataItem.unitPrice * dataItem.quantity
        });
    }
}

