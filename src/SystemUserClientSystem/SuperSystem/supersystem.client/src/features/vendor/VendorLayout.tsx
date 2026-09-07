import { VendorContext } from './context';
import { Link, NavLink, Outlet, useSearchParams, useLocation } from 'react-router-dom';
import smartlogo from '../../assets/SmartCloudLogo.svg';
import { Configuration, RegisteredSystem, RegisteredSystemSummary, localized, query, useLoad } from './api';
import './Vendor.css';

export function Notice({ error, loading, retry }: { error?: string; loading?: boolean; retry?: () => void }) {
  return <>{loading && <p role="status" className="vendor-notice">Henter fra Altinn …</p>}
    {error && <div role="alert" className="vendor-error"><p>{error}</p>{retry && <button type="button" onClick={retry}>Prøv igjen</button>}</div>}</>;
}
export function JsonDetails({ value, label = 'Vis API-data' }: { value: unknown; label?: string }) {
  return <details className="vendor-json"><summary>{label}</summary><pre>{JSON.stringify(value, null, 2)}</pre></details>;
}
export function Status({ value, timedOut }: { value: string; timedOut?: boolean }) {
  const labels: Record<string, string> = { new: 'Venter på godkjenning', accepted: 'Godkjent', rejected: 'Avvist', denied: 'Avslått', deleted: 'Slettet', timedout: 'Utløpt', nochangeneeded: 'Ingen endring nødvendig' };
  const normalized = value.toLowerCase();
  return <span className={`vendor-status ${normalized === 'accepted' ? 'accepted' : ''}`}>{timedOut ? 'Utløpt' : labels[normalized] || value}</span>;
}

export function VendorLayout() {
  const config = useLoad<Configuration>('/api/vendor/configuration');
  const systems = useLoad<RegisteredSystemSummary[]>('/api/vendor/systems');
  const [params, setParams] = useSearchParams();
  const systemId = params.get('system') || (systems.data?.some(s => s.systemId === config.data?.defaultSystemId) ? config.data?.defaultSystemId : systems.data?.[0]?.systemId) || '';
  const system = useLoad<RegisteredSystem>(systemId ? `/api/vendor/systems/${encodeURIComponent(systemId)}` : null);
  const suffix = query(systemId);
  const location = useLocation();
  const systemManagement = location.pathname === "/vendor/systems" || location.pathname === "/vendor/systems/new";
  return <div className="vendor-app">
    <a className="vendor-skip" href="#vendor-main">Hopp til innhold</a>
    <header className="vendor-header"><Link to="/"><img src={smartlogo} alt="SmartCloud – forsiden" /></Link>
      <div><strong>Systembruker</strong><span>Referanseimplementasjon</span></div>
      <span className="vendor-environment">{config.data?.environment || 'Henter miljø …'}</span></header>
    <div className="vendor-workspace"><aside className="vendor-sidebar">
      <label htmlFor="vendor-system">Registrert system</label>
      <select id="vendor-system" value={systemId} onChange={e => setParams({ system: e.target.value })}>
        {!systems.data?.length && <option value="">Ingen systemer</option>}
        {systems.data?.map(s => <option key={s.systemId} value={s.systemId}>{localized(s.name) || s.systemId}</option>)}
      </select>
      <nav aria-label="Leverandør">
        <NavLink to="/vendor/systems">Alle systemer</NavLink>
        <NavLink end to={`/vendor${suffix}`}>Systemoversikt</NavLink>
        <NavLink to={`/vendor/systemusers${suffix}`}>Systembrukere</NavLink>
        <NavLink to={`/vendor/requests${suffix}`}>Forespørsler</NavLink>
        <NavLink to={`/vendor/settings${suffix}`}>Rediger system</NavLink>
        <NavLink to={`/vendor/new${suffix}`}>Ny forespørsel</NavLink>
        <NavLink to={`/vendor/metadata${suffix}`}>Tilgangspakker og tjenester</NavLink>
      </nav>
      <p>Data hentes fra Altinn i miljøet som er konfigurert på serveren.</p>
      <Link to="/dashboard">Til SmartCloud dashboard</Link>
    </aside>
    <main id="vendor-main" className="vendor-main">
      <Notice loading={config.loading || systems.loading || system.loading} error={config.error || systems.error || system.error}
        retry={() => { config.reload(); systems.reload(); system.reload(); }} />
      {!systemManagement && systems.data?.length === 0 && <section className="vendor-card"><h1>Ingen registrerte systemer</h1><p>Leverandøren har ingen systemer i dette miljøet. Registrer et system i systemregisteret først.</p><Link to="/vendor/systems/new">Registrer nytt system</Link></section>}
      {config.data && systems.data && (system.data || systemManagement) && <div key={systemManagement ? location.pathname : systemId}><Outlet context={{ configuration: config.data, system: system.data!, systemId, systems: systems.data, refreshSystems: systems.reload, refreshSystem: system.reload } satisfies VendorContext} /></div>}
    </main></div>
  </div>;
}
