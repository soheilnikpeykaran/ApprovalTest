import { FormEvent, useEffect, useState } from 'react';
import { api } from '../services/api';
import type { FormField, FormSchema } from '../types';

export function DynamicRequestForm({
  onCreated,
}: {
  onCreated: () => void;
}) {
  const [schema, setSchema] = useState<FormSchema | null>(null);
  const [values, setValues] = useState<Record<string, unknown>>({});
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    api
      .getSchema()
      .then(setSchema)
      .catch((e) => {
        setError(
          e instanceof Error
            ? e.message
            : 'خطا در دریافت فرم'
        );
      });
  }, []);

  function setValue(key: string, value: unknown) {
    setValues((current) => ({
      ...current,
      [key]: value,
    }));
  }

  async function submit(e: FormEvent) {
    e.preventDefault();

    if (!schema) {
      return;
    }

    setError('');

    const missing = schema.fields.filter(
      (field) =>
        field.required &&
        (values[field.key] === undefined ||
          values[field.key] === '')
    );

    if (missing.length > 0) {
      setError(
        `فیلدهای الزامی: ${missing
          .map((field) => field.label)
          .join('، ')}`
      );

      return;
    }

    setLoading(true);

    try {
      await api.createRequest(values);

      setValues({});
      onCreated();
    } catch (e) {
      setError(
        e instanceof Error
          ? e.message
          : 'خطا در ثبت درخواست'
      );
    } finally {
      setLoading(false);
    }
  }

  if (!schema) {
    return (
      <div className="card">
        {error || 'در حال دریافت فرم...'}
      </div>
    );
  }

  return (
    <div className="card">
      <h2>ثبت درخواست جدید</h2>

      <form onSubmit={submit}>
        {schema.fields.map((field: FormField) => (
          <label key={field.key}>
            {field.label}

            {field.type === 'textarea' ? (
              <textarea
                value={String(values[field.key] ?? '')}
                onChange={(e) =>
                  setValue(field.key, e.target.value)
                }
                required={field.required}
              />
            ) : field.type === 'select' ? (
              <select
                value={String(values[field.key] ?? '')}
                onChange={(e) =>
                  setValue(field.key, e.target.value)
                }
                required={field.required}
              >
                <option value="">
                  انتخاب کنید
                </option>

                {field.options?.map((option) => (
                  <option key={option} value={option}>
                    {option}
                  </option>
                ))}
              </select>
            ) : (
              <input
                type={
                  field.type === 'number'
                    ? 'number'
                    : 'text'
                }
                value={String(values[field.key] ?? '')}
                onChange={(e) =>
                  setValue(
                    field.key,
                    field.type === 'number'
                      ? e.target.value === ''
                        ? undefined
                        : Number(e.target.value)
                      : e.target.value
                  )
                }
                required={field.required}
              />
            )}
          </label>
        ))}

        {error && (
          <p className="error">
            {error}
          </p>
        )}

        <button disabled={loading}>
          {loading
            ? 'در حال ارسال...'
            : 'ثبت درخواست'}
        </button>
      </form>
    </div>
  );
}