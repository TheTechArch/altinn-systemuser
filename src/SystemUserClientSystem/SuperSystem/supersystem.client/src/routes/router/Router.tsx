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