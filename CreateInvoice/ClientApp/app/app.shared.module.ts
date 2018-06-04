import { NgModule, LOCALE_ID, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

//Components
import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';
import { InvoiceComponent } from './components/invoice/invoice.component';
import { InvoiceListComponent } from './components/invoice/invoiceList/invoiceList.component';
import { ProductListComponent } from './components/dictionaries/product/product-list.component';
import { CertificateListComponent } from './components/dictionaries/certificate/certificate-list.component';
import { CountryListComponent } from './components/dictionaries/country/countries-list.component';

//Directives

//Kendo UI modules
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { WindowModule } from '@progress/kendo-angular-dialog';
import { GridModule } from '@progress/kendo-angular-grid';
import { DialogsModule } from '@progress/kendo-angular-dialog';

import '@progress/kendo-angular-intl/locales/uk/all';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        InvoiceComponent,
        InvoiceListComponent,
        ProductListComponent,
        CertificateListComponent,
        CountryListComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        DateInputsModule,
        BrowserAnimationsModule,
        ButtonsModule,
        GridModule,
        DialogsModule,
        DropDownsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'invoiceList', component: InvoiceListComponent },
            { path: 'product-list', component: ProductListComponent },
            { path: 'certificate-list', component: CertificateListComponent },
            { path: 'countries-list', component: CountryListComponent },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [
        { provide: LOCALE_ID, useValue: 'uk-UK'}
    ]
})
export class AppModuleShared {
}
