import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: 'link-shortener',
        loadChildren: () => import('./modules/link-shortener/link-shortener-module').then(m => m.LinkShortenerModule)
    },
    {
        path: 'customers',
        loadChildren: () => import('./modules/customers/customers-module').then(m => m.CustomersModule)
    },
    {
        path: '',
        redirectTo: 'link-shortener',
        pathMatch: 'full'
    }
];
