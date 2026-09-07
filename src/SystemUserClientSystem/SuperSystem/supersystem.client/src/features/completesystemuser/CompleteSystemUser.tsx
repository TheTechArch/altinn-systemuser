import { Navigate, useSearchParams } from 'react-router-dom';
export function CompleteSystemUser() {
  const [params] = useSearchParams();
  return <Navigate replace to={`/vendor/new?${params.toString()}`} />;
}
