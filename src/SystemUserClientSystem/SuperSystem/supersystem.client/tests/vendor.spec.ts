import { expect, test, Page } from '@playwright/test';

const systemId = '991825827_smartcloud';
const packageUrn = 'urn:altinn:accesspackage:skattegrunnlag';
const requestId = '4aa291ee-1b92-4fdd-b53e-95eadfeac3af';
const userId = '60a291ee-1b92-4fdd-b53e-95eadfeac3af';
const system = {
  id: systemId, vendor: { ID: '0192:991825827' },
  name: { nb: 'SmartCloud', en: 'SmartCloud English', de: 'Wolke' },
  description: { nb: 'Testsystem', en: 'English description' },
  rights: [{ resource: [{ id: 'urn:altinn:resource', value: 'ske-krav-og-betalinger' }] }],
  accessPackages: [{ urn: packageUrn }], clientId: ['existing-client'],
  allowedRedirectUrls: ['https://example.test/receipt'], isVisible: true,
};
const request = { id: requestId, systemId, partyOrgNo: '123456789', status: 'New', integrationTitle: 'Lønn',
  externalRef: 'tenant-1', accessPackages: [{ urn: packageUrn }], rights: [], timedOut: false,
  confirmUrl: 'https://am.ui.tt02.altinn.no/confirm', created: '2026-09-01T12:00:00Z' };
const users = [
  { id: userId, systemId, integrationTitle: 'Aktiv kunde', reporteeOrgNo: '123456789', externalRef: 'tenant-1', userType: 'standard', isDeleted: false, created: '2026-09-01T12:00:00Z' },
  { id: '60a291ee-1b92-4fdd-b53e-95eadfeac3b00', systemId, integrationTitle: 'Slettet kunde', reporteeOrgNo: '987654321', externalRef: 'tenant-2', userType: 'agent', isDeleted: true, created: '2026-09-01T12:00:00Z' },
];

async function mockApi(page: Page) {
  await page.route('**/api/**', async route => {
    const url = new URL(route.request().url());
    const pathname = url.pathname;
    let data: unknown;
    if (pathname === '/api/testdata/configuration') data = { enabled: true };
    else if (pathname === '/api/vendor/configuration') data = { defaultSystemId: systemId, environment: 'platform.tt02.altinn.no', presets: [{ id: 'basic', name: 'SmartBasic', resources: ['ske-krav-og-betalinger'] }] };
    else if (pathname === '/api/vendor/systems') data = [{ ...system, id: undefined, systemId }, { ...system, id: undefined, systemId: '991825827_second', name: { nb: 'Annet system' } }];
    else if (pathname === '/api/vendor/systems/' + systemId) data = system;
    else if (pathname === '/api/vendor/systems/991825827_second') data = { ...system, id: '991825827_second', name: { nb: 'Annet system' } };
    else if (pathname.endsWith('/users')) data = users;
    else if (new RegExp("/requests/(standard|agent|change)$").test(pathname)) data = [request];
    else if (pathname.includes('/api/vendor/requests/')) data = request;
    else if (pathname === '/api/metadata/packages') data = [{ object: { id: '74bb3697-322e-4c07-a525-8391b8146ee8', name: 'Skattegrunnlag', urn: packageUrn, description: 'Tjenester for skatt', isDelegable: true } }];
    else if (pathname === '/api/metadata/packages/by-urn') data = { id: '74bb3697-322e-4c07-a525-8391b8146ee8', name: 'Skattegrunnlag', urn: packageUrn, description: 'Tjenester for skatt', isDelegable: true };
    else if (new RegExp("/packages/.+/resources$").test(pathname)) data = [{ id: 'tax', name: 'Skattemelding' }];
    else if (pathname === '/api/metadata/resources') data = { total: 1, data: [{ identifier: 'ske-krav-og-betalinger', title: { nb: 'Krav og betalinger' }, description: { nb: 'Økonomidata' } }] };
    else return route.fulfill({ status: 500, json: { detail: 'Uventet testkall: ' + pathname } });
    if (route.request().method() !== 'GET') return route.fulfill({ status: 500, json: { detail: 'Mutasjoner må fanges eksplisitt i testen.' } });
    await route.fulfill({ json: data });
  });
}

test.beforeEach(async ({ page }) => { await mockApi(page); });

test('lister alle systemer og bytter aktivt system', async ({ page }) => {
  await page.goto('/vendor/systems');
  await expect(page.getByRole('heading', { name: 'Leverandørens systemer' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Annet system' })).toBeVisible();
  await page.screenshot({ path: 'test-results/vendor-systems.png', fullPage: true });
  await page.getByRole('link', { name: 'Åpne system' }).nth(1).click();
  await expect(page.getByRole('heading', { level: 1, name: 'Annet system' })).toBeVisible();
  await expect(page).toHaveURL(/system=991825827_second/);
});

test('vanlig forespørsel sender valgte rettigheter og pakker med brukerens referanse', async ({ page }) => {
  let posted: Record<string, unknown> | undefined;
  await page.route('**/api/vendor/requests/standard', route => {
    posted = route.request().postDataJSON();
    return route.fulfill({ json: { ...request, ...posted } });
  });
  await page.goto('/vendor/new');
  await page.getByLabel('Organisasjonsnummer', { exact: true }).fill('123456789');
  await page.getByLabel('Navn på systembrukeren (valgfritt)').fill('SmartCloud Lønn');
  await page.getByLabel('Ekstern referanse (valgfritt)').fill('tenant-42');
  await page.getByRole('checkbox', { name: 'ske-krav-og-betalinger', exact: true }).check();
  await page.getByRole('checkbox', { name: packageUrn, exact: true }).check();
  await page.screenshot({ path: 'test-results/vendor-request.png', fullPage: true });
  await page.getByRole('button', { name: 'Opprett forespørsel', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Forespørsel hos Altinn' })).toBeVisible();
  expect(posted).toMatchObject({ systemId, partyOrgNo: '123456789', integrationTitle: 'SmartCloud Lønn', externalRef: 'tenant-42',
    rights: system.rights, accessPackages: [{ urn: packageUrn }] });
  await expect(page.getByRole('link', { name: 'Åpne godkjenning i Altinn' })).toBeVisible();
});

test('agentskifte fjerner enkeltrettigheter og sender bare pakker', async ({ page }) => {
  let posted: Record<string, unknown> | undefined;
  await page.route('**/api/vendor/requests/agent', route => {
    posted = route.request().postDataJSON();
    return route.fulfill({ json: { ...request, ...posted } });
  });
  await page.goto('/vendor/new');
  await page.getByRole('checkbox', { name: 'ske-krav-og-betalinger', exact: true }).check();
  await page.getByLabel('Systembrukertype').selectOption('agent');
  await expect(page.getByRole('checkbox', { name: 'ske-krav-og-betalinger', exact: true })).toHaveCount(0);
  await page.getByLabel('Organisasjonsnummer', { exact: true }).fill('123456789');
  await page.getByRole('checkbox', { name: packageUrn, exact: true }).check();
  await page.screenshot({ path: 'test-results/vendor-request.png', fullPage: true });
  await page.getByRole('button', { name: 'Opprett forespørsel', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Forespørsel hos Altinn' })).toBeVisible();
  expect(posted).not.toHaveProperty('rights');
  expect(posted).toMatchObject({ accessPackages: [{ urn: packageUrn }], externalRef: null });
});

test('oppretter et nytt system med pakker og enkeltrettigheter', async ({ page }) => {
  let posted: Record<string, unknown> | undefined;
  await page.route('**/api/vendor/systems', async route => {
    if (route.request().method() === 'GET') return route.fallback();
    posted = route.request().postDataJSON();
    await route.fulfill({ json: requestId });
  });
  await page.goto('/vendor/systems/new');
  await page.getByLabel('System-ID', { exact: true }).fill('991825827_new');
  await page.getByLabel('Systemnavn (nb)').fill('Nytt system');
  await page.getByLabel('Maskinporten-klient-ID-er').fill('new-client');
  await page.getByLabel('Tillatte returadresser').fill('https://example.test/receipt');
  await page.getByRole('checkbox', { name: 'Velg tilgangspakke', exact: true }).check();
  await page.getByRole('button', { name: 'Enkeltressurser', exact: true }).click();
  await page.getByRole('checkbox', { name: 'Velg enkeltrettighet', exact: true }).check();
  await page.getByRole('button', { name: 'Registrer system i Altinn' }).click();
  await expect.poll(() => posted).toBeTruthy();
  expect(posted).toMatchObject({ id: '991825827_new', vendor: { ID: '0192:991825827' }, name: { nb: 'Nytt system' },
    clientId: ['new-client'], rights: system.rights, accessPackages: [{ urn: packageUrn }], allowedRedirectUrls: ['https://example.test/receipt'] });
});

test('redigering bevarer språk, klienter og returadresser', async ({ page }) => {
  let posted: Record<string, unknown> | undefined;
  await page.route('**/api/vendor/systems/' + systemId, async route => {
    if (route.request().method() === 'GET') return route.fallback();
    posted = route.request().postDataJSON();
    await route.fulfill({ json: { success: true } });
  });
  await page.goto('/vendor/settings');
  await page.getByLabel('Systemnavn (nb)').fill('SmartCloud oppdatert');
  await page.getByLabel('Synlig i Altinns systemregister').uncheck();
  await page.getByRole('button', { name: 'Lagre system i Altinn' }).click();
  await expect.poll(() => posted).toBeTruthy();
  expect(posted).toMatchObject({ name: { nb: 'SmartCloud oppdatert', en: 'SmartCloud English', de: 'Wolke' },
    clientId: ['existing-client'], allowedRedirectUrls: ['https://example.test/receipt'], isVisible: false });
});

test('systembrukerliste filtrerer og viser korrekt detaljside', async ({ page }) => {
  await page.goto('/vendor/systemusers');
  await expect(page.getByRole('link', { name: 'Aktiv kunde' })).toBeVisible();
  await expect(page.getByRole('link', { name: 'Slettet kunde' })).toHaveCount(0);
  await page.getByLabel('Status', { exact: true }).selectOption('all');
  await expect(page.getByRole('link', { name: 'Slettet kunde' })).toBeVisible();
  await page.getByRole('link', { name: 'Aktiv kunde' }).click();
  await expect(page.getByRole('heading', { name: 'Identitet i Altinn' })).toBeVisible();
  await expect(page.getByText('Gjeldende delegeringer følger ikke med i svaret.', { exact: false })).toBeVisible();
});

test('API-feil vises og gir mulighet for nytt forsøk', async ({ page }) => {
  await page.route('**/api/vendor/requests/agent', route => route.fulfill({ status: 400, json: { detail: 'Pakken er ikke registrert på systemet.' } }));
  await page.goto('/vendor/new');
  await page.getByLabel('Systembrukertype').selectOption('agent');
  await page.getByLabel('Organisasjonsnummer', { exact: true }).fill('123456789');
  await page.getByRole('checkbox', { name: packageUrn, exact: true }).check();
  await page.screenshot({ path: 'test-results/vendor-request.png', fullPage: true });
  await page.getByRole('button', { name: 'Opprett forespørsel', exact: true }).click();
  await expect(page.getByRole('alert')).toContainText('Pakken er ikke registrert på systemet.');
  await expect(page.getByRole('button', { name: 'Opprett forespørsel', exact: true })).toBeEnabled();
});

test('kvittering viser avvisning fra API-et', async ({ page }) => {
  await page.addInitScript(({ requestId, systemId }) => sessionStorage.setItem('smartcloud.pendingRequest', JSON.stringify({ id: requestId, systemId, kind: 'standard' })), { requestId, systemId });
  await page.route('**/api/vendor/requests/standard/' + requestId, route => route.fulfill({ json: { ...request, status: 'Rejected' } }));
  await page.goto('/receipt');
  await expect(page.getByText('Avvist', { exact: true })).toBeVisible();
  await expect(page.getByText('Da var systemtilgang i boks!')).toHaveCount(0);
});

test('pakkeinnhold og mobilvisning', async ({ page }) => {
  await page.setViewportSize({ width: 390, height: 844 });
  await page.goto('/vendor/new');
  await page.getByRole('button', { name: 'Se innhold', exact: true }).click();
  await expect(page.getByText('Skattemelding', { exact: true })).toBeVisible();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
});



test('første system kan registreres når leverandørlisten er tom', async ({ page }) => {
  await page.route('**/api/vendor/systems', route => route.fulfill({ json: [] }));
  await page.goto('/vendor/systems');
  await expect(page.getByText('Ingen systemer er registrert ennå.', { exact: false })).toBeVisible();
  await page.getByRole('link', { name: 'Registrer nytt system' }).click();
  await expect(page.getByLabel('System-ID', { exact: true })).toHaveValue('991825827_');
  await expect(page.getByRole('button', { name: 'Registrer system i Altinn' })).toBeVisible();
});

test('systemlistefeil viser Maskinporten-diagnose uten å påstå at listen er tom', async ({ page }) => {
  await page.route('**/api/vendor/systems', route => route.fulfill({ status: 502, json: {
    title: 'Kunne ikke hente tilgangstoken fra Maskinporten',
    detail: 'Maskinporten avviste forespurt scope. Kontroller at scopet er registrert på integrasjonen.',
    service: 'Maskinporten', code: 'invalid_scope', providerCode: 'MP-200', environment: 'test',
    scope: 'altinn:authentication/systemregister.write', upstreamStatus: 400, traceId: 'diagnostic-trace',
  } }));
  await page.goto('/vendor/systems');
  const alert = page.getByRole('alert');
  await expect(alert).toContainText('Kontroller at scopet er registrert');
  await expect(alert).toContainText('invalid_scope (MP-200)');
  await expect(alert).toContainText('Forespurt scope: altinn:authentication/systemregister.write');
  await expect(alert).toContainText('HTTP fra Maskinporten: 400');
  await expect(alert).toContainText('diagnostic-trace');
  await expect(page.getByLabel('Registrert system')).toBeDisabled();
  await expect(page.getByLabel('Registrert system')).toContainText('Kunne ikke hente systemer');
  await expect(page.getByText('Ingen systemer', { exact: true })).toHaveCount(0);
  await page.screenshot({ path: 'test-results/vendor-error-diagnostics.png', fullPage: true });
  await page.unroute('**/api/vendor/systems');
  await page.getByRole('button', { name: 'Prøv igjen' }).click();
  await expect(page.getByRole('heading', { name: 'Leverandørens systemer' })).toBeVisible();
});

const tenorOrganisation = { organisationNumber: '123456789', name: 'Syntetisk virksomhet', people: [
  { nationalIdentityNumber: '12345678901', name: 'Syntetisk Testperson', roleCode: 'DAGL', roleName: 'Daglig leder' },
], warning: null };

async function mockTenor(page: Page) {
  await page.route('**/api/testdata/organisations?*', route => route.fulfill({ json: { organisations: [tenorOrganisation], hasMore: false } }));
  await page.route('**/api/testdata/organisations/123456789', route => route.fulfill({ json: tenorOrganisation }));
}

async function selectTenorOrganisation(page: Page) {
  await page.getByText('Finn testvirksomhet i Tenor', { exact: true }).click();
  await page.getByLabel('Virksomhetsnavn eller organisasjonsnummer').fill('Syntetisk');
  await page.getByRole('button', { name: 'Søk i Tenor' }).click();
  await page.getByRole('button', { name: 'Vis testperson for Syntetisk virksomhet' }).click();
  await expect(page.getByText('Daglig leder: Syntetisk Testperson')).toBeVisible();
  await page.getByRole('button', { name: 'Bruk virksomheten' }).click();
}

test('Tenor-valg fyller organisasjon og viser daglig leder ved godkjenningslenken', async ({ page, context }) => {
  await context.grantPermissions(['clipboard-read', 'clipboard-write']);
  await mockTenor(page);
  let posted: Record<string, unknown> | undefined;
  await page.route('**/api/vendor/requests/standard', route => {
    posted = route.request().postDataJSON();
    return route.fulfill({ json: { ...request, ...posted } });
  });
  await page.goto('/vendor/new');
  await selectTenorOrganisation(page);
  await expect(page.getByLabel('Organisasjonsnummer', { exact: true })).toHaveValue('123456789');
  await page.getByRole('checkbox', { name: packageUrn, exact: true }).check();
  await page.getByRole('button', { name: 'Opprett forespørsel', exact: true }).click();
  await expect(page.getByRole('link', { name: 'Åpne godkjenning i Altinn' })).toBeVisible();
  await expect(page.getByText('Daglig leder: Syntetisk Testperson')).toBeVisible();
  await page.getByRole('button', { name: 'Kopier fødselsnummer' }).click();
  await expect(page.getByText('Syntetisk fødselsnummer kopiert.')).toBeVisible();
  expect(await page.evaluate(() => navigator.clipboard.readText())).toBe('12345678901');
  expect(posted?.partyOrgNo).toBe('123456789');
  expect(JSON.stringify(posted)).not.toContain('12345678901');
  await page.screenshot({ path: 'test-results/tenor-confirmation.png', fullPage: true });
});

test('endret organisasjonsnummer fjerner valgt Tenor-testperson', async ({ page }) => {
  await mockTenor(page);
  await page.goto('/vendor/new');
  await selectTenorOrganisation(page);
  await page.getByText('Finn testvirksomhet i Tenor', { exact: true }).click();
  await page.getByLabel('Organisasjonsnummer', { exact: true }).fill('987654321');
  await expect(page.getByText('Daglig leder: Syntetisk Testperson')).not.toBeVisible();
});

test('Tenor-feil blokkerer ikke manuell agentforespørsel', async ({ page }) => {
  await page.route('**/api/testdata/organisations?*', route => route.fulfill({ status: 502, json: {
    title: 'Kunne ikke hente testdata fra Tenor', detail: 'Kontroller tilgang til skatteetaten:testnorge/testdata.read', service: 'Tenor',
  } }));
  await page.route('**/api/vendor/requests/agent', route => route.fulfill({ json: { ...request, ...route.request().postDataJSON() } }));
  await page.goto('/vendor/new');
  await page.getByText('Finn testvirksomhet i Tenor', { exact: true }).click();
  await page.getByLabel('Virksomhetsnavn eller organisasjonsnummer').fill('Syntetisk');
  await page.getByRole('button', { name: 'Søk i Tenor' }).click();
  await expect(page.getByRole('alert')).toContainText('skatteetaten:testnorge/testdata.read');
  await page.getByLabel('Systembrukertype').selectOption('agent');
  await page.getByLabel('Organisasjonsnummer', { exact: true }).fill('123456789');
  await page.getByRole('checkbox', { name: packageUrn, exact: true }).check();
  await page.getByRole('button', { name: 'Opprett forespørsel', exact: true }).click();
  await expect(page.getByRole('link', { name: 'Åpne godkjenning i Altinn' })).toBeVisible();
});

test('søker etter enkeltpersonforetak uten navn og velger innehaver', async ({ page }) => {
  let searchUrl: URL | undefined;
  await mockTenor(page);
  await page.route('**/api/testdata/organisations?*', route => {
    searchUrl = new URL(route.request().url());
    return route.fulfill({ json: { organisations: [tenorOrganisation], hasMore: true } });
  });
  await page.route('**/api/testdata/organisations/123456789', route => route.fulfill({ json: {
    ...tenorOrganisation, people: [{ ...tenorOrganisation.people[0], roleCode: 'INNH', roleName: 'Innehaver' }],
  } }));
  await page.goto('/vendor/new');
  await page.getByText('Finn testvirksomhet i Tenor', { exact: true }).click();
  await expect(page.getByRole('button', { name: 'Søk i Tenor' })).toBeDisabled();
  await page.getByLabel('Organisasjonsform', { exact: true }).selectOption('ENK');
  await expect(page.getByLabel('Virksomhetsnavn eller organisasjonsnummer')).toHaveValue('');
  await page.getByRole('button', { name: 'Søk i Tenor' }).click();
  await page.getByRole('button', { name: 'Vis testperson for Syntetisk virksomhet' }).click();
  expect(searchUrl?.searchParams.get('organisationForm')).toBe('ENK');
  expect(searchUrl?.searchParams.has('term')).toBe(false);
  await expect(page.getByText('Innehaver: Syntetisk Testperson')).toBeVisible();
  await page.getByRole('button', { name: 'Bruk virksomheten' }).click();
  await expect(page.getByLabel('Organisasjonsnummer', { exact: true })).toHaveValue('123456789');
  await page.screenshot({ path: 'test-results/tenor-organisation-form.png', fullPage: true });
});

test('kombinerer navn og organisasjonsform og kan fjerne formfilteret', async ({ page }) => {
  const searches: URL[] = [];
  await page.route('**/api/testdata/organisations?*', route => {
    searches.push(new URL(route.request().url()));
    return route.fulfill({ json: { organisations: [], hasMore: false } });
  });
  await page.goto('/vendor/new');
  await page.getByText('Finn testvirksomhet i Tenor', { exact: true }).click();
  await page.getByLabel('Organisasjonsform', { exact: true }).selectOption('AS');
  await page.getByLabel('Virksomhetsnavn eller organisasjonsnummer').fill('Syntetisk');
  await page.getByRole('button', { name: 'Søk i Tenor' }).click();
  await expect(page.getByText('0 virksomheter vist.')).toBeVisible();
  expect(searches[0].searchParams.get('term')).toBe('Syntetisk');
  expect(searches[0].searchParams.get('organisationForm')).toBe('AS');
  await page.getByLabel('Organisasjonsform', { exact: true }).selectOption('');
  await page.getByRole('button', { name: 'Søk i Tenor' }).click();
  await expect.poll(() => searches.length).toBe(2);
  expect(searches[1].searchParams.has('organisationForm')).toBe(false);
});
