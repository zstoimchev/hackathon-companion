import type { Metadata } from "next";
import "./globals.css";
import { NavBar } from "@/components/layout/NavBar";

export const metadata: Metadata = {
  title: "Hackathon OS",
  description: "Fair, bias-corrected hackathon judging & mentor platform",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className="h-full antialiased">
      <body className="min-h-full flex flex-col bg-gray-50 text-gray-900 font-sans">
        <NavBar />
        <main className="flex-1">{children}</main>
      </body>
    </html>
  );
}
