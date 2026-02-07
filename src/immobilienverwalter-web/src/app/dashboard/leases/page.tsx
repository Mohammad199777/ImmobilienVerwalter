"use client";
import { useEffect, useState, useCallback } from "react";
import { leasesApi, tenantsApi, unitsApi } from "@/lib/api";
import { FileText, Plus, Pencil, Trash2, X } from "lucide-react";

interface Lease {
  id: string;
  tenantName: string;
  unitName: string;
  propertyName: string;
  startDate: string;
  endDate?: string;
  coldRent: number;
  additionalCosts: number;
  totalRent: number;
  depositAmount: number;
  depositPaid: number;
  depositStatus: number;
  status: number;
  noticePeriodMonths: number;
  paymentDayOfMonth: number;
  notes?: string;
  isActive: boolean;
}

interface SelectItem {
  id: string;
  fullName?: string;
  name?: string;
}

const statusLabels = ["Aktiv", "Gekündigt", "Beendet", "Entwurf"];
const statusColors = [
  "bg-green-100 text-green-700",
  "bg-yellow-100 text-yellow-700",
  "bg-gray-100 text-gray-500",
  "bg-blue-100 text-blue-600",
];
const depositStatusLabels = [
  "Offen",
  "Teilweise",
  "Vollständig",
  "Zurückgezahlt",
];

const emptyForm = {
  tenantId: "",
  unitId: "",
  startDate: new Date().toISOString().split("T")[0],
  endDate: "",
  coldRent: 0,
  additionalCosts: 0,
  depositAmount: 0,
  noticePeriodMonths: 3,
  paymentDayOfMonth: 1,
  notes: "",
};
const emptyEditForm = {
  coldRent: 0,
  additionalCosts: 0,
  depositAmount: 0,
  depositPaid: 0,
  depositStatus: 0,
  status: 0,
  endDate: "",
  terminationDate: "",
  moveOutDate: "",
  noticePeriodMonths: 3,
  paymentDayOfMonth: 1,
  notes: "",
};

export default function LeasesPage() {
  const [leases, setLeases] = useState<Lease[]>([]);
  const [tenants, setTenants] = useState<SelectItem[]>([]);
  const [vacantUnits, setVacantUnits] = useState<SelectItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [editForm, setEditForm] = useState(emptyEditForm);
  const [filter, setFilter] = useState<"all" | "active">("all");

  const load = useCallback(async () => {
    try {
      const res =
        filter === "active"
          ? await leasesApi.getActive()
          : await leasesApi.getAll();
      setLeases(res.data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }, [filter]);

  useEffect(() => {
    load();
  }, [load]);

  const openCreate = async () => {
    try {
      const [t, u] = await Promise.all([
        tenantsApi.getAll(),
        unitsApi.getVacant(),
      ]);
      setTenants(t.data);
      setVacantUnits(u.data);
    } catch (e) {
      console.error(e);
    }
    setForm(emptyForm);
    setShowCreate(true);
  };

  const handleCreate = async () => {
    try {
      await leasesApi.create({ ...form, endDate: form.endDate || undefined });
      setShowCreate(false);
      load();
    } catch (e) {
      console.error(e);
    }
  };

  const openEdit = (l: Lease) => {
    setEditId(l.id);
    setEditForm({
      coldRent: l.coldRent,
      additionalCosts: l.additionalCosts,
      depositAmount: l.depositAmount,
      depositPaid: l.depositPaid,
      depositStatus: l.depositStatus ?? 0,
      status: l.status,
      endDate: l.endDate ? l.endDate.split("T")[0] : "",
      terminationDate: "",
      moveOutDate: "",
      noticePeriodMonths: l.noticePeriodMonths,
      paymentDayOfMonth: l.paymentDayOfMonth,
      notes: l.notes || "",
    });
    setShowEdit(true);
  };

  const handleUpdate = async () => {
    if (!editId) return;
    try {
      await leasesApi.update(editId, {
        ...editForm,
        endDate: editForm.endDate || undefined,
        terminationDate: editForm.terminationDate || undefined,
        moveOutDate: editForm.moveOutDate || undefined,
      });
      setShowEdit(false);
      setEditId(null);
      load();
    } catch (e) {
      console.error(e);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Mietvertrag wirklich löschen?")) return;
    try {
      await leasesApi.delete(id);
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
  const fmtDate = (d: string) => new Date(d).toLocaleDateString("de-DE");

  if (loading)
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Mietverträge</h1>
        <div className="flex gap-3">
          <select
            value={filter}
            onChange={(e) => setFilter(e.target.value as "all" | "active")}
            className="border rounded-lg px-3 py-2 text-sm"
          >
            <option value="all">Alle</option>
            <option value="active">Nur aktive</option>
          </select>
          <button
            onClick={openCreate}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-blue-700"
          >
            <Plus size={18} /> Neuer Vertrag
          </button>
        </div>
      </div>

      {leases.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <FileText size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Keine Mietverträge vorhanden.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border overflow-x-auto">
          <table className="w-full min-w-[900px]">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Mieter
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Einheit
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Zeitraum
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Kalt
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  NK
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Warm
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Kaution
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
              {leases.map((l) => (
                <tr key={l.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-900">
                    {l.tenantName}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    <div>{l.unitName}</div>
                    <div className="text-xs text-gray-400">
                      {l.propertyName}
                    </div>
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {fmtDate(l.startDate)} –{" "}
                    {l.endDate ? fmtDate(l.endDate) : "unbefristet"}
                  </td>
                  <td className="px-6 py-4 text-sm text-right text-gray-700">
                    {fmt(l.coldRent)}
                  </td>
                  <td className="px-6 py-4 text-sm text-right text-gray-500">
                    {fmt(l.additionalCosts)}
                  </td>
                  <td className="px-6 py-4 text-sm text-right font-medium text-gray-900">
                    {fmt(l.totalRent)}
                  </td>
                  <td className="px-6 py-4 text-sm text-right text-gray-500">
                    {fmt(l.depositPaid)} / {fmt(l.depositAmount)}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${statusColors[l.status]}`}
                    >
                      {statusLabels[l.status]}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-center">
                    <div className="flex justify-center gap-2">
                      <button
                        onClick={() => openEdit(l)}
                        className="text-blue-600 hover:text-blue-800"
                        title="Bearbeiten"
                      >
                        <Pencil size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(l.id)}
                        className="text-red-500 hover:text-red-700"
                        title="Löschen"
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

      {/* CREATE MODAL */}
      {showCreate && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Neuer Mietvertrag</h2>
              <button onClick={() => setShowCreate(false)}>
                <X size={20} />
              </button>
            </div>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Mieter *
                </label>
                <select
                  value={form.tenantId}
                  onChange={(e) =>
                    setForm({ ...form, tenantId: e.target.value })
                  }
                  className="w-full border rounded-lg px-3 py-2"
                >
                  <option value="">Bitte wählen…</option>
                  {tenants.map((t) => (
                    <option key={t.id} value={t.id}>
                      {t.fullName}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Einheit (leer stehend) *
                </label>
                <select
                  value={form.unitId}
                  onChange={(e) => setForm({ ...form, unitId: e.target.value })}
                  className="w-full border rounded-lg px-3 py-2"
                >
                  <option value="">Bitte wählen…</option>
                  {vacantUnits.map((u) => (
                    <option key={u.id} value={u.id}>
                      {u.name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Beginn *
                  </label>
                  <input
                    type="date"
                    value={form.startDate}
                    onChange={(e) =>
                      setForm({ ...form, startDate: e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Ende (leer = unbefristet)
                  </label>
                  <input
                    type="date"
                    value={form.endDate}
                    onChange={(e) =>
                      setForm({ ...form, endDate: e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kaltmiete *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.coldRent || ""}
                    onChange={(e) =>
                      setForm({ ...form, coldRent: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Nebenkosten *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.additionalCosts || ""}
                    onChange={(e) =>
                      setForm({ ...form, additionalCosts: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kaution *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.depositAmount || ""}
                    onChange={(e) =>
                      setForm({ ...form, depositAmount: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kündigungsfrist (Monate)
                  </label>
                  <input
                    type="number"
                    value={form.noticePeriodMonths}
                    onChange={(e) =>
                      setForm({ ...form, noticePeriodMonths: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zahltag
                  </label>
                  <input
                    type="number"
                    min={1}
                    max={28}
                    value={form.paymentDayOfMonth}
                    onChange={(e) =>
                      setForm({ ...form, paymentDayOfMonth: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
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
                disabled={!form.tenantId || !form.unitId || !form.coldRent}
                className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50"
              >
                Vertrag anlegen
              </button>
            </div>
          </div>
        </div>
      )}

      {/* EDIT MODAL */}
      {showEdit && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Vertrag bearbeiten</h2>
              <button
                onClick={() => {
                  setShowEdit(false);
                  setEditId(null);
                }}
              >
                <X size={20} />
              </button>
            </div>
            <div className="space-y-4">
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kaltmiete
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={editForm.coldRent || ""}
                    onChange={(e) =>
                      setEditForm({ ...editForm, coldRent: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    NK
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={editForm.additionalCosts || ""}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        additionalCosts: +e.target.value,
                      })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kaution
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={editForm.depositAmount || ""}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        depositAmount: +e.target.value,
                      })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kaution bezahlt
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={editForm.depositPaid || ""}
                    onChange={(e) =>
                      setEditForm({ ...editForm, depositPaid: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kautionsstatus
                  </label>
                  <select
                    value={editForm.depositStatus}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        depositStatus: +e.target.value,
                      })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  >
                    {depositStatusLabels.map((l, i) => (
                      <option key={i} value={i}>
                        {l}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Status
                </label>
                <select
                  value={editForm.status}
                  onChange={(e) =>
                    setEditForm({ ...editForm, status: +e.target.value })
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
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Vertragsende
                  </label>
                  <input
                    type="date"
                    value={editForm.endDate}
                    onChange={(e) =>
                      setEditForm({ ...editForm, endDate: e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kündigungsdatum
                  </label>
                  <input
                    type="date"
                    value={editForm.terminationDate}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        terminationDate: e.target.value,
                      })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Auszugsdatum
                  </label>
                  <input
                    type="date"
                    value={editForm.moveOutDate}
                    onChange={(e) =>
                      setEditForm({ ...editForm, moveOutDate: e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kündigungsfrist (Monate)
                  </label>
                  <input
                    type="number"
                    value={editForm.noticePeriodMonths}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        noticePeriodMonths: +e.target.value,
                      })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notizen
                </label>
                <textarea
                  value={editForm.notes}
                  onChange={(e) =>
                    setEditForm({ ...editForm, notes: e.target.value })
                  }
                  rows={2}
                  className="w-full border rounded-lg px-3 py-2"
                />
              </div>
              <button
                onClick={handleUpdate}
                className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700"
              >
                Speichern
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
