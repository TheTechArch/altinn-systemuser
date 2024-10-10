import { createBrowserRouter } from 'react-router-dom';

import { LandingPage } from './../../features/landingpage/LandingPage';
import { SignUpBasic } from './../../features/signupbasic/SignUpBasic'; 
import { CompleteSystemUser } from './../../features/completesystemuser/CompleteSystemUser';
import { Receipt } from './../../features/receipt/Receipt';
import { Dashboard } from './../../features/dashboard/Dashboard';
import { ClientAdmin } from './../../features/clientadmin/ClientAdmin';

export const Router = createBrowserRouter(
    [
        {
            path: '/',
            element: <LandingPage />,
        },
        {
            path: '/signupbasic',
            element: <SignUpBasic />,
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
        }
    ],
    { basename: '/' },
);