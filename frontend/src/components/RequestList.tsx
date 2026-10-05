import { useEffect, useState } from 'react';
import { api } from '../services/api';
import type { RequestItem, RequestStatus } from '../types';

const statusLabels: Record<RequestStatus, string> = {
  0: 'در انتظار بررسی',
  1: 'تأیید شده',
  2: 'رد شده',
};

export function RequestList({
  roles,
  refreshKey,
}: {
  roles: string[];
  refreshKey: number;
}) {
  const [items, setItems] = useState<RequestItem[]>([]);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState<string | null>(null);

  const load = () => {
    setError('');

    api.getRequests()
      .then(setItems)
      .catch((e) => {
        setError(e instanceof Error ? e.message : 'خطا در دریافت درخواست‌ها');
      });
  };

  useEffect(() => {
    load();
  }, [refreshKey]);

  const canDecide =
    roles.includes('Manager') || roles.includes('Finance');

  async function decide(
    id: string,
    action: 'approve' | 'reject'
  ) {
    setBusy(id);
    setError('');

    try {
      await api.decide(id, action);
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'خطا در تصمیم‌گیری');
    } finally {
      setBusy(null);
    }
  }

  return (
    <div className="card">
      <div className="row">
        <h2>درخواست‌ها</h2>

        <button onClick={load}>
          بروزرسانی
        </button>
      </div>

      {error && (
        <p className="error">
          {error}
        </p>
      )}

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>عنوان</th>
              <th>مبلغ</th>
              <th>فوریت</th>
              <th>وضعیت</th>
              <th>مسئول</th>
              <th>عملیات</th>
            </tr>
          </thead>

          <tbody>
            {items.map((x) => (
              <tr key={x.id}>
                <td>{x.title}</td>

                <td>
                  {x.amount.toLocaleString('fa-IR')}
                </td>

                <td>
                  {x.urgency}
                </td>

                <td>
                  {statusLabels[x.status]}
                </td>

                <td>
                  {x.assignedRole}
                </td>

                <td>
                  {canDecide &&
                  x.status === 0 &&
                  roles.includes(x.assignedRole) ? (
                    <span>
                      <button
                        disabled={busy === x.id}
                        onClick={() => decide(x.id, 'approve')}
                      >
                        تأیید
                      </button>

                      {' '}

                      <button
                        disabled={busy === x.id}
                        onClick={() => decide(x.id, 'reject')}
                      >
                        رد
                      </button>
                    </span>
                  ) : (
                    '-'
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}