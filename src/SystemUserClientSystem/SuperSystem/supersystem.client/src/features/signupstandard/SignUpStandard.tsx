import { useEffect } from 'react';
import './SignUpStandard.css';
import './../../tailwind.css';
import smartlogo from './../../assets/SmartCloudLogo.svg';
import { Textfield, Checkbox } from '@digdir/designsystemet-react';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';
import { CheckmarkIcon } from '@navikt/aksel-icons';

export const SignUpStandard = () => {
    const [firmaNavn, setFirmaNavn] = useState<string | undefined>(undefined);
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
                                <h2 className="text-3xl font-semibold mb-2 text-center p-4">Du har valgt pakken SmartBasic</h2>
                                <p>Med SmartBasic får du tilgang til SmartRegnkap med alt du trenger for å holde orden på regnskapet</p>
                            </div>
                            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-8 ">
                                <div className="container mx-auto px-4 py-4">
                                    <Textfield
                                        description=""
                                        error=""
                                        label="Firmanavn"
                                        size="md"
                                        value={firmaNavn}
                                        onBlur={(e) => setFirmaNavn(e.target.value)}
                                    />
                                    <Textfield
                                        description=""
                                        error=""
                                        label="Organisasjonsnummer"
                                        size="md"
                                        value={organisasjonsNr}
                                        onBlur={(e) => setOrganisasjonsNr(e.target.value)}
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
                                        label="Kontaktperson"
                                        size="md"
                                    />
                                    <Checkbox
                                        description="Les vilkårene her"
                                        size="md"
                                        value="value"
                                    >
                                       Jeg godtar betingelsene til SmartCloud AS
                                    </Checkbox>
                                    <br />

                                    <Link to={`/complete?organisajonsnr=${organisasjonsNr}&product=standard`} className="bg-smartcloudbluelight px-4 py-2 rounded-3xl text-white shadow-md hover:bg-blue-500 hover:text-white transition">Registrer deg</Link>
                                    <br /><br />
                                </div>
                                <div>
                                    <h3 className="text-xl font-semibold mb-4">SmartStandard</h3>
                                    <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartRegnskap<br />
                                    <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> Systemtilgang i Altinn<br />
                                    <br />
                                    Prøv gratis i tre måneder!
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

