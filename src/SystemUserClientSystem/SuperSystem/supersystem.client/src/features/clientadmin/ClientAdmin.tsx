import { useEffect } from 'react';
import './ClientAdmin.css';
import './../../tailwind.css';
import smartlogo from './../../assets/SmartCloudLogo.svg';
import { Link } from 'react-router-dom';
import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';

export const ClientAdmin = () => {


    useEffect(() => {
    }, []);

    return (
        <div>
            <div className="min-h-screen bg-gray-100">
                {/* Hero Section */}
                <header className="bg-smartcloud text-white">
                    <div className="container mx-auto px-4 py-12 overflow-auto">
                        <a href="/"><img src={smartlogo} alt="Smart Cloud Logo" className="w-auto mx-auto mb-2 inline h-28" /></a>
                       
                    </div>
                </header>
                {/* Features Section */}
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <h3 className="text-xl font-semibold mb-4">Kunder</h3>
                        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-8">
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">TallMoro</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Skattetango</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">KredittKongen</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">BudsjettBobla</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Krav og betalinger, MVA</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Kontoklovnene</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">RaskRentesnurr</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">MorsomMoms</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Fiksefaktura</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">LatterligLønn</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">RegneRakkerne</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">RaskRentesnurr</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">RaskRentesnurr</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">RaskRentesnurr</h3>
                                <ul>
                                    <li>Kundetype: <b>Regnskap</b></li>
                                    <li>Tilganger: <b>Regnskapsfører lønn, Regnskapsfører uten signeringsrett</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Se tilganger</Link>
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

