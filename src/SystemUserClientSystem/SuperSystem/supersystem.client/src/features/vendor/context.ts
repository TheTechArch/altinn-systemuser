import { useOutletContext } from 'react-router-dom';
import { Configuration, RegisteredSystem, RegisteredSystemSummary } from './api';

export interface VendorContext {
  configuration: Configuration;
  system: RegisteredSystem;
  systemId: string;
  systems: RegisteredSystemSummary[];
  refreshSystems: () => void;
  refreshSystem: () => void;
}
export const useVendor = () => useOutletContext<VendorContext>();
