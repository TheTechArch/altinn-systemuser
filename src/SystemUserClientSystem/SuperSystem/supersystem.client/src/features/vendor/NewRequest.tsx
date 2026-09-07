import { useVendor } from './context';
import { useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { AccessPackage, Right, SystemRequest, api, rememberRequest, query, rightKey, rightLabel } from './api';
import { JsonDetails, Notice } from './VendorLayout';
import { AccessCatalogue, SelectedAccess } from './AccessCatalogue';
import { PackageRow } from './SystemOverview';
import { RequestResult } from './Requests';

export function RegisteredAccessPicker({ rights, packages, onRights, onPackages, agent = false }: {
  rights: Right[]; packages: AccessPackage[]; onRights: (value: Right[]) => void; onPackages: (value: AccessPackage[]) => void; agent?: boolean;
}) {
  const { system } = useVendor();
  return <><SelectedAccess rights={rights} packages={packages} onRights={onRights} onPackages={onPackages} />
    {!agent && <><h3>Enkeltrettigheter registrert på systemet</h3>{system.rights?.map(r => <label key={rightKey(r)} className="vendor-check"><input type="checkbox" checked={rights.some(item => rightKey(item) === rightKey(r))}
      onChange={e => onRights(e.target.checked ? [...rights, r] : rights.filter(item => rightKey(item) !== rightKey(r)))} />{rightLabel(r)}</label>)}
      {!system.rights?.length && <p>Ingen enkeltrettigheter registrert.</p>}</>}
    <h3>Tilgangspakker registrert på systemet</h3>{system.accessPackages?.map(p => <div key={p.urn}><label className="vendor-check"><input type="checkbox" checked={packages.some(item => item.urn === p.urn)}
      onChange={e => onPackages(e.target.checked ? [...packages, p] : packages.filter(item => item.urn !== p.urn))} />{p.urn}</label><PackageRow urn={p.urn} /></div>)}
    {!system.accessPackages?.length && <p>Ingen tilgangspakker registrert. Legg til pakker i systemregisteret før du oppretter en forespørsel med pakker.</p>}
    <details><summary>Finn pakker etter navn eller tjeneste</summary><AccessCatalogue packages={packages} onPackages={onPackages} allowedPackages={system.accessPackages || []} /></details>
  </>;
}

export function NewRequest() {
  const { system, systemId, configuration } = useVendor();
  const [params] = useSearchParams();
  const preset = configuration.presets.find(p => p.id === params.get('product'));
  const [kind, setKind] = useState<'standard' | 'agent'>('standard');
  const [org, setOrg] = useState(params.get('org') || params.get('organisajonsnr') || '');
  const [title, setTitle] = useState('');
  const [externalRef, setExternalRef] = useState('');
  const [redirect, setRedirect] = useState(configuration.defaultRedirectUrl || '');
  const [rights, setRights] = useState<Right[]>((system.rights || []).filter(r => preset?.resources.includes(rightLabel(r))));
  const [packages, setPackages] = useState<AccessPackage[]>([]);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const [result, setResult] = useState<SystemRequest | null>(null);
  const body = { systemId, partyOrgNo: org.trim(), integrationTitle: title.trim() || null, externalRef: externalRef.trim() || null,
    redirectUrl: redirect.trim() || null, ...(kind === 'standard' ? { rights } : {}), accessPackages: packages };
  async function submit(e: React.FormEvent) {
    e.preventDefault(); setError('');
    if (!rights.length && !packages.length) { setError('Velg minst én tilgang.'); return; }
    setBusy(true);
    try {
      const response = await api<SystemRequest>(`/api/vendor/requests/${kind}`, { method: 'POST', body: JSON.stringify(body) });
      rememberRequest(response, kind); setResult(response);
    } catch (e) { setError((e as Error).message); }
    finally { setBusy(false); }
  }
  return <><div className="vendor-title"><div><p className="vendor-eyebrow">Opprett systembruker</p><h1>Ny forespørsel</h1><p>Velg hvem systemet skal representere, og hvilke tilganger kunden skal godkjenne.</p></div>
    <Link to={`/vendor/settings${query(systemId)}`}>Rediger systemets tilganger</Link></div>
    {result ? <><p role="status" className="vendor-notice">Altinn har returnert forespørselen. Ved gjenbruk av system, organisasjon og ekstern referanse kan dette være en eksisterende forespørsel.</p>
      <RequestResult request={result} kind={kind} /><JsonDetails value={result} label="Vis svaret fra Altinn" />
      <button onClick={() => { setResult(null); setExternalRef(''); }}>Tilbake til konfigurasjon</button></> :
    <form onSubmit={submit}><fieldset disabled={busy}><section className="vendor-card"><h2>1. Type og organisasjon</h2>
      <label>Systembrukertype<select value={kind} onChange={e => { const value = e.target.value as 'standard' | 'agent'; setKind(value); if (value === 'agent') setRights([]); }}>
        <option value="standard">Vanlig systembruker – eget system</option><option value="agent">Agentsystembruker – klientsystem</option></select></label>
      <p>{kind === 'agent' ? 'Representerer kundens klienter. Bare tilgangspakker kan velges. Kunden må knytte klienter til systembrukeren etter godkjenning.' : 'Representerer organisasjonen som godkjenner forespørselen. Velg tilgangspakker og/eller enkeltrettigheter.'}</p>
      <div className="vendor-form-grid"><label>Organisasjonsnummer<input required inputMode="numeric" pattern="[0-9]{9}" maxLength={9} value={org} onChange={e => setOrg(e.target.value)} placeholder="9 siffer" /></label>
        <label>Navn på systembrukeren (valgfritt)<input value={title} onChange={e => setTitle(e.target.value)} placeholder="Bruker systemnavnet hvis tomt" /></label>
        <label>Ekstern referanse (valgfritt)<input value={externalRef} onChange={e => setExternalRef(e.target.value)} /><small>Tom verdi bruker organisasjonsnummeret. Bruk samme referanse ved gjentatte forsøk for samme integrasjon.</small></label>
        <label>Returadresse (valgfritt)<input type="url" list="request-redirects" value={redirect} onChange={e => setRedirect(e.target.value)} placeholder="https://…/receipt" />
          <datalist id="request-redirects">{system.allowedRedirectUrls?.map(url => <option key={url} value={url} />)}</datalist><small>Må samsvare med en tillatt returadresse i systemregisteret. Kan utelates.</small></label></div></section>
      <section className="vendor-card"><h2>2. Tilganger</h2>{kind === 'standard' && <div className="vendor-actions">{configuration.presets.map(p => <button type="button" key={p.id}
        onClick={() => setRights((system.rights || []).filter(r => p.resources.includes(rightLabel(r))))}>Bruk {p.name}</button>)}</div>}
        <RegisteredAccessPicker rights={rights} packages={packages} onRights={setRights} onPackages={setPackages} agent={kind === 'agent'} /></section>
      <section className="vendor-card"><h2>3. Kontroller og send</h2><p>Forespørselen opprettes i {configuration.environment}. Kunden godkjenner tilgangene i Altinn.</p>
        <JsonDetails value={body} label="Vis nøyaktig JSON som sendes" /><Notice error={error} />
        <button className="vendor-button" type="submit" disabled={!rights.length && !packages.length}>{busy ? 'Oppretter …' : 'Opprett forespørsel'}</button></section>
    </fieldset></form>}
  </>;
}
