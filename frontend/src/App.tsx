import { useState } from 'react';
import { Login } from './components/Login';
import { DynamicRequestForm } from './components/DynamicRequestForm';
import { RequestList } from './components/RequestList';
import type { AuthResponse } from './types';
import './styles.css';

function App() {
  const [refreshKey, setRefreshKey] = useState(0);

  const [auth, setAuth] = useState<AuthResponse | null>(() => {
    const token = localStorage.getItem('access_token');
    const roles = JSON.parse(localStorage.getItem('roles') ?? '[]');

    return token
      ? {
          token,
          roles,
          userId: '',
          expiresAt: '',
        }
      : null;
  });

  if (!auth) {
    return (
      <main>
        <Login onLogin={setAuth} />
      </main>
    );
  }

  const isEmployee = auth.roles.includes('Employee');

  function logout() {
    localStorage.clear();
    setAuth(null);
  }

  return (
    <main>
      <header className="topbar">
        <div>
          <strong>سامانه گردش درخواست</strong>
        </div>

        <div className="user-info">
          <span>
            نقش: {auth.roles.join(' / ')}
          </span>

          <button onClick={logout}>
            خروج
          </button>
        </div>
      </header>

      {isEmployee && (
        <DynamicRequestForm
          onCreated={() => setRefreshKey((x) => x + 1)}
        />
      )}

      <RequestList
        roles={auth.roles}
        refreshKey={refreshKey}
      />
    </main>
  );
}

export default App;