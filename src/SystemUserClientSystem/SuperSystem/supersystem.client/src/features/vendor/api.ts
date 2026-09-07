import { useEffect, useState } from 'react';

export interface Right { resource: { id: string; value: string }[]; action?: string }
export interface AccessPackage { urn: string }
export interface RegisteredSystem {
  id: string; name: Record<string, string>; description: Record<string, string>;
  rights: Right[] | null; accessPackages: AccessPackage[] | null;
  allowedRedirectUrls: string[]; clientId: string[]; isVisible: boolean;
  vendor: { id?: string; ID?: string };
}
export interface RegisteredSystemSummary { systemId: string; name: Record<string, string>; description: Record<string, string>; systemVendorOrgNumber: string; isVisible: boolean }
export interface Configuration {
  defaultSystemId: string; environment: string; defaultRedirectUrl?: string;
  presets: { id: string; name: string; resources: string[] }[];
}
export interface SystemUser {
  id: string; integrationTitle: string; systemId: string; productName: string;
  reporteeOrgNo: string; externalRef: string; userType: string; created: string;
  isDeleted: boolean; supplierName: string; supplierOrgno: string;
}
export type RequestKind = 'standard' | 'agent' | 'change';
export interface SystemRequest {
  id: string; systemId: string; systemUserId?: string; partyOrgNo: string;
  integrationTitle?: string; externalRef?: string; status: string;
  confirmUrl?: string; redirectUrl?: string; created?: string; timedOut: boolean; escalated?: boolean;
  rights?: Right[]; accessPackages?: AccessPackage[];
  requiredRights?: Right[]; unwantedRights?: Right[];
  requiredAccessPackages?: AccessPackage[]; unwantedAccessPackages?: AccessPackage[];
}
export interface PackageMetadata {
  id: string; name: string; urn: string; description: string;
  isDelegable: boolean; resources?: ResourceMetadata[];
}
export interface ResourceMetadata { id: string; name?: string; description?: string; refId?: string }
export interface PackageSearch { object: PackageMetadata }

export async function api<T>(url: string, init?: RequestInit): Promise<T> {
  const response = await fetch(url, { ...init, headers: { ...init?.headers, ...(init?.body ? { 'Content-Type': 'application/json' } : {}) } });
  const text = await response.text();
  let data: unknown;
  try { data = text ? JSON.parse(text) : null; }
  catch { throw new Error(`Uventet svar fra serveren (HTTP ${response.status}).`); }
  if (!response.ok) {
    const problem = data as { detail?: string; title?: string; errors?: unknown; validationErrors?: unknown; traceId?: string;
      service?: string; code?: string; providerCode?: string; environment?: string; scope?: string; upstreamStatus?: number } | null;
    throw new Error([problem?.title || 'Kallet feilet.', problem?.detail,
      problem?.service ? `Tjeneste: ${problem.service}` : '',
      problem?.code ? `Feilkode: ${problem.code}${problem.providerCode ? ` (${problem.providerCode})` : ''}` : '',
      problem?.environment ? `Miljø: ${problem.environment}` : '',
      problem?.scope ? `Forespurt scope: ${problem.scope}` : '',
      `HTTP ${response.status}${problem?.upstreamStatus ? ` · HTTP fra ${problem.service || 'tjenesten'}: ${problem.upstreamStatus}` : ''}`,
      problem?.errors ? JSON.stringify(problem.errors) : '', problem?.validationErrors ? JSON.stringify(problem.validationErrors) : '',
      problem?.traceId ? `Sporings-ID: ${problem.traceId}` : ''].filter(Boolean).join('\n'));
  }
  return data as T;
}

export function useLoad<T>(url: string | null) {
  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(!!url);
  const [version, setVersion] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setData(null); setError(''); setLoading(!!url);
    if (url) api<T>(url, { signal: controller.signal }).then(value => { if (!controller.signal.aborted) setData(value); }).catch((e: Error) => {
      if (!controller.signal.aborted) setError(e.message);
    }).finally(() => { if (!controller.signal.aborted) setLoading(false); });
    return () => controller.abort();
  }, [url, version]);
  return { data, error, loading, reload: () => setVersion(v => v + 1) };
}
export const localized = (value?: Record<string, string>) => value?.nb || value?.nn || value?.en || Object.values(value || {})[0] || '';
export const rightKey = (right: Right) => JSON.stringify([...right.resource].sort((a, b) => a.id.localeCompare(b.id) || a.value.localeCompare(b.value)));
export const rightLabel = (right: Right) => right.resource.map(a => a.value).join(' / ');
export const dateLabel = (value?: string) => value && !Number.isNaN(Date.parse(value)) ? new Date(value).toLocaleString('nb-NO') : 'Ikke oppgitt';
export const query = (system: string) => `?system=${encodeURIComponent(system)}`;
export function safeLink(value?: string) {
  if (!value) return undefined;
  try { const url = new URL(value); return url.protocol === 'https:' || url.protocol === 'http:' && ['localhost', '127.0.0.1', '[::1]'].includes(url.hostname) ? url.href : undefined; }
  catch { return undefined; }
}

export function rememberRequest(request: SystemRequest, kind: RequestKind) {
  try { sessionStorage.setItem('smartcloud.pendingRequest', JSON.stringify({ id: request.id, kind, systemId: request.systemId })); }
  catch { /* The status page remains available when browser storage is disabled. */ }
}
