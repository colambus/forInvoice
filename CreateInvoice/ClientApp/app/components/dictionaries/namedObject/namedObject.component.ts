import { Component, OnInit, Inject } from '@angular/core';
import { DataNamedService } from '../../../services/dataNamed.service';
import { NamedIdObject } from '../../../models/NamedIdObject.model';

@Component({
    selector: 'namedObject',
    templateUrl: './namedObject.component.html',
    providers: [DataNamedService]
})

export class NamedObjectComponent implements OnInit {
    namedObject: NamedIdObject;

    ngOnInit() {

    }
}