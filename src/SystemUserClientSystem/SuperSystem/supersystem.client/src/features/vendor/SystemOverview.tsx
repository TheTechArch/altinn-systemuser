import { useVendor } from './context';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import { localized, query, rightLabel, useLoad, PackageMetadata, ResourceMetadata } from './api';
import { JsonDetails, Notice } from './VendorLayout';

export function PackageInfo({ urn }: { urn: string }) {
  const info = useLoad<PackageMetadata>(`/api/metadata/packages/by-urn?urn=${encodeURIComponent(urn)}`);
  const resources = useLoad<ResourceMetadata[]>(info.data ? `/api/metadata/packages/${info.data.id}/resources` : null);
  return <div className="vendor-package-info"><Notice loading={info.loading || resources.loading} error={info.error || resources.error} retry={() => { info.reload(); resources.reload(); }} />
    {info.data && <><h3>{info.data.name}</h3><p>{info.data.description}</p><p>{info.data.isDelegable ? 'Kan delegeres' : 'Ikke delegerbar'}</p></>}
    {resources.data && <><h4>Tjenester i pakken ({resources.data.length})</h4><ul>{resources.data.map((r, i) => <li key={r.id || i}>{r.name || r.refId || r.id}</li>)}</ul>{!resources.data.length && <p>Ingen tjenester oppgitt i metadata.</p>}</>}
  </div>;
}
export function PackageRow({ urn }: { urn: string }) {
  const [open, setOpen] = useState(false);
  return <div className="vendor-access-row"><code>{urn}</code><button type="button" onClick={() => setOpen(!open)} aria-expanded={open}>{open ? 'Skjul innhold' : 'Se innhold'}</button>{open && <PackageInfo urn={urn} />}</div>;
}
export function SystemOverview() {
  const { system, systemId } = useVendor();
  return <>
    <div className="vendor-title"><div><p className="vendor-eyebrow">Systemregisteret</p><h1>{localized(system.name) || system.id}</h1><p>{localized(system.description)}</p></div>
      <Link className="vendor-button" to={`/vendor/new${query(systemId)}`}>Ny forespørsel</Link></div>
    <div className="vendor-stats"><div><strong>{system.rights?.length || 0}</strong><span>Registrerte ressurser</span></div>
      <div><strong>{system.accessPackages?.length || 0}</strong><span>Tilgangspakker</span></div>
      <div><strong>{system.isVisible ? 'Synlig' : 'Skjult'}</strong><span>I Altinns systemregister</span></div></div>
    <section className="vendor-card"><h2>Systeminformasjon</h2><dl><dt>System-ID</dt><dd>{system.id}</dd><dt>Leverandør</dt><dd>{system.vendor?.id || system.vendor?.ID}</dd>
      <dt>Maskinporten-klienter</dt><dd>{system.clientId?.join(', ') || 'Ingen oppgitt'}</dd>
      <dt>Tillatte returadresser</dt><dd>{system.allowedRedirectUrls?.length ? system.allowedRedirectUrls.map(url => <div key={url}>{url}</div>) : 'Ingen registrert. Utelat returadresse i forespørselen.'}</dd></dl></section>
    <section className="vendor-card"><h2>Tilganger systemet kan be om</h2><p>Kunden velger om en forespørsel skal godkjennes. Registrerte tilganger er rammen for nye forespørsler.</p>
      <h3>Enkeltressurser</h3><ul>{system.rights?.map((r, i) => <li key={i}><code>{rightLabel(r)}</code></li>)}</ul>{!system.rights?.length && <p>Ingen enkeltressurser registrert.</p>}
      <h3>Tilgangspakker</h3>{system.accessPackages?.map(p => <PackageRow key={p.urn} urn={p.urn} />)}{!system.accessPackages?.length && <p>Ingen tilgangspakker registrert.</p>}</section>
    <JsonDetails value={system} label="Vis systemregisterets API-svar" />
  </>;
}
