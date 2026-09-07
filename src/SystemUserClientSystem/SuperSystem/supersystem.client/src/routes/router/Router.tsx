import { VendorLayout } from '../../features/vendor/VendorLayout';
import { SystemOverview } from '../../features/vendor/SystemOverview';
import { SystemsPage, SystemEditor } from '../../features/vendor/SystemEditor';
import { SystemUsers, SystemUserDetail } from '../../features/vendor/SystemUsers';
import { Requests, RequestDetail } from '../../features/vendor/Requests';
import { NewRequest } from '../../features/vendor/NewRequest';
import { ChangeRequest } from '../../features/vendor/ChangeRequest';
import { MetadataPage } from '../../features/vendor/AccessCatalogue';
import { createBrowserRouter } from 'react-router-dom';

import { LandingPage } from './../../features/landingpage/LandingPage';
import { SignUpBasic } from './../../features/signupbasic/SignUpBasic'; 
import { SignUpStandard } from './../../features/signupstandard/SignUpStandard';
import { CompleteSystemUser } from './../../features/completesystemuser/CompleteSystemUser';
import { Receipt } from './../../features/receipt/Receipt';
import { Dashboard } from './../../features/dashboard/Dashboard';
import { ClientAdmin } from './../../features/clientadmin/ClientAdmin';
import { Login } from './../../features/login/Login';

export const Router = createBrowserRouter(
    [
        {
            path: '/vendor',
            element: <VendorLayout />,
            children: [
                { index: true, element: <SystemOverview /> },
                { path: 'systems', element: <SystemsPage /> },
                { path: 'systems/new', element: <SystemEditor create /> },
                { path: 'settings', element: <SystemEditor /> },
                { path: 'systemusers', element: <SystemUsers /> },
                { path: 'systemusers/:id', element: <SystemUserDetail /> },
                { path: 'systemusers/:id/change', element: <ChangeRequest /> },
                { path: 'requests', element: <Requests /> },
                { path: 'requests/:kind/:id', element: <RequestDetail /> },
                { path: 'new', element: <NewRequest /> },
                { path: 'metadata', element: <MetadataPage /> },
            ],
        },
        {
            path: '/',
            element: <LandingPage />,
        },
        {
            path: '/signupbasic',
            element: <SignUpBasic />,
        },
        {
            path: '/signupstandard',
            element: <SignUpStandard />,
        },
        {
            path: '/complete',
            element: <CompleteSystemUser />,
        },
        {
            path: '/receipt',
            element: <Receipt />,
        },
        {
            path: '/dashboard',
            element: <Dashboard />,
        },
        {
            path: '/clientadmin',
            element: <ClientAdmin />,
        },
        {
            path: '/login',
            element: <Login />,
        }
    ],
    { basename: '/' },
);
