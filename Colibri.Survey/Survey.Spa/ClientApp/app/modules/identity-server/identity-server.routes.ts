import { Routes, RouterModule } from '@angular/router';

/* component */ import { GroupComponent } from './containers/group/group.component';
/* component */ import { GroupManageComponent } from './containers/group-manage/group-manage.component';
/* component */ import { GroupOverviewComponent } from './containers/group-overview/group-overview.component';
/* component */ import { GroupOverviewMainComponent } from './containers/group-overview-main/group-overview-main.component';
/* component */ import { SystemConfigurationComponent } from './containers/system-configuration/system-configuration.component';
/* component */ import { MemberManageComponent } from './containers/member-manage/member-manage.component';
/* component */ import { UserComponent } from './containers/user/user.component';
/* component */ import { UserManageComponent } from './containers/user-manage/user-manage.component';
/* component */ import { UserProfileComponent } from './containers/user-profile/user-profile.component';

const routes: Routes = [
    {
        path: 'groups',
        component: GroupComponent,
        data: {
            breadcrumb: { title: 'Management of organizations', icon: '' }
        },
        children: [
            {
                path: '',
                component: GroupManageComponent,
            },
            {
                path: 'overview/:id',
                component: GroupOverviewComponent,
                data: {
                    breadcrumb: { title: 'Overview', icon: 'bowtie-group-rows' }
                },
                children: [
                    {
                        path: 'main',
                        component: GroupOverviewMainComponent
                    },
                    {
                        path: 'members',
                        component: MemberManageComponent
                    },
                ]
            },
        ]
    },
    {
        path: 'users',
        component: UserComponent,
        children: [
            {
                path: '',
                component: UserManageComponent
            },
        ]
    },
    {
        path: 'profile',
        component: UserProfileComponent
    },
    {
        path: 'system',
        component: SystemConfigurationComponent
    }
];

export const IdentityServerRoutingModule = RouterModule.forChild(routes);