import { useEffect } from 'react';
import './Dashboard.css';
import './../../tailwind.css';
import smartlogo from './../../assets/SmartCloudLogo.svg';
import { Link } from 'react-router-dom';
import '@digdir/designsystemet-theme';
import '@digdir/designsystemet-css';
import { Table } from '@digdir/designsystemet-react';

export const Dashboard = () => {


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
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <h2 className="text-xl font-semibold mb-4">Dashboard for Import & Eksport AS</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-1 lg:grid-cols-1 gap-8">
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Aktive saker i Altinn</h3>
                                <Table zebra={true} className="w-full">
                                    <Table.Head>
                                        <Table.Row>
                                            <Table.HeaderCell>
                                                Tittel
                                            </Table.HeaderCell>
                                            <Table.HeaderCell>
                                                Avsender
                                            </Table.HeaderCell>
                                            <Table.HeaderCell>
                                                Status
                                            </Table.HeaderCell>
                                        </Table.Row>
                                    </Table.Head>
                                    <Table.Body>
                                        <Table.Row>
                                            <Table.Cell>
                                                Statistikk over arbeidsforhold
                                            </Table.Cell>
                                            <Table.Cell>
                                                SSB
                                            </Table.Cell>
                                            <Table.Cell>
                                                Ulest
                                            </Table.Cell>
                                        </Table.Row>
                                        <Table.Row>
                                            <Table.Cell>
                                               Reelle rettighetshavere
                                            </Table.Cell>
                                            <Table.Cell>
                                               Brønnøysundregistrene
                                            </Table.Cell>
                                            <Table.Cell>
                                                Under utfylling
                                            </Table.Cell>
                                        </Table.Row>
                                        <Table.Row>
                                            <Table.Cell>
                                                Melding fra kommunen
                                            </Table.Cell>
                                            <Table.Cell>
                                                Oslo Kommune
                                            </Table.Cell>
                                            <Table.Cell>
                                                Ulest
                                            </Table.Cell>
                                        </Table.Row>
                                    </Table.Body>
                                </Table>
                            </div>
                        </div>
                    </div>
                </section>
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <h3 className="text-xl font-semibold mb-4">SmartCloud Moduler</h3>
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-8">

                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Krav og betalinger</h3>
                                <ul>
                                    <li>Utestående krav: <b>kr 17.231</b></li>
                                    <li>Forfallende krav: <b>kr 12.231 (12.10.2024)</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Gå til oversikt</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Siste 7 dager</h3>
                                <ul>
                                    <li>Ordrer: <b>1337 (+3.5%)</b> </li>
                                    <li>Omsetning: <b>kr 2.922.134 (-2.2%)</b> </li>
                                    <li>Nye kunder: <b>666 (+5.2%) </b> </li>
                                    <li>Returnerende kunder: <b>582 (+1.1%)</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Gå til oversikt</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Forventet lagerankomst</h3>
                                <ul>
                                    <li>28.09.2024: <b>Siemens - 2 x 20ft</b> </li>
                                    <li>29.09.2024: <b>Temu - 3 x 40ft</b> </li>
                                    <li>02.10.2024: <b>Samsung - 1 x 20ft</b> </li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Gå til oversikt</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Sykefravær</h3>
                                <ul>
                                    <li>Denne uke: <b>3.8%</b></li>
                                    <li>Langtidsykemeldte: <b>2</b></li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Gå til oversikt</Link>
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

