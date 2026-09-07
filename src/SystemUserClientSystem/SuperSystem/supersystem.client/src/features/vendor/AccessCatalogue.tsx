import { useVendor } from './context';
import { useState } from 'react';
import { AccessPackage, PackageSearch, Right, localized, rightKey, rightLabel, useLoad } from './api';
import { Notice } from './VendorLayout';
import { PackageRow } from './SystemOverview';

interface Resource { identifier: string; title: Record<string, string>; description: Record<string, string>; resourceType?: string }
interface Props {
  packages?: AccessPackage[]; rights?: Right[];
  onPackages?: (value: AccessPackage[]) => void; onRights?: (value: Right[]) => void;
  allowedPackages?: AccessPackage[];
}
export function AccessCatalogue({ packages = [], rights = [], onPackages, onRights, allowedPackages }: Props) {
  const [tab, setTab] = useState<'packages' | 'resources'>('packages');
  const [term, setTerm] = useState('');
  const [search, setSearch] = useState('');
  const results = useLoad<PackageSearch[]>(tab === 'packages' ? `/api/metadata/packages?term=${encodeURIComponent(search)}` : null);
  const resources = useLoad<{ total: number; data: Resource[] }>(tab === 'resources' ? `/api/metadata/resources?term=${encodeURIComponent(search)}` : null);
  return <div className="vendor-catalogue">
    <div className="vendor-tabs"><button type="button" aria-pressed={tab === 'packages'} onClick={() => setTab('packages')}>Tilgangspakker</button>
      {!allowedPackages && <button type="button" aria-pressed={tab === 'resources'} onClick={() => setTab('resources')}>Enkeltressurser</button>}</div>
    <div className="vendor-toolbar"><label>Søk i {tab === 'packages' ? 'tilgangspakker og deres tjenester' : 'ressursregisteret'}<input value={term} onChange={e => setTerm(e.target.value)}
      onKeyDown={e => { if (e.key === 'Enter') { e.preventDefault(); setSearch(term.trim()); } }} placeholder="Navn, beskrivelse eller ID" /></label><button type="button" onClick={() => setSearch(term.trim())}>Søk</button></div>
    <Notice loading={results.loading || resources.loading} error={results.error || resources.error} retry={() => { results.reload(); resources.reload(); }} />
    {tab === 'packages' && results.data && <><p>{results.data.length} treff</p><div className="vendor-catalogue-results">{results.data.map(({ object: p }) => {
      const selected = packages.some(item => item.urn === p.urn);
      const allowed = !allowedPackages || allowedPackages.some(item => item.urn === p.urn);
      return <article key={p.id} className="vendor-catalogue-item"><h3>{p.name}</h3><p>{p.description}</p>
        <PackageRow urn={p.urn} />
        {onPackages && <label className="vendor-check"><input type="checkbox" disabled={!allowed || !selected && !p.isDelegable} checked={selected}
          onChange={() => onPackages(selected ? packages.filter(item => item.urn !== p.urn) : [...packages, { urn: p.urn }])} />{selected ? 'Valgt' : allowed ? p.isDelegable ? 'Velg tilgangspakke' : 'Ikke delegerbar' : 'Ikke registrert på systemet'}</label>}
      </article>;
    })}</div>{!results.data.length && <p>Ingen treff. Prøv et annet søkeord.</p>}</>}
    {tab === 'resources' && resources.data && <><p>{resources.data.total} treff{resources.data.total > resources.data.data.length && ' · Viser de første 50. Avgrens søket for å finne flere.'}</p>
      <div className="vendor-catalogue-results">{resources.data.data.map(r => {
        const right = { resource: [{ id: 'urn:altinn:resource', value: r.identifier }] };
        const selected = rights.some(item => rightKey(item) === rightKey(right));
        return <article key={r.identifier} className="vendor-catalogue-item"><h3>{localized(r.title) || r.identifier}</h3><p>{localized(r.description)}</p><code>{r.identifier}</code>
          {onRights && <label className="vendor-check"><input type="checkbox" checked={selected} onChange={() => onRights(selected ? rights.filter(item => rightKey(item) !== rightKey(right)) : [...rights, right])} />{selected ? 'Valgt' : 'Velg enkeltrettighet'}</label>}</article>;
      })}</div>{!resources.data.total && <p>Ingen ressurser passer til søket.</p>}</>}
  </div>;
}
export function SelectedAccess({ rights, packages, onRights, onPackages }: Required<Pick<Props, 'rights' | 'packages' | 'onRights' | 'onPackages'>>) {
  return <div className="vendor-selected"><h3>Valgte tilganger ({rights.length + packages.length})</h3>
    {!rights.length && !packages.length && <p>Ingen tilganger valgt.</p>}
    {rights.map((r, i) => <div className="vendor-access-row" key={rightKey(r)}><code>{rightLabel(r)}</code><button type="button" onClick={() => onRights(rights.filter((_, index) => i !== index))}>Fjern</button></div>)}
    {packages.map(p => <div className="vendor-access-row" key={p.urn}><code>{p.urn}</code><button type="button" onClick={() => onPackages(packages.filter(item => item.urn !== p.urn))}>Fjern</button></div>)}
  </div>;
}
export function MetadataPage() {
  const { configuration } = useVendor();
  return <><p className="vendor-eyebrow">Metadata-API og ressursregister</p><h1>Tilgangspakker og tjenester</h1>
    <p>Søk i katalogen for {configuration.environment}, og se hvilke tjenester en pakke inneholder. Tilganger må registreres på systemet før de kan brukes i en forespørsel.</p>
    <section className="vendor-card"><AccessCatalogue /></section></>;
}
