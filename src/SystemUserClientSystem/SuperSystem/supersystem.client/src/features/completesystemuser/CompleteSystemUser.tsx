import { useEffect } from 'react';
import './CompleteSystemUser.css';
import './../../tailwind.css';
import smartlogo from './../../assets/SmartCloudLogo.svg';

import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';

export const CompleteSystemUser = () => {


    useEffect(() => {
    }, []);

    return (
        <div>
            <div className="min-h-screen bg-gray-100">
                {/* Hero Section */}
                <header className="bg-smartcloud text-white">
                    <div className="container mx-auto px-4 py-12 overflow-auto">
                        <img src={smartlogo} alt="Smart Cloud Logo" className="w-auto mx-auto mb-2 float-left h-28"  />
                        <div className="float-right pt-16">
                            <button className="text-white px-4 py-2 rounded-lg hover:bg-blue-500 hover:text-white transition mr-2">Logg inn</button>
                            <button className="bg-white text-blue-600 px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Prøv gratis</button>
                        </div>
                    </div>
                </header>
                {/* Features Section */}
                <section id="features" className="bg-white ">
                    <div className="container mx-auto px-24 py-24">
                        <div className="mb-12 bg-smartcloudbright shadow-lg">
                            <div className="text-center">
                                <h2 className="text-3xl font-semibold mb-2 text-center p-4">Takk for at du valgte SmartBasic!</h2>
                                <p>For å komme i gang må du godkjenne en systemtiltang i Altinn, slik at SmartCloud kan vise og behandle dine data. </p>

                                <p>For å kunne godkjenne systemtilgang trenger du rollen Hovedadministrator, Tilgangstyrer eller Daglig leder.</p>

                                <a href="/complete" className="bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Kom i gang</a>

                                <br />
                            </div>
                            
                        </div>
                    </div>
                </section>

                {/* Call to Action Section */}
                <section className="bg-smartcloud text-white py-12">
                    <div className="container mx-auto px-4 text-center">
                    </div>
                </section>


                {/* Footer */}
                <footer className="bg-gray-800 text-white py-6">
                    <div className="container mx-auto px-4 text-center">
                        <p>&copy; {new Date().getFullYear()} Smart Cloud AS. All rights reserved.</p>
                    </div>
                </footer>
            </div>
        </div>
    );
}
