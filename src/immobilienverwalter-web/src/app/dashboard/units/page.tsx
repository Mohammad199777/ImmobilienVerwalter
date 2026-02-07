"use client";
import { useEffect, useState, useCallback } from "react";
import { propertiesApi, unitsApi, UnitCreate, UnitUpdate } from "@/lib/api";
import { getErrorMessage } from "@/lib/useToast";
import { useAppToast } from "../layout";
import { Home, Plus, Pencil, Trash2, X } from "lucide-react";

const unitTypes = [
  "Wohnung",
  "Gewerbe",
  "Garage",
  "Stellplatz",
  "Keller",
  "Sonstige",
];
const unitStatuses = ["Vermietet", "Leer", "In Renovierung", "Eigennutzung"];

interface Unit {
  id: string;
  name: string;
  description?: string;
  floor?: number;
  area: number;
  rooms?: number;
  type: number;
  status: number;
  targetRent: number;
  propertyId: string;
  propertyName: string;
  currentTenant?: { id: string; fullName: string; email: string };
}
interface Property {
  id: string;
  name: string;
}

const emptyCreate: UnitCreate = {
  name: "",
  area: 0,
  type: 0,
  targetRent: 0,
  propertyId: "",
};

export default function UnitsPage() {
  const toast = useAppToast();
  const [units, setUnits] = useState<Unit[]>([]);
  const [properties, setProperties] = useState<Property[]>([]);
  const [selectedProperty, setSelectedProperty] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [form, setForm] = useState<UnitCreate>(emptyCreate);
  const [editForm, setEditForm] = useState<UnitUpdate>({
    name: "",
    area: 0,
    type: 0,
    targetRent: 0,
    status: 0,
  });

  const loadData = useCallback(async () => {
    try {
      const propRes = await propertiesApi.getAll();
      setProperties(propRes.data);
      if (propRes.data.length > 0 && !selectedProperty)
        setSelectedProperty(propRes.data[0].id);
    } catch (err) {
      toast(getErrorMessage(err));
    }
  }, [selectedProperty, toast]);

  const loadUnits = useCallback(async () => {
    if (!selectedProperty) {
      setLoading(false);
      return;
    }
    try {
      const res = await unitsApi.getByProperty(selectedProperty);
      setUnits(res.data);
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [selectedProperty, toast]);

  useEffect(() => {
    loadData();
  }, [loadData]);
  useEffect(() => {
    if (selectedProperty) loadUnits();
  }, [loadUnits, selectedProperty]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await unitsApi.create({ ...form, propertyId: selectedProperty });
      toast("Einheit erstellt.", "success");
      setShowForm(false);
      setForm(emptyCreate);
      loadUnits();
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const openEdit = (u: Unit) => {
    setEditId(u.id);
    setEditForm({
      name: u.name,
      description: u.description,
      floor: u.floor,
      area: u.area,
      rooms: u.rooms,
      type: u.type,
      targetRent: u.targetRent,
      status: u.status,
    });
    setShowEdit(true);
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editId) return;
    setSubmitting(true);
    try {
      await unitsApi.update(editId, editForm);
      toast("Einheit aktualisiert.", "success");
      setShowEdit(false);
      setEditId(null);
      loadUnits();
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Einheit wirklich löschen?")) return;
    try {
      await unitsApi.delete(id);
      toast("Einheit gelöscht.", "success");
      loadUnits();
    } catch (err) {
      toast(getErrorMessage(err));
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
        <h1 className="text-2xl font-bold text-gray-900">Einheiten</h1>
        <button
          onClick={() => setShowForm(true)}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          <Plus size={18} /> Neue Einheit
        </button>
      </div>

      <div className="mb-6">
        <label htmlFor="property-select" className="sr-only">
          Immobilie auswählen
        </label>
        <select
          id="property-select"
          className="px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
          value={selectedProperty}
          onChange={(e) => setSelectedProperty(e.target.value)}
        >
          {properties.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name}
            </option>
          ))}
        </select>
      </div>

      {/* Create Modal */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Neue Einheit</h2>
              <button onClick={() => setShowForm(false)} aria-label="Schließen">
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Name *
                </label>
                <input
                  required
                  className="w-full px-3 py-2 border rounded-lg"
                  placeholder="z.B. Wohnung 1 OG links"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                />
              </div>
              <div className="grid grid-cols-3 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Fläche (m²) *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    min={0.01}
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.area || ""}
                    onChange={(e) =>
                      setForm({ ...form, area: parseFloat(e.target.value) })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zimmer
                  </label>
                  <input
                    type="number"
                    min={1}
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.rooms || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        rooms: parseInt(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Etage
                  </label>
                  <input
                    type="number"
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.floor ?? ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        floor: parseInt(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Typ
                  </label>
                  <select
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.type}
                    onChange={(e) =>
                      setForm({ ...form, type: parseInt(e.target.value) })
                    }
                  >
                    {unitTypes.map((t, i) => (
                      <option key={i} value={i}>
                        {t}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Soll-Miete *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    min={0}
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.targetRent || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        targetRent: parseFloat(e.target.value),
                      })
                    }
                  />
                </div>
              </div>
              <button
                type="submit"
                disabled={submitting}
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium disabled:opacity-50"
              >
                {submitting ? "Erstellen…" : "Erstellen"}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Units Grid */}
      {loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : units.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Home size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Keine Einheiten für diese Immobilie.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {units.map((u) => (
            <div
              key={u.id}
              className="bg-white rounded-xl shadow-sm border p-5"
            >
              <div className="flex justify-between items-start mb-3">
                <h3 className="font-bold text-gray-900">{u.name}</h3>
                <span
                  className={`text-xs px-2 py-1 rounded-full font-medium ${u.status === 0 ? "bg-green-100 text-green-700" : u.status === 1 ? "bg-orange-100 text-orange-700" : "bg-gray-100 text-gray-600"}`}
                >
                  {unitStatuses[u.status]}
                </span>
              </div>
              <div className="space-y-1 text-sm text-gray-500">
                <p>
                  {u.area} m² · {u.rooms || "–"} Zimmer ·{" "}
                  {u.floor != null ? `${u.floor}. OG` : "–"}
                </p>
                <p className="text-xs">{unitTypes[u.type]}</p>
                <p className="font-medium text-gray-700 mt-2">
                  Miete: {fmt(u.targetRent)}
                </p>
              </div>
              {u.currentTenant && (
                <div className="mt-3 pt-3 border-t text-sm">
                  <p className="text-gray-700 font-medium">
                    {u.currentTenant.fullName}
                  </p>
                  <p className="text-gray-400 text-xs">
                    {u.currentTenant.email}
                  </p>
                </div>
              )}
              <div className="mt-3 pt-3 border-t flex gap-2">
                <button
                  onClick={() => openEdit(u)}
                  className="text-blue-600 hover:text-blue-800 text-sm flex items-center gap-1"
                >
                  <Pencil size={14} /> Bearbeiten
                </button>
                <button
                  onClick={() => handleDelete(u.id)}
                  className="text-red-500 hover:text-red-700 text-sm flex items-center gap-1"
                >
                  <Trash2 size={14} /> Löschen
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Edit Modal */}
      {showEdit && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Einheit bearbeiten</h2>
              <button
                onClick={() => {
                  setShowEdit(false);
                  setEditId(null);
                }}
                aria-label="Schließen"
              >
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleUpdate} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Name *
                </label>
                <input
                  required
                  className="w-full px-3 py-2 border rounded-lg"
                  value={editForm.name}
                  onChange={(e) =>
                    setEditForm({ ...editForm, name: e.target.value })
                  }
                />
              </div>
              <div className="grid grid-cols-3 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Fläche (m²) *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    className="w-full px-3 py-2 border rounded-lg"
                    value={editForm.area || ""}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        area: parseFloat(e.target.value),
                      })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zimmer
                  </label>
                  <input
                    type="number"
                    className="w-full px-3 py-2 border rounded-lg"
                    value={editForm.rooms || ""}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        rooms: parseInt(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Etage
                  </label>
                  <input
                    type="number"
                    className="w-full px-3 py-2 border rounded-lg"
                    value={editForm.floor ?? ""}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        floor: parseInt(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
              </div>
              <div className="grid grid-cols-3 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Typ
                  </label>
                  <select
                    className="w-full px-3 py-2 border rounded-lg"
                    value={editForm.type}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        type: parseInt(e.target.value),
                      })
                    }
                  >
                    {unitTypes.map((t, i) => (
                      <option key={i} value={i}>
                        {t}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Status
                  </label>
                  <select
                    className="w-full px-3 py-2 border rounded-lg"
                    value={editForm.status}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        status: parseInt(e.target.value),
                      })
                    }
                  >
                    {unitStatuses.map((s, i) => (
                      <option key={i} value={i}>
                        {s}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Soll-Miete *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    className="w-full px-3 py-2 border rounded-lg"
                    value={editForm.targetRent || ""}
                    onChange={(e) =>
                      setEditForm({
                        ...editForm,
                        targetRent: parseFloat(e.target.value),
                      })
                    }
                  />
                </div>
              </div>
              <button
                type="submit"
                disabled={submitting}
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium disabled:opacity-50"
              >
                {submitting ? "Speichern…" : "Speichern"}
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
