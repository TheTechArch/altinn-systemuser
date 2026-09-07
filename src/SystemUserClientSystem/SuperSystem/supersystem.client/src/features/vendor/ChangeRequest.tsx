import { useVendor } from './context';
import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { AccessPackage, Right, SystemRequest, SystemUser, api, rememberRequest, query, useLoad } from './api';
import { JsonDetails, Notice } from './VendorLayout';
import { RegisteredAccessPicker } from './NewRequest';
import { RequestResult } from './Requests';

export function ChangeRequest() {
  const { id = '' } = useParams();
  const { systemId, configuration } = useVendor();
  const users = useLoad<SystemUser[]>(`/api/vendor/systems/${encodeURIComponent(systemId)}/users`);
  const user = users.data?.find(u => u.id === id);
  const [requiredRights, setRights] = useState<Right[]>([]);
  const [requiredAccessPackages, setPackages] = useState<AccessPackage[]>([]);
  const [removeResources, setRemoveResources] = useState('');
  const [removePackages, setRemovePackages] = useState('');
  const [redirectUrl, setRedirect] = useState(configuration.defaultRedirectUrl || '');
  const [correlationId] = useState(() => crypto.randomUUID());
  const [result, setResult] = useState<SystemRequest | null>(null);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const split = (value: string) => [...new Set(value.split(/\r?\n/).map(v => v.trim()).filter(Boolean))];
  const body = { requiredRights, requiredAccessPackages, unwantedRights: split(removeResources).map(value => ({ resource: [{ id: 'urn:altinn:resource', value }] })),
    unwantedAccessPackages: split(removePackages).map(urn => ({ urn })), redirectUrl: redirectUrl.trim() || null };
  async function submit(e: React.FormEvent) {
    e.preventDefault(); setBusy(true); setError('');
    try {
      const response = await api<SystemRequest>(`/api/vendor/users/${encodeURIComponent(id)}/change-requests/${correlationId}`, { method: 'POST', body: JSON.stringify(body) });
      rememberRequest(response, 'change'); setResult(response);
    } catch (e) { setError((e as Error).message); }
    finally { setBusy(false); }
  }
  return <><Link to={`/vendor/systemusers/${id}${query(systemId)}`}>← Systembruker</Link><h1>Be om endrede tilganger</h1>
    <Notice {...users} retry={users.reload} />
    {users.data && (!user || user.isDeleted) && <p role="alert">Ingen aktiv systembruker med denne ID-en i valgt system.</p>}
    {result ? <><RequestResult request={result} kind="change" /><JsonDetails value={result} /></> : user && !user.isDeleted && <form onSubmit={submit}><fieldset disabled={busy}>
      <p>{user.integrationTitle} · {user.reporteeOrgNo}</p>
      <section className="vendor-card"><h2>Legg til tilganger</h2><RegisteredAccessPicker rights={requiredRights} packages={requiredAccessPackages} onRights={setRights} onPackages={setPackages} agent={user.userType.toLowerCase() === 'agent'} /></section>
      <section className="vendor-card"><h2>Fjern tilganger</h2><p>Leverandør-API-et gir ikke en liste over dagens delegeringer. Oppgi ID eller URN for tilgangene som skal fjernes; Altinn kontrollerer endringen.</p>
        <div className="vendor-form-grid">{user.userType.toLowerCase() !== 'agent' && <label>Ressurs-ID-er, én per linje<textarea value={removeResources} onChange={e => setRemoveResources(e.target.value)} placeholder="ske-krav-og-betalinger" /></label>}
          <label>Tilgangspakke-URN-er, én per linje<textarea value={removePackages} onChange={e => setRemovePackages(e.target.value)} placeholder="urn:altinn:accesspackage:…" /></label></div></section>
      <section className="vendor-card"><h2>Kontroller endringen</h2><label>Returadresse (valgfritt)<input type="url" value={redirectUrl} onChange={e => setRedirect(e.target.value)} /></label>
        <p>Korrelasjons-ID: <code>{correlationId}</code>. Samme ID brukes ved nytt forsøk i dette skjemaet.</p><JsonDetails value={body} label="Vis JSON for endringsforespørselen" /><Notice error={error} />
        <button className="vendor-button" type="submit">{busy ? 'Oppretter …' : 'Opprett endringsforespørsel'}</button></section>
    </fieldset></form>}
  </>;
}
