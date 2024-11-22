import { useEffect } from 'react';
import './LandingPage.css';
import './../../tailwind.css';
import imagelogo from './../../assets/illustration.jpg';
import smartlogo from './../../assets/SmartCloudLogo.svg';
import { Link } from 'react-router-dom';
import { CheckmarkIcon } from '@navikt/aksel-icons';

import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';

export const LandingPage = () => {


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
                            <Link to="/login" className="text-white px-4 py-2 rounded-lg hover:bg-blue-500 hover:text-white transition mr-2">Logg inn</Link> {/* Use the Link component with the "to" prop set to "/signupbasic" */}
                            <Link to="/signupbasic" className="bg-white text-blue-600 px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Prøv gratis</Link> {/* Use the Link component with the "to" prop set to "/signupbasic" */}
                        </div>
                    </div>
                </header>
                {/* About Section */} 
                <section id="about" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-8">
                            <div>
                                <img src={imagelogo} alt="Illustration" />
                            </div>
                            <div>
                                <h2 className="text-6xl font-semibold font-color-cloudblue py-4">Jobb smartere med SmartCloud</h2>
                                <p className="text-xl py-4 font-color-cloudblue">Med SmartCloud får du jobben gjort på dine egne premisser. Vårt system er fullt integrert med Altinn, noe som gjør at du kan fokusere på andre ting, mens dine egne tilpassede moduler gjør jobben.</p>
                            </div>
                        </div>
                    </div>
                </section>
                {/* Features Section */}
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
 
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
                           
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">SmartRegnskap</h3>
                                <p>Vår regnskapsmodul holder oversikt på inntekter og utgifter.
                                Aldri mer vil krav og betalinger komme som en overraskelse.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">SmartHR</h3>
                                <p>Med våre HR modul får du god oppfølging av dine ansatte. Den innebygde AI assistenten holder kontakten med ansatte som er syk og gir en rask oppfølging.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">SmartLønn</h3>
                                <p>Trenger du enkel modul som holder oversikt over lønnsutbetalingene til dine ansatte? Med Smart Lønn får du full oversikt over utbetalinger.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">SmartCRM</h3>
                                <p>Vår CRM modul gir deg full oversikt over dine kunder. Automatisk oppfølging via AI. Enkel import av kundedatabase fra andre CRM løsninger</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4 font-color-cloudblue">SmartFleet</h3>
                                <p>Hold oversikt over din bedrifts bilpark. Når skal man på service? Trenger bilen lades? Vår
                                flåtemodul gir deg full oversikt.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4 font-color-cloudblue">SmartLogistikk</h3>
                                <p>Smart Logikstikkmodulen gjør deg til en vinner på markedet. Vår Logi AI støtte optimaliserer all transport av varer med fokus på pris og kvalitet.
                                Vår predektive AI sikrer at varene er fremme i akkurat samme øyeblikk som du innser du må bestille. </p>
                            </div>
                        </div>
                    </div>
                </section>

                {/* Call to Action Section */}
                <section className="bg-smartcloud text-white py-12">
                    <div className="container mx-auto px-4">
                        <h2 className="text-3xl font-semibold mb-2 text-center">Våre pakker</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-4 lg:grid-cols-4 gap-8">
                            <div className="p-2">
                                <h3 className="text-xl font-semibold mb-4">SmartBasic</h3>
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartRegnskap<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> Systemtilgang i Altinn<br />
                                <br />
                                <br />
                                <br />
                               <b>Gratis i 3 mnd</b><br /><br />
                                <Link to="/signupbasic" className="bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Registrer deg</Link>
                            </div>
                            <div className="p-2">
                                <h3 className="text-xl font-semibold mb-4">SmartStandard</h3>
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartRegnskap<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartLønn<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> Systemtilgang i Altinn<br />
                                <br />
                                <br />
                                <b>kr 609 pr. mnd</b><br /><br />
                                <Link to="/signupbasic" className="bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Registrer deg</Link>
                            </div>
                            <div className="p-2">
                                <h3 className="text-xl font-semibold mb-4">SmartPluss</h3>
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartRegnskap<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartLønn<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartHR<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> Systemtilgang i Altinn<br />
                                <br />
                                <b>kr 930 pr. mnd</b><br /><br />
                                <Link to="/signupbasic" className="bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Registrer deg</Link>
                            </div>
                            <div className="p-2">
                                <h3 className="text-xl font-semibold mb-4">SmartPremium</h3>
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartRegnskap og SmartHR<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartCRM<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> SmartFleet og SmartLogistikk<br />
                                <CheckmarkIcon title="a11y-title" className="inline w-6 h-6 font-color-cloudbluelight" fontSize="1.5rem" /> Systemtilgang i Altinn <br />
                                <br />
                                <b>kr 1.409 pr. mnd</b><br /><br />
                                <Link to="/signupbasic" className="bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Registrer deg</Link>
                                <br />
                            </div>
                        </div>
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

