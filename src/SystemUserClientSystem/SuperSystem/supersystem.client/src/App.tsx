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
                        <h1 className="text-4xl font-bold mb-4">Welcome to My Product</h1>
                        <p className="text-xl mb-6">A simple product that solves complex problems.</p>
                        <a
                            href="#features"
                            className="bg-white text-blue-600 px-6 py-3 rounded-lg shadow-md hover:bg-blue-500 hover:text-white transition"
                        >
                            Learn More
                        </a>
                    </div>
                </header>

                {/* Features Section */}
                <section id="features" className="py-12">
                    <div className="container mx-auto px-4">
                        <h2 className="text-3xl font-bold text-center mb-8">Features</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Feature One</h3>
                                <p>Explanation of what makes Feature One amazing and how it solves problems.</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Feature Two</h3>
                                <p>Explanation of what makes Feature Two amazing and how it solves problems.</p>
                            </div>
                            <div className="bg-white shadow-lg p-6 rounded-lg">
                                <h3 className="text-xl font-semibold mb-4">Feature Three</h3>
                                <p>Explanation of what makes Feature Three amazing and how it solves problems.</p>
                            </div>
                        </div>
                    </div>
                </section>

                {/* Call to Action Section */}
                <section className="bg-blue-600 text-white py-12">
                    <div className="container mx-auto px-4 text-center">
                        <h2 className="text-3xl font-bold mb-4">Ready to Get Started?</h2>
                        <p className="text-lg mb-6">Join us today and experience the difference.</p>
                        <a
                            href="#"
                            className="bg-white text-blue-600 px-6 py-3 rounded-lg shadow-md hover:bg-blue-500 hover:text-white transition"
                        >
                            Sign Up Now
                        </a>
                    </div>
                </section>

                {/* Footer */}
                <footer className="bg-gray-800 text-white py-6">
                    <div className="container mx-auto px-4 text-center">
                        <p>&copy; {new Date().getFullYear()} My Product. All rights reserved.</p>
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