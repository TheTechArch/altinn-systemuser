import { useVendor } from './context';
import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { SystemUser, dateLabel, query, useLoad } from './api';
import { JsonDetails, Notice } from './VendorLayout';

export function SystemUsers() {
  const { systemId } = useVendor();
  const users = useLoad<SystemUser[]>(`/api/vendor/systems/${encodeURIComponent(systemId)}/users`);
  const [search, setSearch] = useState('');
  const [type, setType] = useState('');
  const [status, setStatus] = useState('active');
  const rows = users.data?.filter(u => (!type || u.userType.toLowerCase() === type) &&
    (status === 'all' || (status === 'deleted' ? u.isDeleted : !u.isDeleted)) &&
    [u.integrationTitle, u.reporteeOrgNo, u.externalRef, u.id].some(v => v?.toLowerCase().includes(search.toLowerCase()))) || [];
  return <><div className="vendor-title"><div><p className="vendor-eyebrow">Leverandørens systembrukere</p><h1>Systembrukere</h1><p>Alle systembrukere som Altinn returnerer for valgt system.</p></div>
    <Link className="vendor-button" to={`/vendor/new${query(systemId)}`}>Ny forespørsel</Link></div>
    <div className="vendor-toolbar"><label>Søk<input value={search} onChange={e => setSearch(e.target.value)} placeholder="Navn, organisasjonsnummer eller referanse" /></label>
      <label>Type<select value={type} onChange={e => setType(e.target.value)}><option value="">Alle typer</option><option value="standard">Eget system</option><option value="agent">Klientsystem</option></select></label>
      <label>Status<select aria-label="Status" value={status} onChange={e => setStatus(e.target.value)}><option value="active">Aktive</option><option value="deleted">Slettede</option><option value="all">Alle</option></select></label>
      <button disabled={users.loading} onClick={users.reload}>Oppdater</button></div>
    <Notice {...users} retry={users.reload} />
    {users.data && <section className="vendor-card"><h2>{rows.length} systembrukere</h2>
      {rows.length ? <div className="vendor-table-wrap"><table><thead><tr><th>Systembruker</th><th>Organisasjon</th><th>Type</th><th>Status</th><th>Opprettet</th></tr></thead>
        <tbody>{rows.map(u => <tr key={u.id}><td><Link to={`/vendor/systemusers/${u.id}${query(systemId)}`}>{u.integrationTitle || u.productName || u.id}</Link><small>{u.externalRef}</small></td>
          <td>{u.reporteeOrgNo}</td><td>{u.userType.toLowerCase() === 'agent' ? 'Klientsystem' : 'Eget system'}</td><td>{u.isDeleted ? 'Slettet' : 'Aktiv'}</td><td>{dateLabel(u.created)}</td></tr>)}</tbody></table></div>
        : <p>{users.data.length ? 'Ingen systembrukere passer til filtrene.' : 'Ingen systembrukere er opprettet for dette systemet ennå. Opprett en forespørsel og la kunden godkjenne den i Altinn.'}</p>}</section>}
  </>;
}

export function SystemUserDetail() {
  const { systemId } = useVendor();
  const { id } = useParams();
  const users = useLoad<SystemUser[]>(`/api/vendor/systems/${encodeURIComponent(systemId)}/users`);
  const user = users.data?.find(u => u.id === id);
  return <><Link to={`/vendor/systemusers${query(systemId)}`}>← Systembrukere</Link>
    <Notice {...users} retry={users.reload} />
    {users.data && !user && <p role="alert">Systembrukeren finnes ikke i valgt system.</p>}
    {user && <><div className="vendor-title"><div><h1>{user.integrationTitle || user.id}</h1><p>{user.isDeleted ? 'Slettet systembruker' : 'Aktiv systembruker'}</p></div>
      {!user.isDeleted && <Link className="vendor-button" to={`/vendor/systemusers/${user.id}/change${query(systemId)}`}>Be om endrede tilganger</Link>}</div>
      <section className="vendor-card"><h2>Identitet i Altinn</h2><dl><dt>Systembruker-ID</dt><dd>{user.id}</dd><dt>Organisasjonsnummer</dt><dd>{user.reporteeOrgNo}</dd>
        <dt>Ekstern referanse</dt><dd>{user.externalRef}</dd><dt>Type</dt><dd>{user.userType}</dd><dt>Opprettet</dt><dd>{dateLabel(user.created)}</dd><dt>Leverandør</dt><dd>{user.supplierName} {user.supplierOrgno}</dd></dl></section>
      <section className="vendor-card"><h2>Tilganger og godkjenning</h2><p>Dette leverandør-API-et returnerer systembrukerens identitet og status. Gjeldende delegeringer følger ikke med i svaret.</p>
        <p>Innholdet i tidligere forespørsler viser hva det ble bedt om, og er ikke en fullstendig oversikt over dagens tilganger.</p>
        {user.userType.toLowerCase() === 'agent' && <p>Klientdelegeringer administreres med innlogget sluttbruker via Altinns klientdelegerings-API. Kunden må også knytte klienter til systembrukeren i Altinn.</p>}
        <Link to={`/vendor/requests${query(systemId)}&org=${user.reporteeOrgNo}`}>Se forespørsler for organisasjonen</Link></section>
      <JsonDetails value={user} /></>}
  </>;
}
