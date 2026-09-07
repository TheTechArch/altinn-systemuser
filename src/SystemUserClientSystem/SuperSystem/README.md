# SmartCloud – systembruker som referanseimplementasjon

Start på **/vendor/systems**. Leverandøren identifiseres med virksomhetens Maskinporten-integrasjon på serveren. Oppsettet i repoet peker på **TT02**.

## Funksjonalitet

- List leverandørens registrerte systemer og velg system.
- Registrer nye systemer med leverandør-ID, navn/beskrivelse på flere språk, Maskinporten-klienter, synlighet og tillatte returadresser.
- Rediger systemet og dets tilgangspakker og enkeltrettigheter. Øvrige språk, klienter og returadresser bevares i oppdateringen.
- Søk etter tilgangspakker via metadata-API-et, se tjenestene i pakken og finn enkeltressurser i ressursregisteret. Avansert rettighetsredigering støtter sammensatte ressursattributter.
- Opprett vanlige forespørsler med enkeltrettigheter og/eller tilgangspakker, eller agentforespørsler med bare tilgangspakker.
- Velg organisasjonsnummer, integrasjonsnavn, ekstern referanse og valgfri returadresse. Tilgangsvalgene hentes fra det registrerte systemet.
- List og filtrer systembrukere og forespørsler. Egne detaljsider viser identitet, status, forespørselsinnhold og API-data.
- Opprett endringsforespørsler med tilganger som skal legges til eller fjernes. Korrelasjons-ID gjenbrukes ved nye forsøk i samme skjema.
- Slett vanlige forespørsler og endringsforespørsler etter bekreftelse. Agentforespørsler har ikke et tilsvarende vendor-delete-endepunkt i den undersøkte kontrakten.
- Kvitteringen henter forespørselsstatus fra API-et. En retur fra Altinn er ikke i seg selv bevis på godkjenning.

## Lokal oppstart

Krever .NET 10 SDK/runtime, Node og npm.

```powershell
# Fra src/SystemUserClientSystem/SuperSystem
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project SuperSystem.Server --no-launch-profile --urls http://localhost:5236
```

I en annen terminal:

```powershell
cd supersystem.client
npm ci
$env:ASPNETCORE_URLS = 'http://localhost:5236'
$env:SMARTCLOUD_HTTP = '1'
npm run dev
```

Åpne `http://localhost:5173/vendor/systems`. Uten `SMARTCLOUD_HTTP=1` bruker Vite HTTPS-oppsettet. Vite proxyer `/api`, `/Redirect` og `/Authenticate` til backend. Produksjonsbygg trenger ikke utviklingssertifikat.

## Konfigurasjon

Bruk eksisterende `Maskinporten`-konfigurasjon via user-secrets eller miljøvariabler. Private nøkler og access tokens sendes ikke til de nye sidene.

| SystemRegister-innstilling | Bruk |
| --- | --- |
| `BaseAdress` | Authentication-base, f.eks. `https://platform.tt02.altinn.no/authentication/api/v1/`. Eksisterende feltnavn beholdes. |
| `SystemId` | Foretrukket system. Alle leverandørens systemer kan velges. |
| `SystemRegisterScope` | `altinn:authentication/systemregister.write` – systemregister og systembrukerliste. |
| `RequestSystemUserScope` | `altinn:authentication/systemuser.request.write` – opprettelse, endring, sletting av forespørsler og enkeltoppslag. |
| `ScopeSystemUserRequestRead` | `altinn:authentication/systemuser.request.read` – forespørselslister og status. |
| `DefaultRedirectUrl` | Valgfri forhåndsutfylt returadresse, f.eks. egen `/receipt`. Må være tillatt på systemet. Kan utelates. |
| `RightResourcesBasic`, `RightResourcesStandard` | Kommaseparerte ressursvalg for produktforhåndsvalg. Bare registrerte ressurser velges. |

Metadata og ressursregister bruker samme plattformvert som Authentication, slik at TT02-data ikke blandes med produksjon. Den nye leverandørdelen bruker Maskinporten-token direkte.

System-ID begynner med virksomhetens organisasjonsnummer og `_`. `vendor.ID` har formen `0192:991825827`. Altinn kontrollerer eierskap, klient-ID-er, tilganger og returadresser.

## API-kart

Altinn-stiene under er relative til `/authentication/api/v1/`.

| SmartCloud | Altinn |
| --- | --- |
| `GET/POST /api/vendor/systems` | `GET/POST systemregister/vendor` |
| `GET/PUT /api/vendor/systems/{id}` | `GET/PUT systemregister/vendor/{id}` |
| `GET /api/vendor/systems/{id}/users` | `GET systemuser/vendor/bysystem/{id}` |
| `GET /api/vendor/systems/{id}/users/lookup?orgno=…&externalRef=…` | `GET systemuser/vendor/byquery?system-id=…&orgno=…&external-ref=…` |
| `GET /api/vendor/systems/{id}/requests/standard` | `GET systemuser/request/vendor/bysystem/{id}` |
| Samme med `agent` | `GET systemuser/request/vendor/agent/bysystem/{id}` |
| Samme med `change` | `GET systemuser/changerequest/vendor/bysystem/{id}` |
| `POST /api/vendor/requests/standard` | `POST systemuser/request/vendor` |
| `POST /api/vendor/requests/agent` | `POST systemuser/request/vendor/agent` |
| `GET /api/vendor/requests/{kind}/{requestId}` | Statusendepunkt for aktuell forespørselstype |
| `POST /api/vendor/users/{id}/change-requests/{correlationId}` | `POST systemuser/changerequest/vendor?system-user-id=…&correlation-id=…` |
| `DELETE /api/vendor/requests/{kind}/{requestId}` | Vendor-delete for `standard` eller `change` |

Systemlisten returnerer `RegisteredSystemDTO` med `systemId`; detaljoppslaget returnerer `RegisteredSystemResponse` med `id`. Frontendtypene skiller disse kontraktene.

Alle paginerte leverandørlister følges til slutt. Bare `token`-verdien fra `links.next` brukes videre på opprinnelig endepunkt. Dette håndterer interne vertsnavn i Altinns next-lenker uten å sende bearer-token til en annen vert. Ugyldige/gjentatte markører gir feil; delvise lister vises ikke som komplette.

Metadata bruker `/accessmanagement/api/v1/meta/info/accesspackages/search`, `urn/{urn}` og `{id}/resource`, som i tjenesteoversikten. Ressurser hentes fra `/resourceregistry/api/v1/resource/resourcelist?includeApps=true&includeAltinn2=false`. Søket viser opptil 50 ressurser og oppgir totalt antall treff. Offentlige metadata caches i ti minutter.

## Avgrensninger

- SmartCloud har fortsatt demoens innloggingsmodell. Leverandør-API-et har ikke en egen autentisert administratorøkt. Sett løsningen bak autentisering og autoriser administratorer før den eksponeres utenfor et kontrollert demomiljø.
- Leverandørens systembrukeroppslag viser identitet/status, ikke fullstendige gjeldende delegeringer. Forespørselsinnhold og systemregisterinnhold vises derfor ikke som kundens nåværende rettigheter.
- Klientdelegerings-API-ene krever innlogget sluttbruker / ID-porten og egne scopes. Agentforespørsler støttes; klientdelegeringer administreres fortsatt i Altinn.
- Fjerning i endringsskjemaet støtter ressurs-ID-er og pakke-URN-er. Avansert fjerning av sammensatte ressursattributter er tilgjengelig via backendens JSON-kontrakt.
- Endring av systemregisteret oppdaterer ikke automatisk eksisterende systembrukere. Bruk endringsforespørsler når kunden må godkjenne tilganger.
- Tom ekstern referanse bruker organisasjonsnummeret. De eldre fagmodulene og demo-innloggingen er fortsatt organisasjonsnummerbaserte og velger ikke vilkårlig system/ekstern referanse ved tokenutstedelse.
- Ingen ekte opprettelser, oppdateringer eller slettinger i TT02 inngår i testene.

## Testvirksomhet og daglig leder fra Tenor

I «Ny forespørsel» kan du åpne «Finn testvirksomhet i Tenor», søke på navn, organisasjonsnummer eller organisasjonsform, vise testpersonen og velge virksomheten. Organisasjonsnummeret fylles i skjemaet. Daglig leder (`DAGL`) prioriteres; innehaver (`INNH`) vises med egen rollebetegnelse dersom daglig leder mangler. Navn hentes fra gjeldende navneoppføring i Folkeregister-kilden. Manglende navn eller roller vises eksplisitt.

Velg for eksempel «Enkeltpersonforetak (ENK)» og la navnefeltet stå tomt. Organisasjonsform kan også kombineres med navn eller organisasjonsnummer; begge kriterier må da stemme. «Alle organisasjonsformer» fjerner formfilteret. Valgene kommer fra et lokalt uttrekk av [Brønnøysundregistrenes organisasjonsformer](https://data.brreg.no/enhetsregisteret/api/organisasjonsformer), kontrollert 7. september 2026 (`organisationForms.json`). Tilgjengelige testdata kan variere mellom organisasjonsformene.

Legg scopet `skatteetaten:testnorge/testdata.read` til på den eksisterende Maskinporten-klienten. Virksomheten må også ha fått tilgang til Tenors søke-API. Backend bruker samme `Maskinporten:ClientId` og `Maskinporten:EncodedJwk` som ellers, men ber om et eget token med bare Tenor-scopet og uten systembrukerens `authorization_details`. Tokenet sendes direkte til Tenor og veksles ikke hos Altinn. Altinn-scopene endres ikke.

Funksjonen er tilgjengelig når `Maskinporten:Environment` er `test` og `SystemRegister:BaseAdress` peker på `platform.tt02.altinn.no`. Ingen nye innstillinger eller hemmeligheter kreves. Andre miljøer avvises på serveren før tokenhenting.

| SmartCloud API | Tenor-kall |
| --- | --- |
| `GET /api/testdata/configuration` | Lokal kontroll av testmiljø, ingen tokenhenting |
| `GET /api/testdata/organisations?organisationForm=ENK&term=...` | `/api/testnorge/v2/soek/brreg-er-fr`, KQL på navn/organisasjonsnummer og/eller `organisasjonsform.kode`; minst ett filter kreves |
| `GET /api/testdata/organisations/{orgnr}` | Virksomhetens kildedata og rolleopplysninger; `/soek/freg` for testpersonens navn |

Tenor-basen er `https://testdata.api.skatteetaten.no`. Søk viser maksimalt ti virksomheter; avgrens søket ved flere treff. Input behandles som en søkeverdi, ikke som fri KQL. Personer hentes først når en virksomhet åpnes. Kildedata og tokens sendes ikke til nettleseren; bare virksomhetsnavn, organisasjonsnummer og relevante testpersoner returneres.

Testpersonens syntetiske fødselsnummer kan kopieres for TestID-innlogging i TT02. Personen vises også ved godkjenningslenken etter opprettelse. Valget beholdes i den åpne siden, og fjernes ved manuell endring av organisasjonsnummeret; det lagres ikke mellom sideoppdateringer. Fødselsnummeret inngår ikke i systembrukerforespørselen. Tenor-feil blokkerer ikke manuell utfylling.

Kontrakten er kontrollert mot [Tenors gjeldende OpenAPI-spesifikasjon](https://gist.githubusercontent.com/skatteetaten-tenor-bot/59ea64aad03d5adf953729793fcbde19/raw), lenket fra [Tenor-dokumentasjonens konfigurasjon](https://github.com/Skatteetaten/testnorge-tenor-dokumentasjon/blob/main/docusaurus.config.ts). Tester bruker syntetiske fixtures og avskjærer alle eksterne kall.
## Feilsøking av API-kall

Feilpanelet skiller mellom feil ved tokenhenting i Maskinporten og avviste kall til Altinn. For Maskinporten vises OAuth-feilkode, eventuell MP-kode, miljø, forespurt scope og tjenestens HTTP-status. HTTP 502 fra SmartCloud kan for eksempel skyldes HTTP 400 fra Maskinporten; begge vises for å gjøre dette tydelig.

Veiledningen er tilpasset kjente feilkoder, blant annet manglende scope, utløpt signeringsnøkkel og feil klientkonfigurasjon. Ukjente feil vises uten å gjette en bestemt årsak. Se [Digdirs feilsøkingsoversikt](https://docs.digdir.no/docs/Maskinporten/maskinporten_feilsoking) for kodene. Rå tokenresponser, signerte assertions og nøkler returneres ikke til nettleseren eller logges av denne feilhåndteringen.

Bruk sporings-ID fra feilpanelet til å finne samme hendelse i serverloggen. Etter publisering av endringer i feilhåndteringen må siden lastes på nytt og kallet gjentas for å få den nye diagnosen. En mislykket systemliste vises som «Kunne ikke hente systemer», ikke som en tom liste.
## Verifisering

```powershell
dotnet build SuperSystem.Server -p:BuildProjectReferences=false
dotnet test SmartCloud.Tests -p:BuildProjectReferences=false
cd supersystem.client
npm run build
npm run lint
npm run test:e2e
```

Bygg backend før test når `BuildProjectReferences=false` brukes. Kontrakttestene bruker en falsk HTTP-handler. Nettlesertestene avskjærer alle API-kall og krever eksplisitt mock for mutasjoner. På Windows brukes installert Edge; andre miljøer kan tilpasse Playwright-konfigurasjonen til Chromium.

## Kilder undersøkt

- `C:/repos/altinn-authentication/docs/flows/system-user.md`
- `altinn-authentication/src/Authentication/Controllers/`: SystemRegister-, RequestSystemUser-, ChangeRequestSystemUser- og SystemUserController.
- `altinn-authentication/src/Core/Models/`: register-, forespørsels-, systembruker- og tilgangspakkekontrakter.
- `C:/repos/tjenesteoversikten.no/src/AltinnServiceCatalogue/AltinnServiceCatalogue.Server/Services/MetadataClient.cs`
- [Opprette systembruker](https://docs.altinn.studio/nb/authorization/guides/system-vendor/system-user/systemuserrequest/)
- [Systemuser API](https://docs.altinn.studio/en/api/authentication/systemuserapi/systemuser/external/)
- [Klientdelegering for systemleverandører](https://docs.altinn.studio/en/authorization/guides/system-vendor/system-user/client-delegation/)
