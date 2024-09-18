import { createBrowserRouter } from 'react-router-dom';

import { LandingPage } from './../../features/landingpage/LandingPage';
import { SignUpBasic } from './../../features/signupbasic/SignUpBasic'; 
import { CompleteSystemUser } from './../../features/completesystemuser/CompleteSystemUser';

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
        }
    ],
    { basename: '/' },
);