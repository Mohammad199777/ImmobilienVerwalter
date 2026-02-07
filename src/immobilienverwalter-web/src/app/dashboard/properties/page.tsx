"use client";
import { useEffect, useState } from "react";
import { propertiesApi, PropertyCreate } from "@/lib/api";
import { Building2, Plus, MapPin, Edit, Trash2, X } from "lucide-react";

const propertyTypes = [
  "Einfamilienhaus",
  "Mehrfamilienhaus",
  "Gewerbeimmobilie",
  "Misch Gewerbe/Wohn",
  "Garage",
  "Grundstück",
];

interface Property {
  id: string;
  name: string;
  street: string;
  houseNumber: string;
  zipCode: string;
  city: string;
  type: number;
  yearBuilt?: number;
  totalArea?: number;
  purchasePrice?: number;
  fullAddress: string;
  unitCount: number;
  occupiedUnits: number;
}

export default function PropertiesPage() {
  const [properties, setProperties] = useState<Property[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState<Property | null>(null);

  const [form, setForm] = useState<PropertyCreate>({
    name: "",
    street: "",
    houseNumber: "",
    zipCode: "",
    city: "",
    country: "Deutschland",
    type: 1,
  });

  const loadProperties = async () => {
    try {
      const res = await propertiesApi.getAll();
      setProperties(res.data);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadProperties();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editing) {
        await propertiesApi.update(editing.id, form);
      } else {
        await propertiesApi.create(form);
      }
      setShowForm(false);
      setEditing(null);
      setForm({
        name: "",
        street: "",
        houseNumber: "",
        zipCode: "",
        city: "",
        country: "Deutschland",
        type: 1,
      });
      loadProperties();
    } catch (e) {
      console.error(e);
    }
  };

  const handleEdit = (p: Property) => {
    setEditing(p);
    setForm({
      name: p.name,
      street: p.street,
      houseNumber: p.houseNumber,
      zipCode: p.zipCode,
      city: p.city,
      type: p.type,
      yearBuilt: p.yearBuilt,
      totalArea: p.totalArea,
      purchasePrice: p.purchasePrice,
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Immobilie wirklich löschen?")) return;
    await propertiesApi.delete(id);
    loadProperties();
  };

  const formatCurrency = (n?: number) =>
    n
      ? new Intl.NumberFormat("de-DE", {
          style: "currency",
          currency: "EUR",
        }).format(n)
      : "–";

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
        <h1 className="text-2xl font-bold text-gray-900">Immobilien</h1>
        <button
          onClick={() => {
            setShowForm(true);
            setEditing(null);
          }}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          <Plus size={18} /> Neue Immobilie
        </button>
      </div>

      {/* Form Modal */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">
                {editing ? "Immobilie bearbeiten" : "Neue Immobilie"}
              </h2>
              <button
                onClick={() => {
                  setShowForm(false);
                  setEditing(null);
                }}
              >
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Name
                </label>
                <input
                  required
                  className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                />
              </div>
              <div className="grid grid-cols-3 gap-3">
                <div className="col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Straße
                  </label>
                  <input
                    required
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.street}
                    onChange={(e) =>
                      setForm({ ...form, street: e.target.value })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Hausnr.
                  </label>
                  <input
                    required
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.houseNumber}
                    onChange={(e) =>
                      setForm({ ...form, houseNumber: e.target.value })
                    }
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    PLZ
                  </label>
                  <input
                    required
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.zipCode}
                    onChange={(e) =>
                      setForm({ ...form, zipCode: e.target.value })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Stadt
                  </label>
                  <input
                    required
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.city}
                    onChange={(e) => setForm({ ...form, city: e.target.value })}
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Typ
                  </label>
                  <select
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.type}
                    onChange={(e) =>
                      setForm({ ...form, type: parseInt(e.target.value) })
                    }
                  >
                    {propertyTypes.map((t, i) => (
                      <option key={i} value={i}>
                        {t}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Baujahr
                  </label>
                  <input
                    type="number"
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.yearBuilt || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        yearBuilt: parseInt(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Fläche (m²)
                  </label>
                  <input
                    type="number"
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.totalArea || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        totalArea: parseFloat(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Kaufpreis
                  </label>
                  <input
                    type="number"
                    className="w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                    value={form.purchasePrice || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        purchasePrice: parseFloat(e.target.value) || undefined,
                      })
                    }
                  />
                </div>
              </div>
              <button
                type="submit"
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium"
              >
                {editing ? "Speichern" : "Erstellen"}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* Properties List */}
      {properties.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Building2 size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Noch keine Immobilien vorhanden.</p>
          <p className="text-sm text-gray-400">
            Füge deine erste Immobilie hinzu!
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {properties.map((p) => (
            <div
              key={p.id}
              className="bg-white rounded-xl shadow-sm border p-6 hover:shadow-md transition"
            >
              <div className="flex justify-between items-start mb-3">
                <h3 className="font-bold text-lg text-gray-900">{p.name}</h3>
                <span className="text-xs bg-blue-100 text-blue-700 px-2 py-1 rounded-full">
                  {propertyTypes[p.type]}
                </span>
              </div>
              <p className="text-gray-500 text-sm flex items-center gap-1 mb-4">
                <MapPin size={14} /> {p.fullAddress}
              </p>
              <div className="grid grid-cols-3 gap-2 text-center mb-4">
                <div>
                  <p className="text-lg font-bold text-gray-900">
                    {p.unitCount}
                  </p>
                  <p className="text-xs text-gray-500">Einheiten</p>
                </div>
                <div>
                  <p className="text-lg font-bold text-green-600">
                    {p.occupiedUnits}
                  </p>
                  <p className="text-xs text-gray-500">Vermietet</p>
                </div>
                <div>
                  <p className="text-lg font-bold text-orange-600">
                    {p.unitCount - p.occupiedUnits}
                  </p>
                  <p className="text-xs text-gray-500">Leer</p>
                </div>
              </div>
              {p.purchasePrice && (
                <p className="text-sm text-gray-400 mb-3">
                  Kaufpreis: {formatCurrency(p.purchasePrice)}
                </p>
              )}
              <div className="flex gap-2">
                <button
                  onClick={() => handleEdit(p)}
                  className="flex-1 flex items-center justify-center gap-1 text-sm border border-gray-300 py-1.5 rounded-lg hover:bg-gray-50 transition"
                >
                  <Edit size={14} /> Bearbeiten
                </button>
                <button
                  onClick={() => handleDelete(p.id)}
                  className="flex items-center justify-center gap-1 text-sm border border-red-200 text-red-600 py-1.5 px-3 rounded-lg hover:bg-red-50 transition"
                >
                  <Trash2 size={14} />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
