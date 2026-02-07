"use client";
import { useEffect, useState } from "react";
import { expensesApi, ExpenseCreate, ExpenseUpdate } from "@/lib/api";
import { Euro, Plus, Pencil, Trash2, X } from "lucide-react";

const categoryLabels = [
  "Reparatur",
  "Wartung",
  "Versicherung",
  "Grundsteuer",
  "Hausverwaltung",
  "Wasser",
  "Heizung",
  "Strom",
  "Müllabfuhr",
  "Schornsteinfeger",
  "Gartenpflege",
  "Reinigung",
  "Aufzug",
  "Bankgebühren",
  "Zinsen",
  "Abschreibung",
  "Renovierung",
  "Modernisierung",
  "Rechtskosten",
  "Sonstige",
];

interface Expense {
  id: string;
  title: string;
  description?: string;
  amount: number;
  date: string;
  dueDate?: string;
  category: number;
  isRecurring: boolean;
  isAllocatable: boolean;
  isTaxDeductible: boolean;
  vendor?: string;
  invoiceNumber?: string;
  propertyName?: string;
  unitName?: string;
  notes?: string;
}

const emptyForm: ExpenseCreate = {
  title: "",
  amount: 0,
  date: new Date().toISOString().split("T")[0],
  category: 0,
  isRecurring: false,
  isAllocatable: false,
  isTaxDeductible: true,
};

export default function ExpensesPage() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [form, setForm] = useState<ExpenseCreate>(emptyForm);
  const [editForm, setEditForm] = useState<ExpenseUpdate>(emptyForm);

  const load = async () => {
    try {
      const res = await expensesApi.getAll();
      setExpenses(res.data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    load();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await expensesApi.create(form);
    setShowForm(false);
    setForm(emptyForm);
    load();
  };

  const openEdit = (exp: Expense) => {
    setEditId(exp.id);
    setEditForm({
      title: exp.title,
      description: exp.description,
      amount: exp.amount,
      date: exp.date.split("T")[0],
      dueDate: exp.dueDate?.split("T")[0],
      category: exp.category,
      isRecurring: exp.isRecurring,
      isAllocatable: exp.isAllocatable,
      isTaxDeductible: exp.isTaxDeductible,
      vendor: exp.vendor,
      invoiceNumber: exp.invoiceNumber,
      notes: exp.notes,
    });
    setShowEdit(true);
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editId) return;
    await expensesApi.update(editId, editForm);
    setShowEdit(false);
    setEditId(null);
    load();
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Ausgabe wirklich löschen?")) return;
    try {
      await expensesApi.delete(id);
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

  const renderFormFields = (
    f: ExpenseCreate | ExpenseUpdate,
    setF: (v: typeof f) => void,
  ) => (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Titel *
        </label>
        <input
          required
          className="w-full px-3 py-2 border rounded-lg"
          value={f.title}
          onChange={(e) => setF({ ...f, title: e.target.value })}
        />
      </div>
      <div className="grid grid-cols-2 gap-3">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Betrag *
          </label>
          <input
            type="number"
            step="0.01"
            required
            className="w-full px-3 py-2 border rounded-lg"
            value={f.amount || ""}
            onChange={(e) => setF({ ...f, amount: parseFloat(e.target.value) })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Datum *
          </label>
          <input
            type="date"
            required
            className="w-full px-3 py-2 border rounded-lg"
            value={f.date}
            onChange={(e) => setF({ ...f, date: e.target.value })}
          />
        </div>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Kategorie
        </label>
        <select
          className="w-full px-3 py-2 border rounded-lg"
          value={f.category}
          onChange={(e) => setF({ ...f, category: parseInt(e.target.value) })}
        >
          {categoryLabels.map((c, i) => (
            <option key={i} value={i}>
              {c}
            </option>
          ))}
        </select>
      </div>
      <div className="grid grid-cols-2 gap-3">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Dienstleister
          </label>
          <input
            className="w-full px-3 py-2 border rounded-lg"
            value={f.vendor || ""}
            onChange={(e) => setF({ ...f, vendor: e.target.value })}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Rechnungsnr.
          </label>
          <input
            className="w-full px-3 py-2 border rounded-lg"
            value={f.invoiceNumber || ""}
            onChange={(e) => setF({ ...f, invoiceNumber: e.target.value })}
          />
        </div>
      </div>
      <div className="flex gap-6">
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={f.isAllocatable}
            onChange={(e) => setF({ ...f, isAllocatable: e.target.checked })}
          />
          <span className="text-sm">Umlagefähig</span>
        </label>
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={f.isTaxDeductible}
            onChange={(e) => setF({ ...f, isTaxDeductible: e.target.checked })}
          />
          <span className="text-sm">Steuerlich absetzbar</span>
        </label>
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={f.isRecurring}
            onChange={(e) => setF({ ...f, isRecurring: e.target.checked })}
          />
          <span className="text-sm">Wiederkehrend</span>
        </label>
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Notizen
        </label>
        <textarea
          className="w-full px-3 py-2 border rounded-lg"
          rows={2}
          value={f.notes || ""}
          onChange={(e) => setF({ ...f, notes: e.target.value })}
        />
      </div>
    </div>
  );

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Ausgaben</h1>
        <button
          onClick={() => {
            setForm(emptyForm);
            setShowForm(true);
          }}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          <Plus size={18} /> Neue Ausgabe
        </button>
      </div>

      {/* CREATE MODAL */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Neue Ausgabe</h2>
              <button onClick={() => setShowForm(false)}>
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleSubmit}>
              {renderFormFields(
                form,
                setForm as (v: ExpenseCreate | ExpenseUpdate) => void,
              )}
              <button
                type="submit"
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium mt-4"
              >
                Erstellen
              </button>
            </form>
          </div>
        </div>
      )}

      {/* EDIT MODAL */}
      {showEdit && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Ausgabe bearbeiten</h2>
              <button
                onClick={() => {
                  setShowEdit(false);
                  setEditId(null);
                }}
              >
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleUpdate}>
              {renderFormFields(
                editForm,
                setEditForm as (v: ExpenseCreate | ExpenseUpdate) => void,
              )}
              <button
                type="submit"
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium mt-4"
              >
                Speichern
              </button>
            </form>
          </div>
        </div>
      )}

      {loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : expenses.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Euro size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Noch keine Ausgaben erfasst.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Titel
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Kategorie
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Betrag
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Datum
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Dienstleister
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
              {expenses.map((exp) => (
                <tr key={exp.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 font-medium text-gray-900">
                    {exp.title}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {categoryLabels[exp.category]}
                  </td>
                  <td className="px-6 py-4 text-right font-medium text-red-600">
                    {fmt(exp.amount)}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {new Date(exp.date).toLocaleDateString("de-DE")}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {exp.vendor || "–"}
                  </td>
                  <td className="px-6 py-4 text-center">
                    {exp.isAllocatable ? "✅" : "–"}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <div className="flex justify-center gap-2">
                      <button
                        onClick={() => openEdit(exp)}
                        className="text-blue-600 hover:text-blue-800"
                        title="Bearbeiten"
                      >
                        <Pencil size={16} />
                      </button>
                      <button
                        onClick={() => handleDelete(exp.id)}
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
            <tfoot>
              <tr className="bg-gray-50 border-t">
                <td colSpan={2} className="px-6 py-4 font-medium text-gray-700">
                  Gesamt
                </td>
                <td className="px-6 py-4 text-right font-bold text-red-600">
                  {fmt(expenses.reduce((s, e) => s + e.amount, 0))}
                </td>
                <td colSpan={4}></td>
              </tr>
            </tfoot>
          </table>
        </div>
      )}
    </div>
  );
}
