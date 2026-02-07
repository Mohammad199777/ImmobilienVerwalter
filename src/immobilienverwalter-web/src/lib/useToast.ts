"use client";
import { useState, useCallback } from "react";

export interface Toast {
  id: number;
  message: string;
  type: "success" | "error" | "info";
}

let nextId = 0;

export function useToast() {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const addToast = useCallback(
    (message: string, type: Toast["type"] = "error") => {
      const id = nextId++;
      setToasts((prev) => [...prev, { id, message, type }]);
      setTimeout(() => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
      }, 5000);
    },
    [],
  );

  const removeToast = useCallback((id: number) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  return { toasts, addToast, removeToast };
}

/**
 * Extracts a user-friendly error message from an API error response.
 */
export function getErrorMessage(err: unknown): string {
  const error = err as {
    response?: {
      data?: {
        message?: string;
        title?: string;
        errors?: Record<string, string[]>;
      };
    };
    message?: string;
  };

  if (error.response?.data?.errors) {
    const allErrors = Object.values(error.response.data.errors).flat();
    if (allErrors.length > 0) return allErrors.join(" ");
  }

  return (
    error.response?.data?.message ||
    error.response?.data?.title ||
    error.message ||
    "Ein unerwarteter Fehler ist aufgetreten."
  );
}
