import { useState } from 'react';
import { useLoad } from './api';
import { Notice } from './VendorLayout';

interface TestOrganisation { organisationNumber: string; name: string }
interface TestPerson { nationalIdentityNumber: string; name: string | null; roleCode: string; roleName: string }
export interface TestOrganisationDetails extends TestOrganisation { people: TestPerson[]; warning: string | null }

export function TestPersonCard({ organisation }: { organisation: TestOrganisationDetails }) {
  const [copyStatus, setCopyStatus] = useState('');
  async function copy(value: string) {
    try { await navigator.clipboard.writeText(value); setCopyStatus('Syntetisk fødselsnummer kopiert.'); }
    catch { setCopyStatus('Kunne ikke kopiere automatisk. Marker og kopier fødselsnummeret.'); }
  }
  return <div className="tenor-selection">
    <h3>{organisation.name}</h3><p>Organisasjonsnummer: {organisation.organisationNumber} · Syntetiske testdata fra Tenor</p>
    {organisation.people.map(person => <div key={person.nationalIdentityNumber} className="tenor-person">
      <strong>{person.roleName}: {person.name || 'Navn ikke tilgjengelig'}</strong>
      <p>Syntetisk fødselsnummer: <code>{person.nationalIdentityNumber}</code></p>
      <button type="button" onClick={() => copy(person.nationalIdentityNumber)}>Kopier fødselsnummer</button>
    </div>)}
    {!organisation.people.length && <p>Ingen daglig leder eller innehaver med syntetisk fødselsnummer er oppgitt i Tenor. Velg en annen virksomhet dersom du trenger en testperson.</p>}
    {organisation.warning && <p role="status">{organisation.warning}</p>}
    {!!organisation.people.length && <p>Bruk fødselsnummeret ved innlogging med TestID når du åpner godkjenningslenken i TT02.</p>}
    {copyStatus && <p role="status">{copyStatus}</p>}
  </div>;
}

function OrganisationDetails({ id, onSelect }: { id: string; onSelect: (value: TestOrganisationDetails) => void }) {
  const details = useLoad<TestOrganisationDetails>(`/api/testdata/organisations/${encodeURIComponent(id)}`);
  return <><Notice error={details.error} retry={details.reload} />{details.loading && <p role="status">Henter testperson fra Tenor …</p>}
    {details.data && <><TestPersonCard organisation={details.data} />
      <button type="button" className="vendor-button" onClick={() => onSelect(details.data!)}>Bruk virksomheten</button></>}
  </>;
}

export function TenorPicker({ onSelect }: { onSelect: (value: TestOrganisationDetails) => void }) {
  const configuration = useLoad<{ enabled: boolean }>('/api/testdata/configuration');
  const [term, setTerm] = useState('');
  const [searchUrl, setSearchUrl] = useState<string | null>(null);
  const [candidate, setCandidate] = useState('');
  const results = useLoad<{ organisations: TestOrganisation[]; hasMore: boolean }>(searchUrl);
  function search() {
    if (term.trim().length < 2) return;
    setCandidate('');
    const url = `/api/testdata/organisations?term=${encodeURIComponent(term.trim())}`;
    if (url === searchUrl) results.reload(); else setSearchUrl(url);
  }
  if (!configuration.error && !configuration.data?.enabled) return null;
  return <details className="tenor-picker"><summary>Finn testvirksomhet i Tenor</summary>
    <p>Søk etter virksomhet og finn daglig leder som testperson. Innehaver vises dersom daglig leder mangler.</p>
    <Notice error={configuration.error} retry={configuration.reload} />
    {configuration.data?.enabled && <><label>Virksomhetsnavn eller organisasjonsnummer
      <input value={term} maxLength={100} onChange={e => setTerm(e.target.value)} onKeyDown={e => { if (e.key === 'Enter') { e.preventDefault(); search(); } }} placeholder="Navn eller 9 siffer" /></label>
      <button type="button" disabled={results.loading || term.trim().length < 2} onClick={search}>Søk i Tenor</button>
      <Notice error={results.error} retry={results.reload} />
      {results.loading && <p role="status">Søker i Tenor …</p>}
      {results.data && <><p role="status">{results.data.organisations.length} virksomheter vist.{results.data.hasMore ? ' Flere treff finnes. Avgrens søket for å finne riktig virksomhet.' : ''}</p>
        <ul className="tenor-results">{results.data.organisations.map(org => <li key={org.organisationNumber}>
          <span><strong>{org.name}</strong><br />{org.organisationNumber}</span>
          <button type="button" aria-label={`Vis testperson for ${org.name}`} onClick={() => setCandidate(org.organisationNumber)}>Vis testperson</button>
        </li>)}</ul></>}
      {candidate && <OrganisationDetails key={candidate} id={candidate} onSelect={value => { setCandidate(''); onSelect(value); }} />}
    </>}
  </details>;
}
