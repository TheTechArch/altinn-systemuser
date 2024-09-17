import { useEffect, useState } from 'react';
import './App.css';
import './tailwind.css';
import imagelogo from './assets/illustration.jpg';
import smartlogo from './assets/SmartCloudLogo.svg';
import { Card } from '@digdir/designsystemet-react';

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

function App() {
    const [forecasts, setForecasts] = useState<Forecast[]>();

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecasts.map(forecast =>
                    <tr key={forecast.date}>
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;

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
                {/* About Section */} 
                <section id="about" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-8">
                            <div>
                                <img src={imagelogo} alt="Illustration" />
                            </div>
                            <div>
                                <h2 className="text-6xl font-semibold font-color-cloudblue py-4">Jobb smartere med SmartCloud</h2>
                                <p className="text-xl py-4">Med SmartCloud får du jobben gjort på dine egne premisser. Vårt system er fullt integrert med Altinn, noe som gjør at du kan fokusere på andre ting, mens dine egne tilpassede moduler gjør jobben.</p>
                            </div>
                        </div>
                    </div>
                </section>
                {/* Features Section */}
                <section id="features" className="py-12 bg-white">
                    <div className="container mx-auto px-4">
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
                           
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl">
                                <h3 className="text-xl font-semibold mb-4">SmartRegnskap</h3>
                                <p>Vår regnskapsmodul holder oversikt på inntekter og utgifter.
                                Aldri mer vil krav og betalinger komme som en overraskelse.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl">
                                <h3 className="text-xl font-semibold mb-4">SmartHR</h3>
                                <p>Med våre HR modul får du god oppfølging av dine ansatte. Den innebygde AI assistenten holder kontakten med ansatte som er syk og gir en rask oppfølging.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl">
                                <h3 className="text-xl font-semibold mb-4">SmartLønn</h3>
                                <p>Trenger du enkel modul som holder oversikt over lønnsutbetalingene til dine ansatte? Med Smart Lønn får du full oversikt over utbetalinger.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl">
                                <h3 className="text-xl font-semibold mb-4">SmartCRM</h3>
                                <p>Vår CRM modul gir deg full oversikt over dine kunder. Automatisk oppfølging via AI. Enkel import av kundedatabase fra andre CRM løsninger</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl">
                                <h3 className="text-xl font-semibold mb-4">SmartFleet</h3>
                                <p>Hold oversikt over din bedrifts bilpark. Når skal man på service? Trenger bilen lades? Vår
                                flåtemodul gir deg full oversikt.</p>
                            </div>
                            <div className="bg-smartcloudbright shadow-lg p-6 rounded-2xl">
                                <h3 className="text-xl font-semibold mb-4">SmartLogistikk</h3>
                                <p>Smart Logikstikkmodulen gjør deg til en vinner på markedet. Vår Logi AI støtte optimaliserer all transport av varer med fokus på pris og kvalitet.
                                Vår predektive AI sikrer at varene er fremme i akkurat samme øyeblikk som du innser du må bestille. </p>
                            </div>
                        </div>
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
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
            <Card>
                <Card.Header>Tittel</Card.Header>
                <Card.Content>Innhold</Card.Content>
                <Card.Footer>Valgfri fotnote</Card.Footer>
            </Card>
        </div>
    );

    async function populateWeatherData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        setForecasts(data);
    }
}

export default App;