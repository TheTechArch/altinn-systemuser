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
                        <img src={smartlogo} alt="Smart Cloud Logo" className="w-auto mx-auto mb-2 float-left h-28"  />
                        <div className="float-right pt-16">
                            <button className="text-white px-4 py-2 rounded-lg hover:bg-blue-500 hover:text-white transition mr-2">Logg inn</button>
                            <button className="bg-white text-blue-600 px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Prøv gratis</button>
                        </div>
                    </div>
                </header>
                {/* Features Section */}
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <div className="grid grid-cols-1 sm:grid-cols-1 lg:grid-cols-1 gap-8">
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Aktive saker i Altinn</h3>
                                <Table zebra="True">
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
                                                Statistikk over arbeidforhjold
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
                                                Cell 1
                                            </Table.Cell>
                                            <Table.Cell>
                                                Cell 2
                                            </Table.Cell>
                                            <Table.Cell>
                                                Cell 3
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
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-8">

                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Krav og betalinger</h3>
                                <ul>
                                    <li>Utestående krav: 17.231,-</li>
                                    <li>Forfallende krav: 12.231,- (12.10.2024)</li>
                                </ul>
                                <br />
                                <Link to="/signupbasic" className="text-white bg-smartcloudbluelight px-4 py-2 rounded-3xl shadow-md hover:bg-blue-500 hover:text-white transition">Gå til oversikt</Link>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl font-color-cloudblue">
                                <h3 className="text-xl font-semibold mb-4">Siste mnd</h3>
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

