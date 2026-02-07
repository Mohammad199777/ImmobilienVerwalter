"use client";
import { useEffect, useState, useCallback, useRef } from "react";
import { tenantsApi, TenantCreate } from "@/lib/api";
import { getErrorMessage } from "@/lib/useToast";
import { useAppToast } from "../layout";
import { Users, Plus, Mail, Phone, Edit, Trash2, X } from "lucide-react";

interface Tenant {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone?: string;
  mobilePhone?: string;
  iban?: string;
  bic?: string;
  bankName?: string;
  dateOfBirth?: string;
  occupation?: string;
  monthlyIncome?: number;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  notes?: string;
  activeLeases: number;
}

export default function TenantsPage() {
  const toast = useAppToast();
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState<Tenant | null>(null);
  const [search, setSearch] = useState("");
  const searchTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [debouncedSearch, setDebouncedSearch] = useState("");

  const [form, setForm] = useState<TenantCreate>({
    firstName: "",
    lastName: "",
    email: "",
  });

  // Debounce Suche
  useEffect(() => {
    if (searchTimeout.current) clearTimeout(searchTimeout.current);
    searchTimeout.current = setTimeout(() => setDebouncedSearch(search), 400);
    return () => {
      if (searchTimeout.current) clearTimeout(searchTimeout.current);
    };
  }, [search]);

  const loadTenants = useCallback(async () => {
    try {
      const res =
        debouncedSearch.length >= 2
          ? await tenantsApi.search(debouncedSearch)
          : await tenantsApi.getAll();
      setTenants(res.data);
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [debouncedSearch, toast]);

  useEffect(() => {
    loadTenants();
  }, [loadTenants]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      if (editing) {
        await tenantsApi.update(editing.id, form);
        toast("Mieter aktualisiert.", "success");
      } else {
        await tenantsApi.create(form);
        toast("Mieter erstellt.", "success");
      }
      setShowForm(false);
      setEditing(null);
      setForm({ firstName: "", lastName: "", email: "" });
      loadTenants();
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleEdit = (t: Tenant) => {
    setEditing(t);
    setForm({
      firstName: t.firstName,
      lastName: t.lastName,
      email: t.email,
      phone: t.phone,
      mobilePhone: t.mobilePhone,
      iban: t.iban,
      bic: t.bic,
      bankName: t.bankName,
      dateOfBirth: t.dateOfBirth?.split("T")[0],
      occupation: t.occupation,
      monthlyIncome: t.monthlyIncome,
      emergencyContactName: t.emergencyContactName,
      emergencyContactPhone: t.emergencyContactPhone,
      notes: t.notes,
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Mieter wirklich löschen?")) return;
    try {
      await tenantsApi.delete(id);
      toast("Mieter gelöscht.", "success");
      loadTenants();
    } catch (err) {
      toast(getErrorMessage(err));
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Mieter</h1>
        <button
          onClick={() => {
            setShowForm(true);
            setEditing(null);
            setForm({ firstName: "", lastName: "", email: "" });
          }}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          <Plus size={18} /> Neuer Mieter
        </button>
      </div>

      <div className="mb-6">
        <input
          type="text"
          placeholder="Mieter suchen (mind. 2 Zeichen)…"
          className="w-full max-w-md px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      {/* Form Modal */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">
                {editing ? "Mieter bearbeiten" : "Neuer Mieter"}
              </h2>
              <button
                onClick={() => {
                  setShowForm(false);
                  setEditing(null);
                }}
                aria-label="Schließen"
              >
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Vorname *
                  </label>
                  <input
                    required
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.firstName}
                    onChange={(e) =>
                      setForm({ ...form, firstName: e.target.value })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Nachname *
                  </label>
                  <input
                    required
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.lastName}
                    onChange={(e) =>
                      setForm({ ...form, lastName: e.target.value })
                    }
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  E-Mail *
                </label>
                <input
                  type="email"
                  required
                  className="w-full px-3 py-2 border rounded-lg"
                  value={form.email}
                  onChange={(e) => setForm({ ...form, email: e.target.value })}
                />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Telefon
                  </label>
                  <input
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.phone || ""}
                    onChange={(e) =>
                      setForm({ ...form, phone: e.target.value })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Mobil
                  </label>
                  <input
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.mobilePhone || ""}
                    onChange={(e) =>
                      setForm({ ...form, mobilePhone: e.target.value })
                    }
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Geburtsdatum
                  </label>
                  <input
                    type="date"
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.dateOfBirth || ""}
                    onChange={(e) =>
                      setForm({ ...form, dateOfBirth: e.target.value })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Beruf
                  </label>
                  <input
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.occupation || ""}
                    onChange={(e) =>
                      setForm({ ...form, occupation: e.target.value })
                    }
                  />
                </div>
              </div>

              {/* Bankdaten */}
              <fieldset className="border rounded-lg p-3">
                <legend className="text-sm font-medium text-gray-700 px-1">
                  Bankdaten
                </legend>
                <div className="space-y-3">
                  <div>
                    <label className="block text-sm text-gray-600 mb-1">
                      IBAN
                    </label>
                    <input
                      className="w-full px-3 py-2 border rounded-lg"
                      maxLength={34}
                      value={form.iban || ""}
                      onChange={(e) =>
                        setForm({ ...form, iban: e.target.value })
                      }
                    />
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-sm text-gray-600 mb-1">
                        BIC
                      </label>
                      <input
                        className="w-full px-3 py-2 border rounded-lg"
                        maxLength={11}
                        value={form.bic || ""}
                        onChange={(e) =>
                          setForm({ ...form, bic: e.target.value })
                        }
                      />
                    </div>
                    <div>
                      <label className="block text-sm text-gray-600 mb-1">
                        Bankname
                      </label>
                      <input
                        className="w-full px-3 py-2 border rounded-lg"
                        value={form.bankName || ""}
                        onChange={(e) =>
                          setForm({ ...form, bankName: e.target.value })
                        }
                      />
                    </div>
                  </div>
                </div>
              </fieldset>

              {/* Finanzen & Notfall */}
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Monatliches Einkommen
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    min={0}
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.monthlyIncome || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        monthlyIncome: parseFloat(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Notfallkontakt Name
                  </label>
                  <input
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.emergencyContactName || ""}
                    onChange={(e) =>
                      setForm({ ...form, emergencyContactName: e.target.value })
                    }
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notfallkontakt Telefon
                </label>
                <input
                  className="w-full px-3 py-2 border rounded-lg"
                  value={form.emergencyContactPhone || ""}
                  onChange={(e) =>
                    setForm({ ...form, emergencyContactPhone: e.target.value })
                  }
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notizen
                </label>
                <textarea
                  className="w-full px-3 py-2 border rounded-lg"
                  rows={3}
                  value={form.notes || ""}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                />
              </div>
              <button
                type="submit"
                disabled={submitting}
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium disabled:opacity-50"
              >
                {submitting
                  ? "Speichern…"
                  : editing
                    ? "Speichern"
                    : "Erstellen"}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Tenants List */}
      {tenants.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Users size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">
            {search ? "Keine Mieter gefunden." : "Noch keine Mieter vorhanden."}
          </p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border overflow-hidden">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Name
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Kontakt
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Beruf
                </th>
                <th className="text-center px-6 py-3 text-sm font-medium text-gray-500">
                  Aktive Verträge
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Aktionen
                </th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {tenants.map((t) => (
                <tr key={t.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-900">
                    {t.fullName}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    <div className="flex items-center gap-1">
                      <Mail size={14} /> {t.email}
                    </div>
                    {t.phone && (
                      <div className="flex items-center gap-1 mt-1">
                        <Phone size={14} /> {t.phone}
                      </div>
                    )}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {t.occupation || "–"}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${t.activeLeases > 0 ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-500"}`}
                    >
                      {t.activeLeases}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => handleEdit(t)}
                        className="text-gray-400 hover:text-blue-600"
                        aria-label="Bearbeiten"
                      >
                        <Edit size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(t.id)}
                        className="text-gray-400 hover:text-red-600"
                        aria-label="Löschen"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
