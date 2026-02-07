"use client";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useState, useEffect, createContext, useContext } from "react";
import {
  LayoutDashboard,
  Building2,
  Home,
  Users,
  FileText,
  Euro,
  Receipt,
  Gauge,
  LogOut,
  Menu,
  X,
} from "lucide-react";
import { useToast } from "@/lib/useToast";
import ToastContainer from "@/app/components/ToastContainer";

const navItems = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/dashboard/properties", label: "Immobilien", icon: Building2 },
  { href: "/dashboard/units", label: "Einheiten", icon: Home },
  { href: "/dashboard/tenants", label: "Mieter", icon: Users },
  { href: "/dashboard/leases", label: "Mietverträge", icon: FileText },
  { href: "/dashboard/payments", label: "Zahlungen", icon: Euro },
  { href: "/dashboard/expenses", label: "Ausgaben", icon: Receipt },
  { href: "/dashboard/meters", label: "Zählerstände", icon: Gauge },
];

// Toast-Context für untergeordnete Seiten
type ToastFn = (message: string, type?: "success" | "error" | "info") => void;
const ToastContext = createContext<ToastFn>(() => {});
export const useAppToast = () => useContext(ToastContext);

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const router = useRouter();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const { toasts, addToast, removeToast } = useToast();
  const [user] = useState<{ fullName: string; email: string } | null>(() => {
    if (typeof window === "undefined") return null;
    const userData = localStorage.getItem("user");
    return userData ? JSON.parse(userData) : null;
  });

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      router.push("/login");
    }
  }, [router]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    router.push("/login");
  };

  return (
    <ToastContext.Provider value={addToast}>
      <div className="min-h-screen bg-gray-50">
        <ToastContainer toasts={toasts} onRemove={removeToast} />

        {/* Mobile Sidebar Overlay */}
        {sidebarOpen && (
          <div
            className="fixed inset-0 bg-black/50 z-40 lg:hidden"
            onClick={() => setSidebarOpen(false)}
          />
        )}

        {/* Sidebar */}
        <aside
          className={`fixed top-0 left-0 z-50 h-full w-64 bg-white shadow-lg transform transition-transform lg:translate-x-0 ${
            sidebarOpen ? "translate-x-0" : "-translate-x-full"
          }`}
        >
          <div className="p-6 border-b">
            <h1 className="text-xl font-bold text-gray-900">
              🏠 ImmobilienVerwalter
            </h1>
          </div>

          <nav className="p-4 space-y-1" aria-label="Hauptnavigation">
            {navItems.map((item) => {
              const isActive = pathname === item.href;
              const Icon = item.icon;
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  onClick={() => setSidebarOpen(false)}
                  aria-current={isActive ? "page" : undefined}
                  className={`flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors ${
                    isActive
                      ? "bg-blue-50 text-blue-700 font-medium"
                      : "text-gray-600 hover:bg-gray-100"
                  }`}
                >
                  <Icon size={20} aria-hidden="true" />
                  {item.label}
                </Link>
              );
            })}
          </nav>

          <div className="absolute bottom-0 w-full p-4 border-t">
            {user && (
              <div className="mb-3 px-3">
                <p className="font-medium text-gray-900 text-sm">
                  {user.fullName}
                </p>
                <p className="text-xs text-gray-500">{user.email}</p>
              </div>
            )}
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg w-full transition-colors"
            >
              <LogOut size={18} aria-hidden="true" />
              Abmelden
            </button>
          </div>
        </aside>

        {/* Main Content */}
        <div className="lg:ml-64">
          {/* Mobile Header */}
          <header className="lg:hidden bg-white shadow-sm p-4 flex items-center justify-between">
            <button
              onClick={() => setSidebarOpen(true)}
              aria-label="Menü öffnen"
            >
              {sidebarOpen ? <X size={24} /> : <Menu size={24} />}
            </button>
            <h1 className="font-bold text-gray-900">🏠 ImmobilienVerwalter</h1>
            <div className="w-6" />
          </header>

          <main className="p-6">{children}</main>
        </div>
      </div>
    </ToastContext.Provider>
  );
}
