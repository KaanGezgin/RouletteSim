# RouletteSim

Rulet numaraları ve renk kombinasyonları için bu web sitesini referans olarak kullanıyorum.
(https://stats.libretexts.org/Bookshelves/Probability_Theory/Probability_Mathematical_Statistics_and_Stochastic_Processes_(Siegrist)/13%3A_Games_of_Chance/13.05%3A_Roulette)

## 🎮 Oynanış ve Kontroller

Oyuncu, 3D bir ortamda serbestçe dolaşabilir ve rulet masasıyla etkileşime girerek bahis oynayabilir.

* **Hareket:** `W, A, S, D`
* **Kamera:** `Mouse`
* **Etkileşim:** Masadaki "Interaction Zone" içine girildiğinde `E` tuşu ile Bahis Paneli açılır.
* **UI Kontrolü:** Panel açıkken Mouse imleci serbest kalır.

### Nasıl Oynanır?
1.  Masaya yaklaşın ve `E` tuşuna basarak arayüzü açın.
2.  **Bet Type** seçin (Sayı, Renk veya Tek/Çift).
3.  Miktarı girin ve **"Place Bet"** butonuna tıklayın.
4.  (Opsiyonel) **Deterministic Input** alanına 0-36 arası bir sayı girip "Set" diyerek bir sonraki sonucu manipüle edin.
5.  **SPIN** butonuna basın ve sonucu izleyin.

---

## 🏗 Mimari ve Tasarım Desenleri (Design Patterns)

Proje, **SOLID** prensiplerine sadık kalınarak, modüler ve genişletilebilir bir yapıda geliştirilmiştir. Kullanılan temel desenler şunlardır:

### 1. Singleton Pattern
Oyunun genel durumunu yöneten ve sahneler arası veri taşıyan sınıflar için kullanılmıştır.
* `GameManager`: Oyun döngüsünü (State Machine) yönetir.
* `BetManager`: Cüzdan ve aktif bahisleri yönetir.
* `StatisticsManager`: Oyuncu istatistiklerini tutar.

### 2. Observer Pattern (Event-Driven)
Sınıflar arasındaki bağımlılığı (Coupling) minimuma indirmek için **C# Events** kullanılmıştır.
* *Örnek:* `GameManager` sonucu belirlediğinde `OnSpinResultDetermined` event'ini fırlatır. `RouletteWheelController` (Animasyon) ve `UIManager` (Arayüz) bu event'i dinler. Böylece GameManager'ın görsel sınıflardan haberdar olması gerekmez.

### 3. State Pattern (State Machine)
Oyunun akışı `GameState` enum yapısı ile yönetilir:
* `Betting` -> `Spinning` -> `Result` -> `Payout`
Her aşama, `GameManager` içinde izole edilmiştir.

### 4. Strategy / Polymorphism (Betting System)
Bahis türleri için `if-else` karmaşası yerine soyutlama kullanılmıştır.
* `BetBase` (Abstract Class): Tüm bahislerin atasıdır.
* `StraightBet`, `ColorBet`, `ParityBet`: Kendi `IsWin()` ve `CalculatePayout()` mantıklarını uygular. Yeni bir bahis türü eklemek için mevcut kodu değiştirmek gerekmez (Open/Closed Principle).

---

## 🔧 Teknik Detaylar

* **Motor:** Unity 6000.0.X
* **Dil:** C#
* **Varlık Yönetimi:** ScriptableObjects (`RouletteDataSO`) kullanılarak çark üzerindeki sayı dizilimi ve renk verileri koddan ayrıştırılmıştır.
* **Animasyon:** Harici kütüphane (DoTween vb.) **kullanılmamıştır**. Custom `Tweener` sınıfı ve `AnimationCurve` kullanılarak matematiksel animasyonlar kodlanmıştır.
* **Persistence:** `PlayerPrefs` kullanılarak toplam kazanç, oynanan maç sayısı ve geçmiş sonuçlar (History) cihazda saklanır.

---

## 🚀 Kurulum

1.  Repo'yu klonlayın: `git clone https://github.com/KaanGezgin/RouletteSim.git`
2.  Unity Hub üzerinden projeyi açın (Unity 6 versiyonu önerilir).
3.  `Scenes/RouletteGame` sahnesini açın ve Play butonuna basın.

---
