import { useEffect } from 'react';
import './Login.css';
import './../../tailwind.css';
import smartlogo from './../../assets/SmartCloudLogo.svg';
import { Textfield } from '@digdir/designsystemet-react';
import { useState } from 'react';
import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';

export const Login = () => {
     const [organisasjonsNr, setOrganisasjonsNr] = useState<string | undefined>(undefined);

    useEffect(() => {
    }, []);

    return (
        <div>
            <div className="min-h-screen bg-gray-100">
                {/* Hero Section */}
                <header className="bg-smartcloud text-white">
                    <div className="container mx-auto px-4 py-12 overflow-auto">
                        <a href="/"><img src={smartlogo} alt="Smart Cloud Logo" className="w-auto mx-auto mb-2 inline h-28" /></a>
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
                                <h2 className="text-3xl font-semibold mb-2 text-center p-4">Logg inn i SmartCloud</h2>
                                <p>Bruk organisasjonsnr for virksomheten</p>
                            </div>
                            <div className="grid grid-cols-1 sm:grid-cols-1 lg:grid-cols-1 gap-8 ">
                                <div className="container mx-auto px-4 py-4">
                                    <Textfield
                                        description=""
                                        error=""
                                        label="Organisasjonsnummer"
                                        size="md"
                                        value={organisasjonsNr}
                                        onBlur={(e) => setOrganisasjonsNr(e.target.value)}
                                    />
                                   <br />
                                    <a href={`/Authenticate?systemUserOrg=${organisasjonsNr}`} className="bg-smartcloudbluelight px-4 py-2 rounded-3xl text-white shadow-md hover:bg-blue-500 hover:text-white transition">Logg inn</a>
                                    <br /><br />
                                </div>
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
                        <p>&copy; {new Date().getFullYear()} SmartCloud AS. All rights reserved.</p>
                    </div>
                </footer>
            </div>
        </div>
    );
}

