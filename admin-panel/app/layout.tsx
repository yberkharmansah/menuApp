import "./globals.css";

export const metadata = {
  title: "MenuApp Admin",
  description: "Firma paneli için yönetim arayüzü"
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="tr">
      <body>
        <header>
          <div className="brand">MenuApp Admin</div>
          <span className="pill">Tenant: Cafe Luna</span>
        </header>
        {children}
      </body>
    </html>
  );
}
