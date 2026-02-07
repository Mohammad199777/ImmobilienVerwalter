"use client";
import { useEffect, useState, useCallback } from "react";
import { expensesApi, propertiesApi, unitsApi } from "@/lib/api";
import { getErrorMessage } from "@/lib/useToast";
import { useAppToast } from "../layout";
import { Receipt, Plus, Trash2, Pencil, X } from "lucide-react";

interface Expense {
  id: string;
  title: string;
  description?: string;
  amount: number;
  date: string;
  dueDate?: string;
  category: number;
  isRecurring: boolean;
  recurringInterval?: number;
  isAllocatable: boolean;
  isTaxDeductible: boolean;
  vendor?: string;
  invoiceNumber?: string;
  propertyId?: string;
  unitId?: string;
  notes?: string;
  propertyName?: string;
  unitName?: string;
}
interface PropertyOption {
  id: string;
  name: string;
}
interface UnitOption {
  id: string;
  name: string;
  propertyId: string;
}

const categoryLabels = [
  "Instandhaltung",
  "Reparatur",
  "Versicherung",
  "Steuer",
  "Verwaltung",
  "Nebenkosten",
  "Sonstige",
];
const categoryColors = [
  "bg-yellow-100 text-yellow-700",
  "bg-red-100 text-red-700",
  "bg-blue-100 text-blue-700",
  "bg-purple-100 text-purple-700",
  "bg-gray-100 text-gray-700",
  "bg-green-100 text-green-700",
  "bg-gray-100 text-gray-500",
];

const emptyForm = {
  title: "",
  description: "",
  amount: 0,
  date: new Date().toISOString().split("T")[0],
  dueDate: "",
  category: 0,
  isRecurring: false,
  recurringInterval: undefined as number | undefined,
  isAllocatable: false,
  isTaxDeductible: false,
  vendor: "",
  invoiceNumber: "",
  propertyId: "",
  unitId: "",
  notes: "",
};

export default function ExpensesPage() {
  const toast = useAppToast();
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [properties, setProperties] = useState<PropertyOption[]>([]);
  const [units, setUnits] = useState<UnitOption[]>([]);
  const [filteredUnits, setFilteredUnits] = useState<UnitOption[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [form, setForm] = useState(emptyForm);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [expRes, propRes, unitRes] = await Promise.all([
        expensesApi.getAll(),
        propertiesApi.getAll(),
        unitsApi.getAll(),
      ]);
      setExpenses(expRes.data);
      setProperties(propRes.data);
      setUnits(unitRes.data);
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [toast]);

  useEffect(() => {
    load();
  }, [load]);

  // Filter units by selected property
  useEffect(() => {
    setFilteredUnits(
      form.propertyId
        ? units.filter((u) => u.propertyId === form.propertyId)
        : [],
    );
    if (form.propertyId && form.unitId) {
      const unitBelongs = units.some(
        (u) => u.id === form.unitId && u.propertyId === form.propertyId,
      );
      if (!unitBelongs) setForm((f) => ({ ...f, unitId: "" }));
    }
  }, [form.propertyId, form.unitId, units]);

  const openCreate = () => {
    setEditId(null);
    setForm(emptyForm);
    setShowModal(true);
  };

  const openEdit = (e: Expense) => {
    setEditId(e.id);
    setForm({
      title: e.title,
      description: e.description ?? "",
      amount: e.amount,
      date: e.date.split("T")[0],
      dueDate: e.dueDate?.split("T")[0] ?? "",
      category: e.category,
      isRecurring: e.isRecurring,
      recurringInterval: e.recurringInterval,
      isAllocatable: e.isAllocatable,
      isTaxDeductible: e.isTaxDeductible,
      vendor: e.vendor ?? "",
      invoiceNumber: e.invoiceNumber ?? "",
      propertyId: e.propertyId ?? "",
      unitId: e.unitId ?? "",
      notes: e.notes ?? "",
    });
    setShowModal(true);
  };

  const handleSave = async () => {
    setSubmitting(true);
    try {
      const payload = {
        title: form.title,
        description: form.description || undefined,
        amount: form.amount,
        date: form.date,
        dueDate: form.dueDate || undefined,
        category: form.category,
        isRecurring: form.isRecurring,
        recurringInterval: form.isRecurring
          ? form.recurringInterval
          : undefined,
        isAllocatable: form.isAllocatable,
        isTaxDeductible: form.isTaxDeductible,
        vendor: form.vendor || undefined,
        invoiceNumber: form.invoiceNumber || undefined,
        propertyId: form.propertyId || undefined,
        unitId: form.unitId || undefined,
        notes: form.notes || undefined,
      };
      if (editId) {
        await expensesApi.update(editId, payload);
        toast("Ausgabe aktualisiert.", "success");
      } else {
        await expensesApi.create(payload);
        toast("Ausgabe erfasst.", "success");
      }
      setShowModal(false);
      load();
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Ausgabe wirklich löschen?")) return;
    try {
      await expensesApi.delete(id);
      toast("Ausgabe gelöscht.", "success");
      load();
    } catch (err) {
      toast(getErrorMessage(err));
    }
  };

  const fmt = (n: number) =>
    new Intl.NumberFormat("de-DE", {
      style: "currency",
      currency: "EUR",
    }).format(n);

  const renderFormFields = () => (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Titel *
        </label>
        <input
          type="text"
          value={form.title}
          onChange={(e) => setForm({ ...form, title: e.target.value })}
          className="w-full border rounded-lg px-3 py-2"
          required
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Beschreibung
        </label>
        <textarea
          value={form.description}
          onChange={(e) => setForm({ ...form, description: e.target.value })}
          rows={2}
          className="w-full border rounded-lg px-3 py-2"
        />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Betrag *
          </label>
          <input
            type="number"
            step="0.01"
            min={0.01}
            value={form.amount || ""}
            onChange={(e) => setForm({ ...form, amount: +e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Kategorie
          </label>
          <select
            value={form.category}
            onChange={(e) => setForm({ ...form, category: +e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
          >
            {categoryLabels.map((l, i) => (
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
            Datum *
          </label>
          <input
            type="date"
            value={form.date}
            onChange={(e) => setForm({ ...form, date: e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Fälligkeitsdatum
          </label>
          <input
            type="date"
            value={form.dueDate}
            onChange={(e) => setForm({ ...form, dueDate: e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Immobilie
          </label>
          <select
            value={form.propertyId}
            onChange={(e) => setForm({ ...form, propertyId: e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
          >
            <option value="">Keine Zuordnung</option>
            {properties.map((p) => (
              <option key={p.id} value={p.id}>
                {p.name}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Einheit
          </label>
          <select
            value={form.unitId}
            onChange={(e) => setForm({ ...form, unitId: e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
            disabled={!form.propertyId}
          >
            <option value="">Keine Zuordnung</option>
            {filteredUnits.map((u) => (
              <option key={u.id} value={u.id}>
                {u.name}
              </option>
            ))}
          </select>
        </div>
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Lieferant
          </label>
          <input
            type="text"
            value={form.vendor}
            onChange={(e) => setForm({ ...form, vendor: e.target.value })}
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Rechnungsnummer
          </label>
          <input
            type="text"
            value={form.invoiceNumber}
            onChange={(e) =>
              setForm({ ...form, invoiceNumber: e.target.value })
            }
            className="w-full border rounded-lg px-3 py-2"
          />
        </div>
      </div>

      <fieldset className="border rounded-lg p-4">
        <legend className="text-sm font-medium text-gray-700 px-2">
          Optionen
        </legend>
        <div className="space-y-3">
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={form.isAllocatable}
              onChange={(e) =>
                setForm({ ...form, isAllocatable: e.target.checked })
              }
              className="rounded"
            />
            <span className="text-sm text-gray-700">Umlagefähig</span>
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={form.isTaxDeductible}
              onChange={(e) =>
                setForm({ ...form, isTaxDeductible: e.target.checked })
              }
              className="rounded"
            />
            <span className="text-sm text-gray-700">Steuerlich absetzbar</span>
          </label>
          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              checked={form.isRecurring}
              onChange={(e) =>
                setForm({ ...form, isRecurring: e.target.checked })
              }
              className="rounded"
            />
            <span className="text-sm text-gray-700">Wiederkehrend</span>
          </label>
          {form.isRecurring && (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Intervall (Monate)
              </label>
              <input
                type="number"
                min={1}
                value={form.recurringInterval ?? ""}
                onChange={(e) =>
                  setForm({
                    ...form,
                    recurringInterval: +e.target.value || undefined,
                  })
                }
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
          )}
        </div>
      </fieldset>

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
        onClick={handleSave}
        disabled={!form.title || !form.amount || submitting}
        className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50"
      >
        {submitting
          ? "Speichern…"
          : editId
            ? "Aktualisieren"
            : "Ausgabe erfassen"}
      </button>
    </div>
  );

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Ausgaben</h1>
        <button
          onClick={openCreate}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-blue-700"
        >
          <Plus size={18} /> Neue Ausgabe
        </button>
      </div>

      {loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : expenses.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Receipt size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Noch keine Ausgaben erfasst.</p>
        </div>
      ) : (
        <>
          <div className="bg-white rounded-xl shadow-sm border overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b">
                <tr>
                  <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                    Titel
                  </th>
                  <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                    Betrag
                  </th>
                  <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                    Datum
                  </th>
                  <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                    Kategorie
                  </th>
                  <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                    Immobilie
                  </th>
                  <th className="text-center px-6 py-3 text-sm font-medium text-gray-500">
                    Umlagef.
                  </th>
                  <th className="text-center px-6 py-3 text-sm font-medium text-gray-500">
                    Aktionen
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {expenses.map((e) => (
                  <tr key={e.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-gray-900">{e.title}</div>
                      {e.vendor && (
                        <div className="text-xs text-gray-400">{e.vendor}</div>
                      )}
                    </td>
                    <td className="px-6 py-4 text-right font-medium text-red-600">
                      {fmt(e.amount)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {new Date(e.date).toLocaleDateString("de-DE")}
                    </td>
                    <td className="px-6 py-4">
                      <span
                        className={`px-2 py-1 rounded-full text-xs font-medium ${categoryColors[e.category]}`}
                      >
                        {categoryLabels[e.category]}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {e.propertyName ?? "–"}
                    </td>
                    <td className="px-6 py-4 text-center">
                      {e.isAllocatable ? "✓" : "–"}
                    </td>
                    <td className="px-6 py-4 text-center flex justify-center gap-2">
                      <button
                        onClick={() => openEdit(e)}
                        className="text-blue-500 hover:text-blue-700"
                        aria-label="Bearbeiten"
                      >
                        <Pencil size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(e.id)}
                        className="text-red-500 hover:text-red-700"
                        aria-label="Löschen"
                      >
                        <Trash2 size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <div className="mt-4 p-4 bg-white rounded-xl shadow-sm border text-right">
            <span className="text-gray-500 text-sm mr-4">Gesamt:</span>
            <span className="font-bold text-lg text-red-600">
              {fmt(expenses.reduce((s, e) => s + e.amount, 0))}
            </span>
          </div>
        </>
      )}

      {/* CREATE / EDIT MODAL */}
      {showModal && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">
                {editId ? "Ausgabe bearbeiten" : "Neue Ausgabe"}
              </h2>
              <button
                onClick={() => setShowModal(false)}
                aria-label="Schließen"
              >
                <X size={20} />
              </button>
            </div>
            {renderFormFields()}
          </div>
        </div>
      )}
    </div>
  );
}
