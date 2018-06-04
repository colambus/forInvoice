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
import { EditEvent, GridComponent } from '@progress/kendo-angular-grid';

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
    selector: 'certificate-list',
    templateUrl: './certificate-list.component.html',
    providers: [EditService, CertificateService, DataNamedService]
})

export class CertificateListComponent implements OnInit {
    private certificates: CerticateModel[];
    @ViewChild(GridComponent) private grid: GridComponent;
    public formGroup: FormGroup | undefined;
    private isNew = false;
    private editedRowIndex: number | undefined;

    constructor(private certificateService: CertificateService,
        private formBuilder: FormBuilder,
        private renderer: Renderer2) {
    }

    ngOnInit() {
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
        this.certificateService.getAll()
            .subscribe(result => this.certificates = result,
            error => console.log("Error :: " + error)
            )
    };

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'name': dataItem.name,
            'descriptionEn': dataItem.startDate,
            'descriptionUa': dataItem.endDate,
            'id': dataItem.id
        });
    }

    public addHandler({ sender }: any) {
        this.closeEditor(sender);
        this.formGroup = this.createFormGroup(new CerticateModel());
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
                this.certificateService.save(this.formGroup.value, this.isNew)
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

}


