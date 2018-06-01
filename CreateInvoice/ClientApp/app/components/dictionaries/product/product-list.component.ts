import { Component, OnInit } from '@angular/core';
import { DialogService, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { Validators, FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { EditService } from '../../../services/edit.service';
import { DialogAction } from '@progress/kendo-angular-dialog/dist/es2015/dialog/dialog-settings';
import { ProductModel } from '../../../models/product.model';
import { ProductService } from '../../../services/products.service';

@Component({
    selector: 'product-list',
    templateUrl: './product-list.component.html',
    providers: [ProductService, EditService]
})

export class ProductListComponent implements OnInit {
    private products: ProductModel[];

    constructor(private productService: ProductService,
        private dialogService: DialogService,
        private formBuilder: FormBuilder,
        private editService: EditService) {
    }

    ngOnInit() {
        this.load();
    }

    load() {
        this.productService.getAll().subscribe(
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
}

