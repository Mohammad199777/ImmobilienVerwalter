import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

export function middleware(request: NextRequest) {
  const token = request.cookies.get("token")?.value;
  const { pathname } = request.nextUrl;

  // Dashboard-Routen schützen (client-side check via localStorage bleibt als Fallback)
  // Server-side können wir nur Cookies prüfen; das Token liegt aber in localStorage.
  // Daher redirect nur wenn kein Cookie-basiertes Token gesetzt ist.
  // Die primäre Auth-Prüfung bleibt client-side im Dashboard Layout.

  // Login-Seite: Wenn bereits authentifiziert, zum Dashboard weiterleiten
  if (pathname === "/login" && token) {
    return NextResponse.redirect(new URL("/dashboard", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/dashboard/:path*", "/login"],
};
