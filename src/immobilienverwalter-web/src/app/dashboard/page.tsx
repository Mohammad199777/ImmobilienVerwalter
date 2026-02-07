"use client";
import { useEffect, useState } from "react";
import { dashboardApi } from "@/lib/api";
import { getErrorMessage } from "@/lib/useToast";
import { useAppToast } from "./layout";
import {
  Building2,
  Home,
  Users,
  Euro,
  AlertTriangle,
  TrendingUp,
  TrendingDown,
  Percent,
} from "lucide-react";

interface DashboardData {
  totalProperties: number;
  totalUnits: number;
  occupiedUnits: number;
  vacantUnits: number;
  occupancyRate: number;
  monthlyIncome: number;
  monthlyExpenses: number;
  monthlyProfit: number;
  yearlyIncome: number;
  yearlyExpenses: number;
  overduePayments: number;
  expiringLeases: number;
}

function StatCard({
  icon: Icon,
  label,
  value,
  sub,
  color,
}: {
  icon: React.ElementType;
  label: string;
  value: string;
  sub?: string;
  color: string;
}) {
  return (
    <div className="bg-white rounded-xl shadow-sm border p-6">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm text-gray-500">{label}</p>
          <p className="text-2xl font-bold mt-1">{value}</p>
          {sub && <p className="text-xs text-gray-400 mt-1">{sub}</p>}
        </div>
        <div className={`p-3 rounded-lg ${color}`}>
          <Icon size={24} className="text-white" />
        </div>
      </div>
    </div>
  );
}

export default function DashboardPage() {
  const toast = useAppToast();
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    dashboardApi
      .get()
      .then((res) => setData(res.data))
      .catch((err) => toast(getErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [toast]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (!data) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Dashboard konnte nicht geladen werden.</p>
      </div>
    );
  }

  const formatCurrency = (n: number) =>
    new Intl.NumberFormat("de-DE", {
      style: "currency",
      currency: "EUR",
    }).format(n);

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Dashboard</h1>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatCard
          icon={Building2}
          label="Immobilien"
          value={data.totalProperties.toString()}
          color="bg-blue-500"
        />
        <StatCard
          icon={Home}
          label="Einheiten"
          value={`${data.occupiedUnits} / ${data.totalUnits}`}
          sub={`${data.vacantUnits} leer`}
          color="bg-green-500"
        />
        <StatCard
          icon={Percent}
          label="Auslastung"
          value={`${data.occupancyRate}%`}
          color="bg-purple-500"
        />
        <StatCard
          icon={AlertTriangle}
          label="Überfällige Zahlungen"
          value={data.overduePayments.toString()}
          color={data.overduePayments > 0 ? "bg-red-500" : "bg-gray-400"}
        />
      </div>

      {/* Finanzen */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-8">
        <div className="bg-white rounded-xl shadow-sm border p-6">
          <div className="flex items-center gap-2 mb-2">
            <TrendingUp size={20} className="text-green-500" />
            <h3 className="font-medium text-gray-700">Monatliche Einnahmen</h3>
          </div>
          <p className="text-3xl font-bold text-green-600">
            {formatCurrency(data.monthlyIncome)}
          </p>
          <p className="text-sm text-gray-400 mt-1">
            Jährlich: {formatCurrency(data.yearlyIncome)}
          </p>
        </div>

        <div className="bg-white rounded-xl shadow-sm border p-6">
          <div className="flex items-center gap-2 mb-2">
            <TrendingDown size={20} className="text-red-500" />
            <h3 className="font-medium text-gray-700">Monatliche Ausgaben</h3>
          </div>
          <p className="text-3xl font-bold text-red-600">
            {formatCurrency(data.monthlyExpenses)}
          </p>
          <p className="text-sm text-gray-400 mt-1">
            Jährlich: {formatCurrency(data.yearlyExpenses)}
          </p>
        </div>

        <div className="bg-white rounded-xl shadow-sm border p-6">
          <div className="flex items-center gap-2 mb-2">
            <Euro size={20} className="text-blue-500" />
            <h3 className="font-medium text-gray-700">Monatlicher Gewinn</h3>
          </div>
          <p
            className={`text-3xl font-bold ${data.monthlyProfit >= 0 ? "text-green-600" : "text-red-600"}`}
          >
            {formatCurrency(data.monthlyProfit)}
          </p>
        </div>
      </div>

      {/* Warnungen */}
      {(data.overduePayments > 0 || data.expiringLeases > 0) && (
        <div className="bg-amber-50 border border-amber-200 rounded-xl p-6">
          <h3 className="font-bold text-amber-800 mb-3 flex items-center gap-2">
            <AlertTriangle size={20} />
            Handlungsbedarf
          </h3>
          <ul className="space-y-2 text-amber-700">
            {data.overduePayments > 0 && (
              <li>⚠️ {data.overduePayments} überfällige Zahlung(en)</li>
            )}
            {data.expiringLeases > 0 && (
              <li>
                📋 {data.expiringLeases} Mietvertrag/Mietverträge laufen in den
                nächsten 90 Tagen aus
              </li>
            )}
          </ul>
        </div>
      )}

      {/* Quick Info */}
      <div className="mt-8 bg-white rounded-xl shadow-sm border p-6">
        <h3 className="font-bold text-gray-900 mb-4 flex items-center gap-2">
          <Users size={20} />
          Schnellübersicht
        </h3>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-center">
          <div>
            <p className="text-2xl font-bold text-blue-600">
              {data.totalProperties}
            </p>
            <p className="text-sm text-gray-500">Immobilien</p>
          </div>
          <div>
            <p className="text-2xl font-bold text-green-600">
              {data.occupiedUnits}
            </p>
            <p className="text-sm text-gray-500">Vermietet</p>
          </div>
          <div>
            <p className="text-2xl font-bold text-orange-600">
              {data.vacantUnits}
            </p>
            <p className="text-sm text-gray-500">Leerstand</p>
          </div>
          <div>
            <p className="text-2xl font-bold text-purple-600">
              {data.expiringLeases}
            </p>
            <p className="text-sm text-gray-500">Auslaufende Verträge</p>
          </div>
        </div>
      </div>
    </div>
  );
}
