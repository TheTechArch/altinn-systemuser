import { useEffect, useState } from 'react';
import './App.css';
import './tailwind.css';
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
        <div className="bg-amber-400">
            <h1 id="tableLabel">Weather forecast</h1>
            <div className="min-h-screen bg-gray-100">
                {/* Hero Section */}
                <header className="bg-blue-600 text-white">
                    <div className="container mx-auto px-4 py-12 text-center">
                        <h1 className="text-4xl font-bold mb-4">Smart Cloud AS</h1>
                        <p className="text-xl mb-6">Norges ledende programvarepakke for foretningstøtte</p>
                        <a
                            href="#features"
                            className="bg-white text-blue-600 px-6 py-3 rounded-lg shadow-md hover:bg-blue-500 hover:text-white transition"
                        >
                            Lær mer
                        </a>
                    </div>
                </header>

                {/* Features Section */}
                <section id="features" className="py-12">
                    <div className="container mx-auto px-4">
                        <h2 className="text-3xl font-bold text-center mb-8">Funksjonalitet</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Full Altinn støtte </h3>
                                <p>Vårt system er fullintegrerert med Altinn og med bruk av systembruker for virksomheter kan du fokusere på andre ting, mens våre system gjør jobben.</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Smart Regnskap</h3>
                                <p>Vår regnskapsmodul holder oversikt på inntekter og utgifter.
                                Aldri mer vil krav og betalinger komme som en overraskelse.</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Smart HR</h3>
                                <p>Med våre HR modul får du god oppfølging av dine ansatte. Den innebygde AI assistenten holder kontakten med ansatte som er syk og gir en rask oppfølging.</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Smart CRM</h3>
                                <p>Vår CRM modul gir deg full oversikt over dine kunder. Automatisk oppfølging via AI. Enkel import av kundedatabase fra andre CRM løsninger</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Smart Fleet</h3>
                                <p>Hold oversikt over din bedrifts bilpark. Når skal man på service? Trenger bilen lades? Vår
                                flåtemodul gir deg full oversikt.</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Smart Logistikk</h3>
                                <p>Smart Logikstikkmodulen gjør deg til en vinner på markedet. Vår Logi AI støtte optimaliserer all transport av varer med fokus på pris og kvalitet.
                                Vår predektive AI sikrer at varene er fremme i akkurat samme øyeblikk som du innser du må bestille. </p>
                            </div>
                        </div>
                    </div>
                </section>

                {/* Call to Action Section */}
                <section className="bg-blue-600 text-white py-12">
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