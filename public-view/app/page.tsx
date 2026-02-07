const items = [
  {
    name: "Badem Sütlü Latte",
    description: "Şekersiz, badem sütü ile.",
    price: "₺125",
    image: "https://images.unsplash.com/photo-1481391032119-d89fee407e44?auto=format&fit=crop&w=800&q=80"
  },
  {
    name: "Avokadolu Bowl",
    description: "Kinoa, avokado, nar taneleri.",
    price: "₺165",
    image: "https://images.unsplash.com/photo-1490645935967-10de6ba17061?auto=format&fit=crop&w=800&q=80"
  },
  {
    name: "Matcha Pancake",
    description: "Büyük porsiyon, taze meyve.",
    price: "₺150",
    image: "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0?auto=format&fit=crop&w=800&q=80"
  }
];

export default function PublicMenu() {
  return (
    <main>
      <section className="hero">
        <h1>Cafe Luna Dijital Menü</h1>
        <p>Mobil-first, hızlı ve sadece görüntüleme modunda.</p>
        <div className="search">
          <input type="text" placeholder="Ürün ara" />
          <button className="chip" type="button">Filtrele</button>
        </div>
      </section>

      <div className="chips">
        {"Kahvaltı,Salata,Tatlılar,İçecekler".split(",").map((label) => (
          <span className="chip" key={label}>{label}</span>
        ))}
      </div>

      <section className="grid">
        {items.map((item) => (
          <article className="card" key={item.name}>
            <img src={item.image} alt={item.name} />
            <h3>{item.name}</h3>
            <p>{item.description}</p>
            <div className="price">{item.price}</div>
          </article>
        ))}
      </section>

      <section className="widgets">
        <div className="widget">
          <strong>Garson çağır</strong>
          <p className="subtle">Masanız için servis talep edin.</p>
        </div>
        <div className="widget">
          <strong>Wi-Fi şifresi</strong>
          <p className="subtle">CafeLuna-2024</p>
        </div>
      </section>
    </main>
  );
}
