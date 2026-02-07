"use client";

import { useEffect, useMemo, useState } from "react";

type Menu = {
  id: string;
  name: string;
  description: string;
  isPublished: boolean;
};

type ThemeSettings = {
  logoUrl: string;
  primaryColor: string;
  secondaryColor: string;
  font: string;
  currency: string;
  language: string;
};

type Asset = {
  id: string;
  title: string;
  url: string;
  type: "image" | "logo";
};

type CdnSettings = {
  provider: string;
  baseUrl: string;
  enabled: boolean;
  autoOptimize: boolean;
};

type AuthState = {
  companyName: string;
  slug: string;
  email: string;
};

const initialTheme: ThemeSettings = {
  logoUrl: "",
  primaryColor: "#5b21b6",
  secondaryColor: "#0f172a",
  font: "Inter",
  currency: "TRY",
  language: "tr"
};

const initialCdn: CdnSettings = {
  provider: "Cloudflare",
  baseUrl: "https://cdn.menuapp.com",
  enabled: true,
  autoOptimize: true
};

const menuTemplate = (): Menu => ({
  id: crypto.randomUUID(),
  name: "Ana Menü",
  description: "Kahvaltı, öğle ve tatlı kategorileri.",
  isPublished: false
});

export default function AdminPanel() {
  const [authTab, setAuthTab] = useState<"login" | "register">("login");
  const [authState, setAuthState] = useState<AuthState | null>(null);
  const [menus, setMenus] = useState<Menu[]>([menuTemplate()]);
  const [activeMenuId, setActiveMenuId] = useState<string>(menus[0].id);
  const [theme, setTheme] = useState<ThemeSettings>(initialTheme);
  const [assets, setAssets] = useState<Asset[]>([]);
  const [cdnSettings, setCdnSettings] = useState<CdnSettings>(initialCdn);
  const [statusMessage, setStatusMessage] = useState<string>("");

  const activeMenu = useMemo(
    () => menus.find((menu) => menu.id === activeMenuId),
    [menus, activeMenuId]
  );

  useEffect(() => {
    const stored = localStorage.getItem("menuapp-admin");
    if (stored) {
      const parsed = JSON.parse(stored) as {
        auth: AuthState;
        menus: Menu[];
        theme: ThemeSettings;
        assets: Asset[];
        cdn: CdnSettings;
      };
      setAuthState(parsed.auth);
      setMenus(parsed.menus);
      setActiveMenuId(parsed.menus[0]?.id ?? "");
      setTheme(parsed.theme);
      setAssets(parsed.assets);
      setCdnSettings(parsed.cdn);
    }
  }, []);

  useEffect(() => {
    if (!authState) return;
    localStorage.setItem(
      "menuapp-admin",
      JSON.stringify({
        auth: authState,
        menus,
        theme,
        assets,
        cdn: cdnSettings
      })
    );
  }, [authState, menus, theme, assets, cdnSettings]);

  const handleRegister = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const companyName = String(formData.get("companyName"));
    const email = String(formData.get("email"));
    const slug = companyName.trim().toLowerCase().replace(/\s+/g, "-");
    setAuthState({ companyName, email, slug });
    setStatusMessage("Hesabınız oluşturuldu. Yönetim paneline yönlendirildiniz.");
  };

  const handleLogin = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const companyName = String(formData.get("companyName"));
    const email = String(formData.get("email"));
    const slug = companyName.trim().toLowerCase().replace(/\s+/g, "-");
    setAuthState({ companyName, email, slug });
    setStatusMessage("Giriş başarılı. Yönetim paneline hoş geldiniz.");
  };

  const handleCreateMenu = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const name = String(formData.get("menuName"));
    const description = String(formData.get("menuDescription"));
    const newMenu: Menu = {
      id: crypto.randomUUID(),
      name,
      description,
      isPublished: false
    };
    setMenus((prev) => [newMenu, ...prev]);
    setActiveMenuId(newMenu.id);
    setStatusMessage("Yeni menü oluşturuldu.");
    event.currentTarget.reset();
  };

  const handleUpdateMenu = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!activeMenu) return;
    const formData = new FormData(event.currentTarget);
    const name = String(formData.get("editMenuName"));
    const description = String(formData.get("editMenuDescription"));
    setMenus((prev) =>
      prev.map((menu) =>
        menu.id === activeMenu.id ? { ...menu, name, description } : menu
      )
    );
    setStatusMessage("Menü güncellendi.");
  };

  const handlePublishMenu = () => {
    if (!activeMenu) return;
    setMenus((prev) =>
      prev.map((menu) =>
        menu.id === activeMenu.id ? { ...menu, isPublished: !menu.isPublished } : menu
      )
    );
    setStatusMessage(
      activeMenu.isPublished ? "Menü yayından kaldırıldı." : "Menü yayınlandı."
    );
  };

  const handleThemeSave = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    setTheme({
      logoUrl: String(formData.get("logoUrl")),
      primaryColor: String(formData.get("primaryColor")),
      secondaryColor: String(formData.get("secondaryColor")),
      font: String(formData.get("font")),
      currency: String(formData.get("currency")),
      language: String(formData.get("language"))
    });
    setStatusMessage("Tema ayarları kaydedildi.");
  };

  const handleAddAsset = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    const title = String(formData.get("assetTitle"));
    const url = String(formData.get("assetUrl"));
    const type = String(formData.get("assetType")) as Asset["type"];
    setAssets((prev) => [
      {
        id: crypto.randomUUID(),
        title,
        url,
        type
      },
      ...prev
    ]);
    event.currentTarget.reset();
    setStatusMessage("Görsel eklendi.");
  };

  const handleRemoveAsset = (id: string) => {
    setAssets((prev) => prev.filter((asset) => asset.id !== id));
    setStatusMessage("Görsel kaldırıldı.");
  };

  const handleCdnSave = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const formData = new FormData(event.currentTarget);
    setCdnSettings({
      provider: String(formData.get("provider")),
      baseUrl: String(formData.get("baseUrl")),
      enabled: formData.get("enabled") === "on",
      autoOptimize: formData.get("autoOptimize") === "on"
    });
    setStatusMessage("CDN ayarları güncellendi.");
  };

  if (!authState) {
    return (
      <main className="auth">
        <section className="auth-card">
          <div className="auth-header">
            <div>
              <h1>MenuApp Admin</h1>
              <p className="subtle">Firmanızın dijital menüsünü yönetin.</p>
            </div>
            <div className="auth-tabs">
              <button
                type="button"
                className={authTab === "login" ? "active" : ""}
                onClick={() => setAuthTab("login")}
              >
                Giriş
              </button>
              <button
                type="button"
                className={authTab === "register" ? "active" : ""}
                onClick={() => setAuthTab("register")}
              >
                Kayıt Ol
              </button>
            </div>
          </div>

          {authTab === "login" ? (
            <form className="form" onSubmit={handleLogin}>
              <label>
                Firma adı
                <input name="companyName" placeholder="Cafe Luna" required />
              </label>
              <label>
                E-posta
                <input type="email" name="email" placeholder="admin@cafeluna.com" required />
              </label>
              <label>
                Şifre
                <input type="password" name="password" placeholder="••••••••" required />
              </label>
              <button className="button" type="submit">
                Giriş Yap
              </button>
            </form>
          ) : (
            <form className="form" onSubmit={handleRegister}>
              <label>
                Firma adı
                <input name="companyName" placeholder="Cafe Luna" required />
              </label>
              <label>
                E-posta
                <input type="email" name="email" placeholder="admin@cafeluna.com" required />
              </label>
              <label>
                Şifre
                <input type="password" name="password" placeholder="••••••••" required />
              </label>
              <button className="button" type="submit">
                Hesap Oluştur
              </button>
            </form>
          )}
        </section>
      </main>
    );
  }

  return (
    <main>
      <section className="hero-card">
        <div>
          <h2>Hoş geldin, {authState.companyName}</h2>
          <p className="subtle">
            Aktif tenant: <strong>{authState.slug}</strong>
          </p>
        </div>
        <div className="actions">
          <button className="button outline" type="button" onClick={handlePublishMenu}>
            {activeMenu?.isPublished ? "Yayından Kaldır" : "Yayınla"}
          </button>
          <button
            className="button"
            type="button"
            onClick={() => {
              localStorage.removeItem("menuapp-admin");
              setAuthState(null);
            }}
          >
            Çıkış
          </button>
        </div>
      </section>

      {statusMessage ? <div className="status">{statusMessage}</div> : null}

      <section className="grid">
        <div className="card">
          <h3>Menü Oluştur</h3>
          <form className="form" onSubmit={handleCreateMenu}>
            <label>
              Menü adı
              <input name="menuName" placeholder="Öğle Menüsü" required />
            </label>
            <label>
              Açıklama
              <textarea name="menuDescription" rows={3} placeholder="Menü notu" />
            </label>
            <button className="button" type="submit">
              Oluştur
            </button>
          </form>
        </div>

        <div className="card">
          <h3>Menü Düzenle</h3>
          <form className="form" onSubmit={handleUpdateMenu}>
            <label>
              Menü seç
              <select
                value={activeMenuId}
                onChange={(event) => setActiveMenuId(event.target.value)}
              >
                {menus.map((menu) => (
                  <option key={menu.id} value={menu.id}>
                    {menu.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Menü adı
              <input
                name="editMenuName"
                defaultValue={activeMenu?.name ?? ""}
                placeholder="Menü adı"
                required
              />
            </label>
            <label>
              Açıklama
              <textarea
                name="editMenuDescription"
                rows={3}
                defaultValue={activeMenu?.description ?? ""}
              />
            </label>
            <button className="button" type="submit">
              Güncelle
            </button>
          </form>
        </div>

        <div className="card">
          <h3>Yayın Durumu</h3>
          <p className="subtle">
            {activeMenu?.isPublished
              ? "Menü yayında. Public linkiniz aktif."
              : "Menü yayında değil. Yayınlamak için butonu kullanın."}
          </p>
          <div className="list">
            <div className="list-item">
              <span>Public link</span>
              <span className="badge">
                https://menuapp.com/b/{authState.slug}
              </span>
            </div>
            <div className="list-item">
              <span>QR Durumu</span>
              <span className="badge">{activeMenu?.isPublished ? "Hazır" : "Beklemede"}</span>
            </div>
          </div>
        </div>
      </section>

      <section className="section grid">
        <div className="card">
          <h3>Tema Ayarları</h3>
          <form className="form" onSubmit={handleThemeSave}>
            <label>
              Logo URL
              <input name="logoUrl" defaultValue={theme.logoUrl} placeholder="https://..." />
            </label>
            <label>
              Primary Color
              <input name="primaryColor" defaultValue={theme.primaryColor} />
            </label>
            <label>
              Secondary Color
              <input name="secondaryColor" defaultValue={theme.secondaryColor} />
            </label>
            <label>
              Font
              <input name="font" defaultValue={theme.font} />
            </label>
            <label>
              Para birimi
              <input name="currency" defaultValue={theme.currency} />
            </label>
            <label>
              Dil
              <input name="language" defaultValue={theme.language} />
            </label>
            <button className="button" type="submit">
              Kaydet
            </button>
          </form>
        </div>

        <div className="card">
          <h3>Görsel Yönetimi</h3>
          <form className="form" onSubmit={handleAddAsset}>
            <label>
              Başlık
              <input name="assetTitle" placeholder="Yeni görsel" required />
            </label>
            <label>
              URL
              <input name="assetUrl" placeholder="https://..." required />
            </label>
            <label>
              Tür
              <select name="assetType" defaultValue="image">
                <option value="image">Ürün görseli</option>
                <option value="logo">Logo</option>
              </select>
            </label>
            <button className="button" type="submit">
              Ekle
            </button>
          </form>
          <div className="list">
            {assets.length === 0 ? (
              <p className="subtle">Henüz görsel eklenmedi.</p>
            ) : (
              assets.map((asset) => (
                <div className="list-item" key={asset.id}>
                  <div>
                    <strong>{asset.title}</strong>
                    <div className="subtle">{asset.type}</div>
                  </div>
                  <button
                    className="ghost"
                    type="button"
                    onClick={() => handleRemoveAsset(asset.id)}
                  >
                    Kaldır
                  </button>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="card">
          <h3>CDN Ayarları</h3>
          <form className="form" onSubmit={handleCdnSave}>
            <label>
              Provider
              <input name="provider" defaultValue={cdnSettings.provider} />
            </label>
            <label>
              Base URL
              <input name="baseUrl" defaultValue={cdnSettings.baseUrl} />
            </label>
            <label className="checkbox">
              <input name="enabled" type="checkbox" defaultChecked={cdnSettings.enabled} />
              CDN aktif
            </label>
            <label className="checkbox">
              <input
                name="autoOptimize"
                type="checkbox"
                defaultChecked={cdnSettings.autoOptimize}
              />
              Otomatik optimizasyon
            </label>
            <button className="button" type="submit">
              Güncelle
            </button>
          </form>
        </div>
      </section>
    </main>
  );
}
