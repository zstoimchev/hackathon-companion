"use client";

import { create } from "zustand";
import { AuthResponse } from "@/types";

interface AuthState {
  user: AuthResponse | null;
  setUser: (user: AuthResponse | null) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,

  setUser: (user) => {
    if (user) {
      localStorage.setItem("token", user.token);
      localStorage.setItem("user", JSON.stringify(user));
    }
    set({ user });
  },

  logout: () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    set({ user: null });
  },
}));

// Hydrate from localStorage (call in root layout)
export function hydrateAuth() {
  if (typeof window === "undefined") return;
  const raw = localStorage.getItem("user");
  if (raw) {
    try {
      useAuthStore.getState().setUser(JSON.parse(raw));
    } catch {
      localStorage.removeItem("user");
    }
  }
}
