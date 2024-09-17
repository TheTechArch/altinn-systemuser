import { useEffect } from 'react';
import './SignUpBasic.css';
import './../../tailwind.css';
import smartlogo from './../../assets/SmartCloudLogo.svg';
import { Textfield, Button } from '@digdir/designsystemet-react';

import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';

export const SignUpBasic = () => {


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
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4 text-center">
                    <Textfield
                        description=""
                        error=""
                        label="Firmnavn"
                        size="md"
                    />
                    <Textfield
                        description=""
                        error=""
                        label="Organisasjonsnummer"
                        size="md"
                    />

                    <Textfield
                    description=""
                        error=""
                        label="E-post"
                        size="md"
                        />


                        <Textfield
                            description=""
                            error=""
                            label="Kontaptperson"
                            size="md"
                        />

                        <Button
                            color="first"
                            size="md"
                            variant="primary"

                        >
                            Knapp
                        </Button>

                        </div>
                </section>

                {/* Call to Action Section */}
                <section className="bg-smartcloud text-white py-12">
                    <div className="container mx-auto px-4 text-center">
                        <h2 className="text-3xl font-bold mb-4">Kom i gang i dag</h2>
                        <p className="text-lg mb-6">Registrer deg i dag og få 3 mnd gratis tilgang</p>
                        <a
                            href="#"
                            className="bg-white text-blue-600 px-6 py-3 rounded-lg shadow-md hover:bg-blue-500 hover:text-white transition"
                        >
                            Registrer deg
                        </a>
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

