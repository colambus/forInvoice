import { Component, OnInit, ViewChild, Inject, ElementRef, Renderer2 } from '@angular/core';
import { DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { Validators, FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { EditService } from '../../../services/edit.service';
import { DialogAction } from '@progress/kendo-angular-dialog/dist/es2015/dialog/dialog-settings';
import { ProductModel } from '../../../models/product.model';
import { ProductService } from '../../../services/products.service';
import { NamedIdObject } from '../../../models/NamedIdObject.model';
import { DataNamedService } from '../../../services/dataNamed.service';
import { CerticateModel } from '../../../models/certificate.model';
import { CertificateService } from '../../../services/certificate.service';
import { EditEvent, GridComponent, PageChangeEvent } from '@progress/kendo-angular-grid';
import { FileRestrictions } from '@progress/kendo-angular-upload';
import { saveAs } from 'file-saver';

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

@Component({
    selector: 'product-list',
    templateUrl: './product-list.component.html',
    providers: [ProductService, EditService, CertificateService, DataNamedService]
})

export class ProductListComponent implements OnInit {
    private products: ProductModel[];
    private countries: NamedIdObject[];
    private certificates: CerticateModel[];
    public formGroup: FormGroup | undefined;
    private isNew = false;
    private product: ProductModel;
    private editedRowIndex: number | undefined;
    @ViewChild(GridComponent) private grid: GridComponent;
    importFromInvoiceValue: boolean;
    uploadSaveUrl = 'api/Product/Upload'; // should represent an actual API endpoint
    uploadRemoveUrl = 'removeUrl'; // should represent an actual API endpoint
    fileRestrictions: FileRestrictions = {
        allowedExtensions: ['xlsx']
    };

    public buttonCount = 5;
    public info = true;
    public type: 'numeric' | 'input' = 'numeric';
    public pageSizes = true;
    public previousNext = true;

    public pageSize = 10;
    public skip = 0;


    constructor(private productService: ProductService,
        private dialogService: DialogService,
        private formBuilder: FormBuilder,
        private editService: EditService,
        private certificateService: CertificateService,
        private renderer: Renderer2,
        private dataNamedService: DataNamedService) {
    }

    ngOnInit() {
        this.dataNamedService.getAll('Country')
            .subscribe(
            resultArray => this.countries = resultArray,
            error => console.log("Error :: " + error));

        this.certificateService.getAll()
            .subscribe(
            resultArray => this.certificates = resultArray,
            error => console.log("Error :: " + error));

        this.renderer.listen(
            "document",
            "click",
            ({ target }) => {
                if (!isChildOf(target, "k-grid-content") && !isChildOf(target, "k-grid-toolbar")) {
                    this.saveClick();
                }
            });

        this.load();
    }

    load() {
        this.productService.getAll(this.skip, this.pageSize).subscribe(
            resultArray => this.products = resultArray,
            error => console.log("Error :: " + error));
    }

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'codeNo': dataItem.codeNo,
            'descriptionEn': dataItem.descriptionEn,
            'descriptionUa': dataItem.descriptionUa,
            'certificate': dataItem.certificate,
            'countryOfOrigin': dataItem.countryOfOrigin,
            'id': dataItem.id
        });
    }

    public addHandler({ sender }: any) {
        this.closeEditor(sender);
        this.formGroup = this.createFormGroup(new ProductModel());
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

    public saveClick(): void {
        if (this.formGroup && !this.formGroup.valid) {
            return;
        }

        this.load();
        this.saveRow();
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

    public get isInEditingMode(): boolean {
        return this.editedRowIndex !== undefined || this.isNew;
    }

    private saveRow(): void {
        if (this.isInEditingMode) {
            if (this.formGroup) {
                this.productService.save(this.formGroup.value, this.isNew)
                    .subscribe(result => this.load());
                this.isNew = false;
            }
        }

        this.closeEditor(this.grid);
    }

    private closeEditor(grid: GridComponent, rowIndex: number | undefined = this.editedRowIndex): void {
        if (this.formGroup != undefined) {
            this.isNew = false;
            grid.closeRow(rowIndex);
            this.editedRowIndex = undefined;
            this.formGroup = undefined;
        }
    }

    protected pageChange({ skip, take }: PageChangeEvent): void {
        this.skip = skip;
        this.pageSize = take;
        this.load();
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
        this.productService.deleteItem(id)
            .subscribe(data =>
                this.load(),
            error => {
                if (error == 547)
                    alert("Item has foreign keys");
            }
            );
    }

    getProductTemplate() {
        let file: any;
        this.productService.getImportTemplate()
            .subscribe(res => {
                saveAs(res.data, res.filename);
            });
    }

    importCheckBoxChange() {
        if (this.importFromInvoiceValue)
            this.uploadSaveUrl = 'api/Product/UploadFromInvoice';
        else
            this.uploadSaveUrl = 'api/Product/Upload';
    }

    private loadFromFile() {

    }
}

