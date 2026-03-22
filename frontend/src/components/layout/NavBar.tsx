"use client";

import Link from "next/link";
import { useEffect } from "react";
import { hydrateAuth, useAuthStore } from "@/store/auth";
import { useRouter } from "next/navigation";

export function NavBar() {
  const { user, logout } = useAuthStore();
  const router = useRouter();

  useEffect(() => {
    hydrateAuth();
  }, []);

  function handleLogout() {
    logout();
    router.push("/login");
  }

  return (
    <nav className="bg-indigo-700 text-white shadow">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex items-center justify-between h-16">
        <Link href="/" className="text-xl font-bold tracking-tight">
          🏆 Hackathon OS
        </Link>
        <div className="flex items-center gap-4 text-sm">
          {user ? (
            <>
              {user.role === "Admin" && (
                <Link href="/admin" className="hover:underline">
                  Admin
                </Link>
              )}
              {(user.role === "Judge" || user.role === "Admin") && (
                <Link href="/judge" className="hover:underline">
                  Judge
                </Link>
              )}
              {(user.role === "Mentor" || user.role === "Admin") && (
                <Link href="/mentor" className="hover:underline">
                  Mentor Queue
                </Link>
              )}
              <Link href="/leaderboard" className="hover:underline">
                Leaderboard
              </Link>
              <span className="opacity-70">{user.name}</span>
              <button
                onClick={handleLogout}
                className="bg-indigo-900 hover:bg-indigo-800 px-3 py-1 rounded"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link href="/login" className="hover:underline">
                Login
              </Link>
              <Link
                href="/register"
                className="bg-white text-indigo-700 px-3 py-1 rounded font-semibold hover:bg-indigo-50"
              >
                Register
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
