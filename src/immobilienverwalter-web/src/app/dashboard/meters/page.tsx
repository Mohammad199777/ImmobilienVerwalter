"use client";
import { useEffect, useState } from "react";
import { meterReadingsApi, unitsApi, MeterReadingCreate } from "@/lib/api";
import { Gauge, Plus, Trash2, X } from "lucide-react";

// Backend enum: Wasser=0, WarmWasser=1, Gas=2, Strom=3, Heizung=4, Sonstige=5
const meterTypeLabels = [
  "Wasser",
  "Warmwasser",
  "Gas",
  "Strom",
  "Heizung",
  "Sonstige",
];

interface Unit {
  id: string;
  name: string;
  propertyName?: string;
}
interface MeterReading {
  id: string;
  unitId: string;
  meterType: number;
  meterNumber: string;
  readingDate: string;
  value: number;
  previousValue?: number;
  consumption?: number;
  notes?: string;
}

export default function MetersPage() {
  const [units, setUnits] = useState<Unit[]>([]);
  const [selectedUnit, setSelectedUnit] = useState("");
  const [readings, setReadings] = useState<MeterReading[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<MeterReadingCreate>({
    unitId: "",
    meterType: 0,
    meterNumber: "",
    readingDate: new Date().toISOString().split("T")[0],
    value: 0,
  });

  useEffect(() => {
    (async () => {
      try {
        const res = await unitsApi.getAll();
        setUnits(res.data);
        if (res.data.length > 0) setSelectedUnit(res.data[0].id);
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const loadReadings = async () => {
    if (!selectedUnit) return;
    try {
      const res = await meterReadingsApi.getByUnit(selectedUnit);
      setReadings(res.data);
    } catch (e) {
      console.error(e);
    }
  };

  useEffect(() => {
    loadReadings();
  }, [selectedUnit]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await meterReadingsApi.create({ ...form, unitId: selectedUnit });
    setShowForm(false);
    loadReadings();
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Zählerstand wirklich löschen?")) return;
    try {
      await meterReadingsApi.delete(id);
      loadReadings();
    } catch (e) {
      console.error(e);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Zählerstände</h1>
        <button
          onClick={() => setShowForm(true)}
          disabled={!selectedUnit}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
        >
          <Plus size={18} /> Neuer Zählerstand
        </button>
      </div>

      <div className="mb-6">
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Einheit auswählen
        </label>
        <select
          value={selectedUnit}
          onChange={(e) => setSelectedUnit(e.target.value)}
          className="w-full max-w-md px-3 py-2 border rounded-lg bg-white"
        >
          {units.map((u) => (
            <option key={u.id} value={u.id}>
              {u.name}
              {u.propertyName ? ` – ${u.propertyName}` : ""}
            </option>
          ))}
        </select>
      </div>

      {showForm && (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Neuer Zählerstand</h2>
              <button onClick={() => setShowForm(false)}>
                <X size={24} className="text-gray-500" />
              </button>
            </div>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Zählertyp
                </label>
                <select
                  className="w-full px-3 py-2 border rounded-lg"
                  value={form.meterType}
                  onChange={(e) =>
                    setForm({ ...form, meterType: parseInt(e.target.value) })
                  }
                >
                  {meterTypeLabels.map((m, i) => (
                    <option key={i} value={i}>
                      {m}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Zählernummer *
                </label>
                <input
                  required
                  className="w-full px-3 py-2 border rounded-lg"
                  value={form.meterNumber}
                  onChange={(e) =>
                    setForm({ ...form, meterNumber: e.target.value })
                  }
                />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Ablesedatum *
                  </label>
                  <input
                    type="date"
                    required
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.readingDate}
                    onChange={(e) =>
                      setForm({ ...form, readingDate: e.target.value })
                    }
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zählerstand *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    required
                    className="w-full px-3 py-2 border rounded-lg"
                    value={form.value || ""}
                    onChange={(e) =>
                      setForm({ ...form, value: parseFloat(e.target.value) })
                    }
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Notizen
                </label>
                <textarea
                  className="w-full px-3 py-2 border rounded-lg"
                  rows={2}
                  value={form.notes || ""}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                />
              </div>
              <button
                type="submit"
                className="w-full bg-blue-600 text-white py-2.5 rounded-lg hover:bg-blue-700 transition font-medium"
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
      ) : !selectedUnit ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Gauge size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Bitte eine Einheit auswählen.</p>
        </div>
      ) : readings.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Gauge size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">
            Keine Zählerstände für diese Einheit vorhanden.
          </p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Zählertyp
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Zählernummer
                </th>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Datum
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Stand
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Vorheriger
                </th>
                <th className="text-right px-6 py-3 text-sm font-medium text-gray-500">
                  Verbrauch
                </th>
                <th className="text-center px-6 py-3 text-sm font-medium text-gray-500">
                  Aktionen
                </th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {readings.map((r) => (
                <tr key={r.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4">
                    <span className="inline-block bg-blue-100 text-blue-700 text-xs font-medium px-2.5 py-1 rounded-full">
                      {meterTypeLabels[r.meterType] ?? "Unbekannt"}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-700">
                    {r.meterNumber}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    {new Date(r.readingDate).toLocaleDateString("de-DE")}
                  </td>
                  <td className="px-6 py-4 text-right font-medium text-gray-900">
                    {r.value.toLocaleString("de-DE")}
                  </td>
                  <td className="px-6 py-4 text-right text-sm text-gray-500">
                    {r.previousValue != null
                      ? r.previousValue.toLocaleString("de-DE")
                      : "–"}
                  </td>
                  <td className="px-6 py-4 text-right font-medium text-green-600">
                    {r.consumption != null
                      ? r.consumption.toLocaleString("de-DE")
                      : "–"}
                  </td>
                  <td className="px-6 py-4 text-center">
                    <button
                      onClick={() => handleDelete(r.id)}
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
        </div>
      )}
    </div>
  );
}
