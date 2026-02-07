"use client";
import { useEffect, useState, useCallback } from "react";
import { meterReadingsApi, propertiesApi, unitsApi } from "@/lib/api";
import { getErrorMessage } from "@/lib/useToast";
import { useAppToast } from "../layout";
import { Gauge, Plus, Trash2, X } from "lucide-react";

interface MeterReading {
  id: string;
  meterType: number;
  meterNumber: string;
  value: number;
  previousValue?: number;
  readingDate: string;
  notes?: string;
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

const meterTypeLabels = ["Strom", "Gas", "Wasser", "Heizung", "Sonstige"];

const emptyForm = {
  unitId: "",
  meterType: 0,
  meterNumber: "",
  value: 0,
  readingDate: new Date().toISOString().split("T")[0],
  notes: "",
};

export default function MetersPage() {
  const toast = useAppToast();
  const [readings, setReadings] = useState<MeterReading[]>([]);
  const [properties, setProperties] = useState<PropertyOption[]>([]);
  const [units, setUnits] = useState<UnitOption[]>([]);
  const [filteredUnits, setFilteredUnits] = useState<UnitOption[]>([]);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [showCreate, setShowCreate] = useState(false);
  const [selectedProperty, setSelectedProperty] = useState("");
  const [selectedUnit, setSelectedUnit] = useState("");
  const [form, setForm] = useState(emptyForm);

  // Load properties and units on mount
  useEffect(() => {
    (async () => {
      try {
        const [propRes, unitRes] = await Promise.all([
          propertiesApi.getAll(),
          unitsApi.getAll(),
        ]);
        setProperties(propRes.data);
        setUnits(unitRes.data);
      } catch (err) {
        toast(getErrorMessage(err));
      }
    })();
  }, [toast]);

  // Filter units based on selected property
  useEffect(() => {
    setFilteredUnits(
      selectedProperty
        ? units.filter((u) => u.propertyId === selectedProperty)
        : units,
    );
    if (selectedProperty && selectedUnit) {
      const belongs = units.some(
        (u) => u.id === selectedUnit && u.propertyId === selectedProperty,
      );
      if (!belongs) setSelectedUnit("");
    }
  }, [selectedProperty, selectedUnit, units]);

  // Load readings when unit is selected
  const loadReadings = useCallback(
    async (unitId: string) => {
      if (!unitId) {
        setReadings([]);
        return;
      }
      setLoading(true);
      try {
        const res = await meterReadingsApi.getByUnit(unitId);
        setReadings(res.data);
      } catch (err) {
        toast(getErrorMessage(err));
      } finally {
        setLoading(false);
      }
    },
    [toast],
  );

  useEffect(() => {
    loadReadings(selectedUnit);
  }, [selectedUnit, loadReadings]);

  const openCreate = () => {
    setForm({ ...emptyForm, unitId: selectedUnit });
    setShowCreate(true);
  };

  const handleCreate = async () => {
    setSubmitting(true);
    try {
      await meterReadingsApi.create({
        unitId: form.unitId,
        meterType: form.meterType,
        meterNumber: form.meterNumber,
        value: form.value,
        readingDate: form.readingDate,
        notes: form.notes || undefined,
      });
      toast("Zählerstand erfasst.", "success");
      setShowCreate(false);
      loadReadings(selectedUnit);
    } catch (err) {
      toast(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Zählerstand wirklich löschen?")) return;
    try {
      await meterReadingsApi.delete(id);
      toast("Zählerstand gelöscht.", "success");
      loadReadings(selectedUnit);
    } catch (err) {
      toast(getErrorMessage(err));
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Zählerstände</h1>
        <button
          onClick={openCreate}
          disabled={!selectedUnit}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-blue-700 disabled:opacity-50"
        >
          <Plus size={18} /> Neuer Zählerstand
        </button>
      </div>

      {/* Filter by property → unit */}
      <div className="flex gap-3 mb-6">
        <select
          className="px-3 py-2 border rounded-lg"
          value={selectedProperty}
          onChange={(e) => {
            setSelectedProperty(e.target.value);
            setSelectedUnit("");
          }}
          aria-label="Immobilie filtern"
        >
          <option value="">Alle Immobilien</option>
          {properties.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name}
            </option>
          ))}
        </select>
        <select
          className="px-3 py-2 border rounded-lg"
          value={selectedUnit}
          onChange={(e) => setSelectedUnit(e.target.value)}
          aria-label="Einheit auswählen"
        >
          <option value="">Einheit wählen…</option>
          {filteredUnits.map((u) => (
            <option key={u.id} value={u.id}>
              {u.name}
            </option>
          ))}
        </select>
      </div>

      {!selectedUnit ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Gauge size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">
            Bitte wählen Sie eine Einheit aus, um Zählerstände anzuzeigen.
          </p>
        </div>
      ) : loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : readings.length === 0 ? (
        <div className="text-center py-16 bg-white rounded-xl border">
          <Gauge size={48} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500">Keine Zählerstände für diese Einheit.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="text-left px-6 py-3 text-sm font-medium text-gray-500">
                  Typ
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
              {readings.map((r) => {
                const consumption =
                  r.previousValue != null ? r.value - r.previousValue : null;
                return (
                  <tr key={r.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 font-medium text-gray-900">
                      {meterTypeLabels[r.meterType] ?? "Unbekannt"}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500 font-mono">
                      {r.meterNumber}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500">
                      {new Date(r.readingDate).toLocaleDateString("de-DE")}
                    </td>
                    <td className="px-6 py-4 text-right font-medium">
                      {r.value.toLocaleString("de-DE")}
                    </td>
                    <td className="px-6 py-4 text-right text-sm text-gray-400">
                      {r.previousValue?.toLocaleString("de-DE") ?? "–"}
                    </td>
                    <td className="px-6 py-4 text-right">
                      {consumption != null ? (
                        <span className="font-medium text-blue-600">
                          {consumption.toLocaleString("de-DE")}
                        </span>
                      ) : (
                        "–"
                      )}
                    </td>
                    <td className="px-6 py-4 text-center">
                      <button
                        onClick={() => handleDelete(r.id)}
                        className="text-red-500 hover:text-red-700"
                        aria-label="Löschen"
                      >
                        <Trash2 size={16} />
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {/* CREATE MODAL */}
      {showCreate && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-6 w-full max-w-md">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">Neuer Zählerstand</h2>
              <button
                onClick={() => setShowCreate(false)}
                aria-label="Schließen"
              >
                <X size={20} />
              </button>
            </div>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Zählertyp *
                </label>
                <select
                  value={form.meterType}
                  onChange={(e) =>
                    setForm({ ...form, meterType: +e.target.value })
                  }
                  className="w-full border rounded-lg px-3 py-2"
                >
                  {meterTypeLabels.map((l, i) => (
                    <option key={i} value={i}>
                      {l}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Zählernummer *
                </label>
                <input
                  type="text"
                  value={form.meterNumber}
                  onChange={(e) =>
                    setForm({ ...form, meterNumber: e.target.value })
                  }
                  className="w-full border rounded-lg px-3 py-2"
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Zählerstand *
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    min={0}
                    value={form.value || ""}
                    onChange={(e) =>
                      setForm({ ...form, value: +e.target.value })
                    }
                    className="w-full border rounded-lg px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Ablesedatum *
                  </label>
                  <input
                    type="date"
                    value={form.readingDate}
                    onChange={(e) =>
                      setForm({ ...form, readingDate: e.target.value })
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
                disabled={!form.meterNumber || !form.value || submitting}
                className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50"
              >
                {submitting ? "Erfassen…" : "Zählerstand erfassen"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
