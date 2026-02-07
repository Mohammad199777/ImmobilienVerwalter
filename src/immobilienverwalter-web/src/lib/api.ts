import axios from "axios";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_URL || "https://localhost:5001/api";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
});

// Token automatisch anhängen
api.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

// Bei 401 zum Login weiterleiten
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401 && typeof window !== "undefined") {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);

// === Auth ===
export const authApi = {
  login: (data: { email: string; password: string }) =>
    api.post("/auth/login", data),
  register: (data: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    phone?: string;
    company?: string;
  }) => api.post("/auth/register", data),
};

// === Dashboard ===
export const dashboardApi = {
  get: () => api.get("/dashboard"),
};

// === Properties ===
export const propertiesApi = {
  getAll: () => api.get("/properties"),
  getById: (id: string) => api.get(`/properties/${id}`),
  create: (data: PropertyCreate) => api.post("/properties", data),
  update: (id: string, data: PropertyCreate) =>
    api.put(`/properties/${id}`, data),
  delete: (id: string) => api.delete(`/properties/${id}`),
};

// === Units ===
export const unitsApi = {
  getAll: () => api.get("/units"),
  getByProperty: (propertyId: string) =>
    api.get(`/units/property/${propertyId}`),
  getById: (id: string) => api.get(`/units/${id}`),
  getVacant: (propertyId?: string) =>
    api.get("/units/vacant", { params: { propertyId } }),
  create: (data: UnitCreate) => api.post("/units", data),
  update: (id: string, data: UnitUpdate) => api.put(`/units/${id}`, data),
  delete: (id: string) => api.delete(`/units/${id}`),
};

// === Tenants ===
export const tenantsApi = {
  getAll: () => api.get("/tenants"),
  getById: (id: string) => api.get(`/tenants/${id}`),
  search: (q: string) => api.get("/tenants/search", { params: { q } }),
  create: (data: TenantCreate) => api.post("/tenants", data),
  update: (id: string, data: TenantUpdate) => api.put(`/tenants/${id}`, data),
  delete: (id: string) => api.delete(`/tenants/${id}`),
};

// === Leases ===
export const leasesApi = {
  getAll: () => api.get("/leases"),
  getActive: () => api.get("/leases/active"),
  getById: (id: string) => api.get(`/leases/${id}`),
  getByUnit: (unitId: string) => api.get(`/leases/unit/${unitId}`),
  getExpiring: (days?: number) =>
    api.get("/leases/expiring", { params: { days } }),
  create: (data: LeaseCreate) => api.post("/leases", data),
  update: (id: string, data: LeaseUpdate) => api.put(`/leases/${id}`, data),
  delete: (id: string) => api.delete(`/leases/${id}`),
};

// === Payments ===
export const paymentsApi = {
  getByLease: (leaseId: string) => api.get(`/payments/lease/${leaseId}`),
  getOverdue: () => api.get("/payments/overdue"),
  getByMonth: (year: number, month: number) =>
    api.get(`/payments/month/${year}/${month}`),
  getById: (id: string) => api.get(`/payments/${id}`),
  create: (data: PaymentCreate) => api.post("/payments", data),
  update: (id: string, data: PaymentUpdate) => api.put(`/payments/${id}`, data),
  delete: (id: string) => api.delete(`/payments/${id}`),
};

// === Expenses ===
export const expensesApi = {
  getAll: () => api.get("/expenses"),
  getByProperty: (propertyId: string) =>
    api.get(`/expenses/property/${propertyId}`),
  getById: (id: string) => api.get(`/expenses/${id}`),
  create: (data: ExpenseCreate) => api.post("/expenses", data),
  update: (id: string, data: ExpenseUpdate) => api.put(`/expenses/${id}`, data),
  delete: (id: string) => api.delete(`/expenses/${id}`),
};

// === Meter Readings ===
export const meterReadingsApi = {
  getByUnit: (unitId: string) => api.get(`/meterreadings/unit/${unitId}`),
  getById: (id: string) => api.get(`/meterreadings/${id}`),
  create: (data: MeterReadingCreate) => api.post("/meterreadings", data),
  update: (id: string, data: MeterReadingUpdate) =>
    api.put(`/meterreadings/${id}`, data),
  delete: (id: string) => api.delete(`/meterreadings/${id}`),
};

// === Types ===
export interface PropertyCreate {
  name: string;
  street: string;
  houseNumber: string;
  zipCode: string;
  city: string;
  country?: string;
  yearBuilt?: number;
  totalArea?: number;
  numberOfFloors?: number;
  type: number;
  purchasePrice?: number;
  purchaseDate?: string;
}

export interface UnitCreate {
  name: string;
  description?: string;
  floor?: number;
  area: number;
  rooms?: number;
  type: number;
  targetRent: number;
  propertyId: string;
}

export interface UnitUpdate extends Omit<UnitCreate, "propertyId"> {
  status: number;
}

export interface TenantCreate {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  mobilePhone?: string;
  previousAddress?: string;
  iban?: string;
  bic?: string;
  bankName?: string;
  dateOfBirth?: string;
  occupation?: string;
  monthlyIncome?: number;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  notes?: string;
}

export type TenantUpdate = TenantCreate;

export interface LeaseCreate {
  tenantId: string;
  unitId: string;
  startDate: string;
  endDate?: string;
  coldRent: number;
  additionalCosts: number;
  depositAmount: number;
  noticePeriodMonths: number;
  paymentDayOfMonth: number;
  notes?: string;
}

export interface LeaseUpdate {
  coldRent: number;
  additionalCosts: number;
  depositAmount: number;
  depositPaid: number;
  depositStatus: number;
  status: number;
  endDate?: string;
  terminationDate?: string;
  moveOutDate?: string;
  noticePeriodMonths: number;
  paymentDayOfMonth: number;
  notes?: string;
}

export interface PaymentCreate {
  leaseId: string;
  amount: number;
  paymentDate: string;
  dueDate: string;
  paymentMonth: number;
  paymentYear: number;
  type: number;
  method: number;
  status?: number;
  reference?: string;
  notes?: string;
  expectedAmount?: number;
}

export interface PaymentUpdate {
  amount: number;
  paymentDate: string;
  dueDate: string;
  type: number;
  method: number;
  status: number;
  reference?: string;
  notes?: string;
}

export interface ExpenseCreate {
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
}

export interface ExpenseUpdate {
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
}

export interface MeterReadingCreate {
  unitId: string;
  meterType: number;
  meterNumber: string;
  value: number;
  readingDate: string;
  notes?: string;
  photoPath?: string;
}

export interface MeterReadingUpdate {
  meterType: number;
  meterNumber: string;
  value: number;
  readingDate: string;
  notes?: string;
}

export default api;
