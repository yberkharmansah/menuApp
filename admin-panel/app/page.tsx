const categories = [
  { name: "Kahvaltı", status: "Yayında" },
  { name: "Tatlılar", status: "Gizli" },
  { name: "İçecekler", status: "Yayında" }
];

const products = [
  { name: "Avokadolu tost", price: "₺140", tag: "vegan" },
  { name: "Çikolatalı kek", price: "₺95", tag: "alerjen: süt" },
  { name: "Matcha latte", price: "₺110", tag: "buzlu" }
];

export default function AdminDashboard() {
  return (
    <main>
      <section className="grid">
        <div className="card">
          <h3>Menü durumu</h3>
          <p className="subtle">1 aktif menü, 12 kategori, 68 ürün</p>
          <div className="actions" style={{ marginTop: 16 }}>
            <a className="button" href="#">Menüyü düzenle</a>
            <a className="button outline" href="#">Yayınla</a>
          </div>
        </div>
        <div className="card">
          <h3>Tema ayarları</h3>
          <p className="subtle">Logo, renk ve font ayarlarını buradan yönet.</p>
          <div className="actions" style={{ marginTop: 16 }}>
            <a className="button" href="#">Temayı özelleştir</a>
          </div>
        </div>
        <div className="card">
          <h3>Görsel yönetimi</h3>
          <p className="subtle">Yeni görselleri yükle, thumbnail üret.</p>
          <div className="actions" style={{ marginTop: 16 }}>
            <a className="button" href="#">Görseller</a>
            <a className="button outline" href="#">CDN ayarları</a>
          </div>
        </div>
      </section>

      <section className="section">
        <h2>Kategoriler</h2>
        <div className="list">
          {categories.map((category) => (
            <div className="list-item" key={category.name}>
              <div>
                <strong>{category.name}</strong>
                <div className="subtle">Sıra: 1</div>
              </div>
              <span className="badge">{category.status}</span>
            </div>
          ))}
        </div>
      </section>

      <section className="section">
        <h2>Öne çıkan ürünler</h2>
        <div className="list">
          {products.map((product) => (
            <div className="list-item" key={product.name}>
              <div>
                <strong>{product.name}</strong>
                <div className="subtle">{product.tag}</div>
              </div>
              <span className="badge">{product.price}</span>
            </div>
          ))}
        </div>
      </section>
    </main>
  );
}
