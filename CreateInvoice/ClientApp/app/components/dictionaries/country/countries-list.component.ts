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
import { CountryOfOriginModel } from '../../../models/countryOfOrigin.model';
import { CountryService } from '../../../services/country.service';

import { EditEvent, GridComponent } from '@progress/kendo-angular-grid';
import { FileRestrictions, UploadEvent, RemoveEvent } from '@progress/kendo-angular-upload';
import { GroupDescriptor, DataResult, process } from '@progress/kendo-data-query';

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
    selector: 'countries-list',
    templateUrl: './countries-list.component.html',
    providers: [CountryService, EditService, CertificateService, DataNamedService]
})

export class CountryListComponent implements OnInit {
    private countries: CountryOfOriginModel[];
    private certificates: CerticateModel[];
    public formGroup: FormGroup | undefined;
    private isNew = false;
    private country: CountryOfOriginModel;
    private editedRowIndex: number | undefined;
    @ViewChild(GridComponent) private grid: GridComponent;
    uploadSaveUrl = 'api/Country/Upload'; // should represent an actual API endpoint
    uploadRemoveUrl = 'removeUrl'; // should represent an actual API endpoint
    fileRestrictions: FileRestrictions = {
        allowedExtensions: ['xlsx']
    };
    public groups: GroupDescriptor[] = [];
    public gridView: DataResult;

    constructor(private countryService: CountryService,
        private dialogService: DialogService,
        private formBuilder: FormBuilder,
        private editService: EditService,
        private certificateService: CertificateService,
        private renderer: Renderer2,
        private dataNamedService: DataNamedService) {
    }

    ngOnInit() {
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
        this.countryService.getAll().subscribe(
            resultArray => {
                this.countries = resultArray,
                this.gridView = process(this.countries, { group: this.groups });
            },
            error => console.log("Error :: " + error));     
    }

    public createFormGroup(dataItem: any): FormGroup {
        return this.formBuilder.group({
            'name': [dataItem.name, Validators.required],
            'descriptionEn': [dataItem.descriptionEn, Validators.required],
            'descriptionUa': [dataItem.descriptionUa],
            'certificateId': [dataItem.certificateId, [Validators.required]]
        });
    }

    public addHandler({ sender }: any) {
        this.closeEditor(sender);
        this.formGroup = this.createFormGroup(new CountryOfOriginModel());
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
            if (this.formGroup != undefined) {
                this.country = new CountryOfOriginModel
                this.countryService.save(this.formGroup.value, this.isNew)
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
                    this.onDeleteData(dataItem.countryId);
                else
                    console.log('Action close');
                ;
            }
        });
    }

    onDeleteData(id: number) {
        this.countryService.deleteItem(id)
            .subscribe(data =>
                this.load(),
            error => {
                if (error == 547)
                    alert("Item has foreign keys");
            }
            );
    }

    handleValueChange(value: any) {
        if (value) {
            this.formGroup!.get('certificate')!.setValue(value);
        }
    }

    public certificate(id: number): any {
        if (this.certificates)
            return this.certificates.find(x => x.id === id);
        return "";
    }

    uploadEventHandler(e: UploadEvent) {
        e.data = {
            description: 'File upload'
        };
        this.load();
    }

    removeEventHandler(e: RemoveEvent) {
        e.data = {
            description: 'File remove'
        };
    }

    getCountryTemplate() {
        let file: any;
        this.countryService.getImportTemplate()
            .subscribe(res => {
                saveAs(res.data, res.filename);
            });
    }

    public groupChange(groups: GroupDescriptor[]): void {
        this.groups = groups;
        this.load();
    }
}

