import { useVendor } from './context';
import { useState } from 'react';
import { Link, useParams, useSearchParams } from 'react-router-dom';
import { SystemRequest, RequestKind, api, dateLabel, query, rightLabel, rememberRequest, safeLink, useLoad } from './api';
import { JsonDetails, Notice, Status } from './VendorLayout';
import { PackageRow } from './SystemOverview';

export function RequestResult({ request, kind }: { request: SystemRequest; kind: RequestKind }) {
  const url = safeLink(request.confirmUrl);
  return <section className="vendor-card"><h2>Forespørsel hos Altinn</h2><p><Status value={request.status} timedOut={request.timedOut} /></p>
    <p>{request.integrationTitle || request.systemId} · {request.partyOrgNo}</p><p>Referanse: {request.externalRef || 'Ikke oppgitt'}</p>
    {url && !request.timedOut && request.status.toLowerCase() === 'new' &&
      <a className="vendor-button" href={url} onClick={() => rememberRequest(request, kind)}>Åpne godkjenning i Altinn</a>}
    <p>Forespørsel-ID: <code>{request.id}</code></p>
    {request.status.toLowerCase() !== 'nochangeneeded' && <Link to={`/vendor/requests/${kind}/${request.id}${query(request.systemId)}`}>Se status og detaljer</Link>}
  </section>;
}

export function Requests() {
  const { systemId } = useVendor();
  const [params, setParams] = useSearchParams();
  const selected = params.get('kind');
  const kind: RequestKind = selected === 'agent' || selected === 'change' ? selected : 'standard';
  const requests = useLoad<SystemRequest[]>(`/api/vendor/systems/${encodeURIComponent(systemId)}/requests/${kind}`);
  const [search, setSearch] = useState(params.get('org') || '');
  const [status, setStatus] = useState('');
  const rows = requests.data?.filter(r => (!status || (status === 'expired' ? r.timedOut : !r.timedOut && r.status.toLowerCase() === status)) &&
    [r.partyOrgNo, r.externalRef, r.integrationTitle, r.id].some(v => v?.toLowerCase().includes(search.toLowerCase()))) || [];
  return <><div className="vendor-title"><div><p className="vendor-eyebrow">Opprettelse og endringer</p><h1>Forespørsler</h1><p>Følg kundens godkjenning og åpne forespørslene i Altinn.</p></div><Link className="vendor-button" to={`/vendor/new${query(systemId)}`}>Ny forespørsel</Link></div>
    <div className="vendor-tabs" aria-label="Forespørselstype">{(['standard', 'agent', 'change'] as const).map(k => <button key={k} aria-pressed={kind === k}
      onClick={() => { setStatus(''); setParams({ system: systemId, kind: k }); }}>{k === 'standard' ? 'Eget system' : k === 'agent' ? 'Klientsystem' : 'Endringer'}</button>)}</div>
    <div className="vendor-toolbar"><label>Søk<input value={search} onChange={e => setSearch(e.target.value)} placeholder="Organisasjon, navn eller referanse" /></label>
      <label>Status<select aria-label="Status" value={status} onChange={e => setStatus(e.target.value)}><option value="">Alle statuser</option><option value="new">Venter på godkjenning</option><option value="accepted">Godkjent</option><option value="rejected">Avvist</option><option value="expired">Utløpt</option></select></label>
      <button disabled={requests.loading} onClick={requests.reload}>Oppdater</button></div>
    <Notice {...requests} retry={requests.reload} />
    {requests.data && <section className="vendor-card"><h2>{rows.length} forespørsler</h2>{rows.length ? <div className="vendor-table-wrap"><table><thead><tr><th>Forespørsel</th><th>Organisasjon</th><th>Status</th><th>Opprettet</th></tr></thead><tbody>
      {rows.map(r => <tr key={r.id}><td><Link to={`/vendor/requests/${kind}/${r.id}${query(systemId)}`}>{r.integrationTitle || r.externalRef || r.id}</Link><small>{r.id}</small></td>
        <td>{r.partyOrgNo}</td><td><Status value={r.status} timedOut={r.timedOut} /></td><td>{dateLabel(r.created)}</td></tr>)}</tbody></table></div> :
      <p>{requests.data.length ? 'Ingen forespørsler passer til filtrene.' : 'Ingen forespørsler av denne typen for valgt system.'}</p>}</section>}
  </>;
}

export function RequestDetail() {
  const { systemId } = useVendor();
  const { kind = '', id = '' } = useParams();
  const validKind = ['standard', 'agent', 'change'].includes(kind);
  const request = useLoad<SystemRequest>(validKind ? `/api/vendor/requests/${kind}/${encodeURIComponent(id)}` : null);
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [deleted, setDeleted] = useState(false);
  async function remove() {
    setBusy(true); setError('');
    try { await api(`/api/vendor/requests/${kind}/${encodeURIComponent(id)}`, { method: 'DELETE' }); setDeleted(true); }
    catch (e) { setError((e as Error).message); }
    finally { setBusy(false); }
  }
  const r = request.data;
  return <><Link to={`/vendor/requests${query(systemId)}&kind=${kind}`}>← Forespørsler</Link><div className="vendor-title"><h1>Forespørsel</h1><button disabled={request.loading || deleted} onClick={request.reload}>Oppdater status</button></div>
    <Notice loading={request.loading} error={error || request.error || (!validKind ? 'Ukjent forespørselstype.' : '')} retry={request.reload} />
    {deleted ? <p role="status" className="vendor-notice">Forespørselen er slettet.</p> : r && <><RequestResult request={r} kind={kind as RequestKind} />
      <section className="vendor-card"><h2>Innhold i forespørselen</h2>{r.systemId !== systemId && <p>Denne forespørselen tilhører systemet {r.systemId}.</p>}
        <dl><dt>Opprettet</dt><dd>{dateLabel(r.created)}</dd><dt>Returadresse</dt><dd>{r.redirectUrl || 'Ingen'}</dd><dt>Eskalert</dt><dd>{r.escalated ? 'Ja' : 'Nei'}</dd></dl>
        <h3>{kind === 'change' ? 'Tilganger som skal legges til' : 'Ønskede tilganger'}</h3>
        <ul>{(r.rights || r.requiredRights || []).map((right, i) => <li key={i}>{rightLabel(right)}</li>)}</ul>
        {(r.accessPackages || r.requiredAccessPackages || []).map(p => <PackageRow key={p.urn} urn={p.urn} />)}
        {kind === 'change' && <><h3>Tilganger som skal fjernes</h3><ul>{r.unwantedRights?.map((right, i) => <li key={i}>{rightLabel(right)}</li>)}</ul>{r.unwantedAccessPackages?.map(p => <PackageRow key={p.urn} urn={p.urn} />)}</>}
      </section>
      {kind !== 'agent' && <section className="vendor-card"><h2>Slett forespørsel</h2><p>Dette sletter forespørselen. En opprettet systembruker slettes ikke.</p>
        {confirmDelete ? <div role="group" aria-label="Bekreft sletting"><p>Vil du slette forespørsel {r.id}?</p><button disabled={busy} onClick={remove}>Ja, slett forespørselen</button> <button disabled={busy} onClick={() => setConfirmDelete(false)}>Avbryt</button></div> :
          <button onClick={() => setConfirmDelete(true)}>Slett forespørsel …</button>}</section>}
      <JsonDetails value={r} /></>}
  </>;
}
