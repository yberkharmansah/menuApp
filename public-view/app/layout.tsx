import "./globals.css";

export const metadata = {
  title: "MenuApp - Cafe Luna",
  description: "Cafe Luna dijital menüsü"
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="tr">
      <body>{children}</body>
    </html>
  );
}
