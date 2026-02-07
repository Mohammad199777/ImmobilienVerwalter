"use client";
import { useEffect, useState, useCallback } from "react";
import { paymentsApi, leasesApi } from "@/lib/api";
import { Euro, Plus, Trash2, X } from "lucide-react";

interface Payment {
  id: string;
  tenantName: string;
  amount: number;
  paymentDate: string;
  dueDate: string;
  paymentMonth: number;
  paymentYear: number;
  type: number;
  method: number;
  status: number;
  reference?: string;
  notes?: string;
}

interface LeaseOption {
  id: string;
  tenantName: string;
  totalRent: number;
}

const statusLabels = [
  "Eingegangen",
  "Ausstehend",
  "Überfällig",
  "Teilzahlung",
  "Storniert",
];
const statusColors = [
  "bg-green-100 text-green-700",
  "bg-yellow-100 text-yellow-700",
  "bg-red-100 text-red-700",
  "bg-orange-100 text-orange-700",
  "bg-gray-100 text-gray-500",
];
const typeLabels = ["Miete", "Nebenkosten", "Kaution", "Sonstige"];
const methodLabels = [
  "Überweisung",
  "Lastschrift",
  "Bar",
  "PayPal",
  "Sonstige",
];
const monthNames = [
  "Jan",
  "Feb",
  "Mär",
  "Apr",
  "Mai",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Okt",
  "Nov",
  "Dez",
];

const emptyForm = {
  leaseId: "",
  amount: 0,
  paymentDate: new Date().toISOString().split("T")[0],
  dueDate: new Date().toISOString().split("T")[0],
  paymentMonth: new Date().getMonth() + 1,
  paymentYear: new Date().getFullYear(),
  type: 0,
  method: 0,
  status: 0,
  reference: "",
  notes: "",
};

export default function PaymentsPage() {
  const [payments, setPayments] = useState<Payment[]>([]);
  const [activeLeases, setActiveLeases] = useState<LeaseOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const now = new Date();
  const [year, setYear] = useState(now.getFullYear());
  const [month, setMonth] = useState(now.getMonth() + 1);

  const load = useCallback(() => {
    setLoading(true);
    paymentsApi
      .getByMonth(year, month)
      .then((res) => setPayments(res.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [year, month]);

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    load();
  }, [year, month]);

  const openCreate = async () => {
    try {
      const res = await leasesApi.getActive();
      setActiveLeases(res.data);
    } catch (e) {
      console.error(e);
    }
    setForm({ ...emptyForm, paymentMonth: month, paymentYear: year });
    setShowCreate(true);
  };

  const handleCreate = async () => {
    try {
      await paymentsApi.create({
        leaseId: form.leaseId,
        amount: form.amount,
        paymentDate: form.paymentDate,
        dueDate: form.dueDate,
        paymentMonth: form.paymentMonth,
        paymentYear: form.paymentYear,
        type: form.type,
        method: form.method,
        status: form.status,
        reference: form.reference || undefined,
        notes: form.notes || undefined,
      });
      setShowCreate(false);
      load();
    } catch (e) {
      console.error(e);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Zahlung wirklich löschen?")) return;
    try {
      await paymentsApi.delete(id);
      load();
    } catch (e) {
      console.error(e);
    }
  };

  const fmt = (n: number) =>
    new Intl.NumberFormat("de-DE", {
      style: "currency",
      currency: "EUR",
    }).format(n);

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Zahlungen</h1>
        <button
          onClick={openCreate}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-blue-700"
        >
          <Plus size={18} /> Neue Zahlung
        </button>
      </div>

      <div className="flex gap-3 mb-6">
        <select
          className="px-3 py-2 border rounded-lg"
          value={month}
          onChange={(e) => setMonth(parseInt(e.target.value))}
        >
          {monthNames.map((m, i) => (
            <option key={i} value={i + 1}>
              {m}
            </option>
          ))}
        </select>
        <select
          className="px-3 py-2 border rounded-lg"
          value={year}
          onChange={(e) => setYear(parseInt(e.target.value))}
        >
          {[2024, 2025, 2026, 2027].map((y) => (
            <option key={y} value={y}>
              {y}
            </option>
          ))}
        </select>
      </div>

      {loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : payments.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Euro size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Keine Zahlungen in diesem Monat.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Mieter
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Betrag
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Datum
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Verwendungszweck
                </th>
                <th className="text-center px-6 py-3 text-sm font-medium text-gray-500">
                  Status
                </th>
                <th className="text-center px-6 py-3 text-sm font-medium text-gray-500">
                  Aktionen
                </th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {payments.map((p) => (
                <tr key={p.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-900">
                    {p.tenantName}
                  </td>
                  <td className="px-6 py-4 text-right font-medium text-green-600">
                    {fmt(p.amount)}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {new Date(p.paymentDate).toLocaleDateString("de-DE")}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {p.reference || "–"}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[p.status]}`}
                    >
                      {statusLabels[p.status]}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-center">
                    <button
                      onClick={() => handleDelete(p.id)}
                      className="text-red-500 hover:text-red-700"
                      title="Löschen"
                    >
                      <Trash2 size={16} />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <div className="px-6 py-4 bg-gray-50 border-t text-right">
            <span className="text-gray-500 text-sm mr-4">Gesamt:</span>
            <span className="font-bold text-lg text-green-600">
              {fmt(payments.reduce((sum, p) => sum + p.amount, 0))}
            </span>
          </div>
        </div>
      )}

      {/* CREATE MODAL */}
      {showCreate && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Neue Zahlung</h2>
              <button onClick={() => setShowCreate(false)}>
                <X size={20} />
              </button>
            </div>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Mietvertrag *
                </label>
                <select
                  value={form.leaseId}
                  onChange={(e) => {
                    const lease = activeLeases.find(
                      (l) => l.id === e.target.value,
                    );
                    setForm({
                      ...form,
                      leaseId: e.target.value,
                      amount: lease?.totalRent ?? 0,
                    });
                  }}
                  className="w-full border rounded-lg px-3 py-2"
                >
                  <option value="">Bitte wählen…</option>
                  {activeLeases.map((l) => (
                    <option key={l.id} value={l.id}>
                      {l.tenantName} – {fmt(l.totalRent)}/mtl.
                    </option>
                  ))}
                </select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Betrag *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.amount || ""}
                    onChange={(e) =>
                      setForm({ ...form, amount: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Status
                  </label>
                  <select
                    value={form.status}
                    onChange={(e) =>
                      setForm({ ...form, status: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  >
                    {statusLabels.map((l, i) => (
                      <option key={i} value={i}>
                        {l}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zahlungsdatum *
                  </label>
                  <input
                    type="date"
                    value={form.paymentDate}
                    onChange={(e) =>
                      setForm({ ...form, paymentDate: e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Fälligkeitsdatum *
                  </label>
                  <input
                    type="date"
                    value={form.dueDate}
                    onChange={(e) =>
                      setForm({ ...form, dueDate: e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Monat
                  </label>
                  <select
                    value={form.paymentMonth}
                    onChange={(e) =>
                      setForm({ ...form, paymentMonth: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  >
                    {monthNames.map((m, i) => (
                      <option key={i} value={i + 1}>
                        {m}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Jahr
                  </label>
                  <input
                    type="number"
                    value={form.paymentYear}
                    onChange={(e) =>
                      setForm({ ...form, paymentYear: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Art
                  </label>
                  <select
                    value={form.type}
                    onChange={(e) =>
                      setForm({ ...form, type: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  >
                    {typeLabels.map((l, i) => (
                      <option key={i} value={i}>
                        {l}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zahlungsart
                  </label>
                  <select
                    value={form.method}
                    onChange={(e) =>
                      setForm({ ...form, method: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  >
                    {methodLabels.map((l, i) => (
                      <option key={i} value={i}>
                        {l}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Verwendungszweck
                </label>
                <input
                  type="text"
                  value={form.reference}
                  onChange={(e) =>
                    setForm({ ...form, reference: e.target.value })
                  }
                  className="w-full border rounded-lg px-3 py-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notizen
                </label>
                <textarea
                  value={form.notes}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                  rows={2}
                  className="w-full border rounded-lg px-3 py-2"
                />
              </div>
              <button
                onClick={handleCreate}
                disabled={!form.leaseId || !form.amount}
                className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50"
              >
                Zahlung erfassen
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
