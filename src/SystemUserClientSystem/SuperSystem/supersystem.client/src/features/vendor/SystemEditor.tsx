import { useVendor } from './context';
import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AccessPackage, Right, api, localized, query } from './api';
import { JsonDetails, Notice } from './VendorLayout';
import { AccessCatalogue, SelectedAccess } from './AccessCatalogue';

export function SystemsPage() {
  const { systems, refreshSystems, configuration } = useVendor();
  return <><div className="vendor-title"><div><p className="vendor-eyebrow">{configuration.environment}</p><h1>Leverandørens systemer</h1><p>Systemene som virksomhetens Maskinporten-integrasjon har tilgang til.</p></div>
    <Link className="vendor-button" to="/vendor/systems/new">Registrer nytt system</Link></div>
    <button onClick={refreshSystems}>Oppdater systemlisten</button>
    <div className="vendor-system-grid">{systems.map(s => <section className="vendor-card" key={s.systemId}><h2>{localized(s.name) || s.systemId}</h2><p>{localized(s.description)}</p><code>{s.systemId}</code>
      <div className="vendor-actions"><Link to={`/vendor${query(s.systemId)}`}>Åpne system</Link><Link to={`/vendor/settings${query(s.systemId)}`}>Rediger system og tilganger</Link></div></section>)}</div>
    {!systems.length && <p>Ingen systemer er registrert ennå. Opprett virksomhetens første system.</p>}
  </>;
}

export function SystemEditor({ create = false }: { create?: boolean }) {
  const { system, configuration, refreshSystems, refreshSystem } = useVendor();
  const navigate = useNavigate();
  const org = (system?.vendor?.ID || system?.vendor?.id || configuration.defaultSystemId.split('_')[0]).replace(/^0192:/, '');
  const [vendorOrg, setVendorOrg] = useState(org);
  const [id, setId] = useState(create ? org + '_' : system.id);
  const [names, setNames] = useState<Record<string, string>>(create ? { nb: '' } : system.name);
  const [descriptions, setDescriptions] = useState<Record<string, string>>(create ? { nb: '' } : system.description);
  const [clients, setClients] = useState(create ? '' : (system.clientId || []).join('\n'));
  const [redirects, setRedirects] = useState(create ? '' : (system.allowedRedirectUrls || []).join('\n'));
  const [visible, setVisible] = useState(create ? true : system.isVisible);
  const [rights, setRights] = useState<Right[]>(create ? [] : system.rights || []);
  const [packages, setPackages] = useState<AccessPackage[]>(create ? [] : system.accessPackages || []);
  const [rawRights, setRawRights] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const split = (value: string) => [...new Set(value.split(/\r?\n/).map(v => v.trim()).filter(Boolean))];
  const body = { id: id.trim(), vendor: { ID: `0192:${vendorOrg}` }, name: names, description: descriptions, rights, accessPackages: packages,
    clientId: split(clients), allowedRedirectUrls: split(redirects), isVisible: visible };
  function applyRights() {
    try {
      const value: unknown = JSON.parse(rawRights);
      if (!Array.isArray(value) || !value.every(r => r && Array.isArray(r.resource) && r.resource.length && r.resource.every((a: { id?: unknown; value?: unknown }) => typeof a.id === 'string' && typeof a.value === 'string'))) throw new Error('Forventet en liste med resource-attributter (id og value).');
      setRights(value as Right[]); setError('');
    } catch (e) { setError((e as Error).message); }
  }
  async function submit(e: React.FormEvent) {
    e.preventDefault(); setBusy(true); setError('');
    try {
      await api(create ? '/api/vendor/systems' : `/api/vendor/systems/${encodeURIComponent(system.id)}`, { method: create ? 'POST' : 'PUT', body: JSON.stringify(body) });
      refreshSystems(); refreshSystem();
      navigate(`/vendor${query(body.id)}`);
    } catch (e) { setError((e as Error).message); }
    finally { setBusy(false); }
  }
  return <><Link to="/vendor/systems">← Alle systemer</Link><h1>{create ? 'Registrer nytt system' : 'Rediger system og tilganger'}</h1>
    <p>Endringer lagres i systemregisteret på {configuration.environment}. Endrede tilganger på systemet endrer ikke automatisk eksisterende systembrukere.</p>
    <form onSubmit={submit}><fieldset disabled={busy}><section className="vendor-card"><h2>Virksomhet og system</h2><div className="vendor-form-grid">
      <label>Leverandørens organisasjonsnummer<input required pattern="[0-9]{9}" value={vendorOrg} readOnly={!create} onChange={e => { setVendorOrg(e.target.value); setId(e.target.value + '_' + id.split('_').slice(1).join('_')); }} /></label>
      <label>System-ID<input aria-label="System-ID" required pattern="[0-9]{9}_[^\s]+" value={id} readOnly={!create} onChange={e => setId(e.target.value)} /><small>Organisasjonsnummer etterfulgt av _ og et unikt navn.</small></label>
      {['nb', 'nn', 'en'].map(lang => <label key={lang}>Systemnavn ({lang})<input required={lang === 'nb'} value={names[lang] || ''} onChange={e => setNames({ ...names, [lang]: e.target.value })} /></label>)}
      {['nb', 'nn', 'en'].map(lang => <label key={lang}>Beskrivelse ({lang})<textarea value={descriptions[lang] || ''} onChange={e => setDescriptions({ ...descriptions, [lang]: e.target.value })} /></label>)}
      <label>Maskinporten-klient-ID-er<textarea value={clients} onChange={e => setClients(e.target.value)} placeholder="Én klient-ID per linje" /><small>Bruk klienter som tilhører denne integrasjonen. Altinn validerer bruken på tvers av systemer.</small></label>
      <label>Tillatte returadresser<textarea value={redirects} onChange={e => setRedirects(e.target.value)} placeholder="Én absolutt adresse per linje" /></label></div>
      <label className="vendor-check"><input type="checkbox" checked={visible} onChange={e => setVisible(e.target.checked)} />Synlig i Altinns systemregister</label></section>
      <section className="vendor-card"><h2>Tilgangspakker og enkeltrettigheter</h2><SelectedAccess rights={rights} packages={packages} onRights={setRights} onPackages={setPackages} />
        <AccessCatalogue rights={rights} packages={packages} onRights={setRights} onPackages={setPackages} />
        <details><summary>Avansert: rediger rettighetsattributter</summary><p>Brukes også for sammensatte appressurser. Knappen under erstatter listen over valgte enkeltrettigheter.</p>
          <button type="button" onClick={() => setRawRights(JSON.stringify(rights, null, 2))}>Hent valgte rettigheter til redigering</button>
          <label>Rettigheter som JSON<textarea className="vendor-code-input" value={rawRights} onChange={e => setRawRights(e.target.value)} /></label>
          <button type="button" onClick={applyRights}>Bruk rettighetene</button></details>
      </section>
      <JsonDetails value={body} label="Kontroller JSON som sendes til systemregisteret" />
      <Notice error={error} /><button className="vendor-button" type="submit">{busy ? 'Lagrer …' : create ? 'Registrer system i Altinn' : 'Lagre system i Altinn'}</button>
    </fieldset></form>
  </>;
}
