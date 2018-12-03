import { Component, Input, Output, EventEmitter, ViewChild , OnInit } from '@angular/core';
import { TreeDragDropService } from 'primeng/components/common/api';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/components/common/messageservice';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute } from '@angular/router';

/* service-api */ import { GroupMembersApiService } from 'shared/services/api/group-members.api.service';
/* model-api */ import { SearchQueryApiModel, SearchQueryPage } from 'shared/models/entities/api/page-search-entry.api.model';
/* model-control */ import { DialogDataModel } from 'shared/models/controls/dialog-data.model';

@Component({
    selector: 'cmp-member-grid',
    templateUrl: './member-grid.component.html',
    styleUrls: ['./member-grid.component.scss'],
    providers: [
        TreeDragDropService,
        ConfirmationService,
        MessageService
    ]
})

export class MemberGridComponent implements OnInit {
    @ViewChild('dtGroups') dtGroups: any;
    // output events
    @Output() deleteItem = new EventEmitter<any>();
    @Output() editItem = new EventEmitter<any>();
    // input events
    @Input() eventResetData: Observable<any>;
    // private subscriberResetData: any;


    dialogCreateConfig: DialogDataModel<any>;

    groupId: any;
    // table
    tbItems: any[] = [];
    tbCols: any[] = [];
    tbLoading = true;
    tbTotalItemCount: number;
    tbSelectedColumns: any[];
    // option
    optionTbToggle: any = {
        columns: [
            { field: 'userName', header: 'Member name', width: 320 },
            { field: 'email', header: 'Email', width: 150 },
            { field: 'emailConfirmed', header: 'Is activate', width: null }
        ],
        filter: false
    };

    constructor(
        private groupMembersApiService: GroupMembersApiService,
        private route: ActivatedRoute,
    ) {
        this.tbSelectedColumns = this.optionTbToggle.columns;
        this.tbLoading = true;
        this.route.parent.params.subscribe((params: any) => {
            this.groupId = params['id'] ? params['id'] : null;
        });
    }

    ngOnInit() {
        // this.subscriberResetData = this.eventResetData.subscribe(() => this.dtGroups.reset());
    }
    ngOnDestroy() {
        // this.subscriberResetData.unsubscribe();
    }

    item_edit(groupId: string) { this.editItem.emit(groupId); }
    item_delete(groupId: string) { this.deleteItem.emit(groupId); }

    tb_loadItems(event: any) {
        this.tbLoading = true;
        const searchEntry = {
            searchQueryPage: {
                pageNumber: event.first,
                pageLength: event.rows
            } as SearchQueryPage,
            orderStatement: (event.sortField && event.sortOrder)
                ? { columName: event.sortField, reverse: event.sortOrder > 0 }
                : null
        } as SearchQueryApiModel;
        this._requestGetRootGroups(searchEntry);
    }

    _requestGetRootGroups(searchEntry: SearchQueryApiModel) {
        this.tbLoading = true;
        this.groupMembersApiService.getByGroup(this.groupId, searchEntry).subscribe((response: any) => {
            this.tbLoading = false;
            this.tbItems = response.itemList;
            this.tbTotalItemCount = response.totalItemCount;
        });
    }

    public dialogCreateOpen() { this.dialogCreateConfig = new DialogDataModel<any>(true); }
    // public dialogCreateOnChange() { this.eventResetData.next(); }
    public dialogCreateOnChange() {  }
    public dialogCreateOnCancel() { console.log('dialogGroupCreateOnCancel'); }
    public dialogCreateOnHide() { console.log('dialogGroupCreateOnHide'); }
}