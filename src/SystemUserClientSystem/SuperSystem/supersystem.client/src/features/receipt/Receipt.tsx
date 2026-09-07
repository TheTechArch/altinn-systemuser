import { Link } from 'react-router-dom';
import { SystemRequest, RequestKind, query, useLoad } from '../vendor/api';
import { JsonDetails, Notice, Status } from '../vendor/VendorLayout';
import '../vendor/Vendor.css';

function pendingRequest(): { id: string; kind: RequestKind; systemId: string } | null {
  try {
    const value = JSON.parse(sessionStorage.getItem('smartcloud.pendingRequest') || 'null');
    return value && typeof value.id === 'string' && typeof value.systemId === 'string' && ['standard', 'agent', 'change'].includes(value.kind) ? value : null;
  } catch { return null; }
}
export const Receipt = () => {
  const pending = pendingRequest();
  const request = useLoad<SystemRequest>(pending ? `/api/vendor/requests/${pending.kind}/${encodeURIComponent(pending.id)}` : null);
  return <div className="vendor-app"><main className="vendor-main vendor-receipt"><Link to="/">← SmartCloud</Link><h1>Status for systemtilgang</h1>
    <p>Returen fra Altinn bekrefter ikke i seg selv at forespørselen er godkjent. Status under hentes fra API-et.</p>
    <Notice {...request} retry={request.reload} />
    {request.data && <section className="vendor-card"><Status value={request.data.status} timedOut={request.data.timedOut} />
      <p>{request.data.integrationTitle || request.data.systemId} · {request.data.partyOrgNo}</p>
      <button onClick={request.reload} disabled={request.loading}>Oppdater status</button>
      <JsonDetails value={request.data} /></section>}
    {!pending && <p>Ingen forespørsel er lagret i denne nettleserfanen. Åpne forespørselsoversikten for å kontrollere status.</p>}
    <div className="vendor-actions"><Link to={`/vendor/requests${pending ? query(pending.systemId) + '&kind=' + pending.kind : ''}`}>Se forespørsler</Link><Link to="/dashboard">SmartCloud dashboard</Link></div>
  </main></div>;
};
