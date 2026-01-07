![Red Strike Logo](https://via.placeholder.com/150)

# ATLAS

**Atlas**, Atlas gezegeninin mavi topraklarında geçen, Unity ile geliştirilmiş gerçek zamanlı çok oyunculu bir strateji oyunudur. İki oyuncu, savunma kuleleri inşa ederek ve modern savaş araçları konuşlandırarak stratejik bir çarpışmaya girer. Oyuncular, rakiplerinin üslerini ele geçirmek için hem savunma yapıları hem de çeşitli kara ve hava birimlerini taktiksel olarak kullanmalıdır. Hızlı düşünme, kaynak yönetimi ve ustaca kule yerleştirme becerileriniz zafer için kritiktir!

## Özellikler
- **Çok Oyunculu Strateji Savaşı:** İki oyuncunun gerçek zamanlı olarak karşı karşıya geldiği yoğun 1v1 maçlar.
- **Kule Savunma Sistemi:** Stratejik noktalara savunma kuleleri inşa ederek bölgenizi koruyun ve düşman saldırılarını püskürtün.
- **Çeşitli Savaş Araçları:** Tanklar, hava birimleri ve hafif araçlarla ordunuzu oluşturun ve saldırılarınızı planlayın.
- **Atlas Savaş Alanı:** Mavi gezegendeki kraterler, tepeler ve stratejik noktalarla dolu dinamik haritada çarpışın.
- **Kaynak ve Enerji Yönetimi:** Enerji kuleleri inşa ederek birimlerinizi güçlendirin ve fabrikalarınızla sürekli üretim yapın.
- **Stratejik Derinlik:** Her birimin kendine özgü güçlü ve zayıf yönlerini kullanarak rakibinizi alt edin.

## Ekran Görüntüleri
![Gameplay](https://via.placeholder.com/600x300?text=Gameplay+Screenshot)
![Mars Base](https://via.placeholder.com/600x300?text=Mars+Base+Screenshot)

## Kurulum
ATLAS ’ı yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin:

### Gereksinimler
- Unity 6 veya üstü
- Git/GitHub
- Photon Fusion (multiplayer için, opsiyonel)

### Adımlar
1. Bu depoyu klonlayın:
   ```bash
   git clone https://github.com/kullanici-adi/red-strike.git
   ```
2. Unity Hub’ı açın ve projeyi "Add" butonuyla ekleyin.
3. Unity Editor’de projeyi açın.
4. Gerekli paketleri (örneğin Photon Fusion) Unity Package Manager’dan indirin.
5. "Scenes" klasöründen ana sahneyi açıp "Play" tuşuna basın!

## Oynanış
**Amaç:** Stratejik kule savunması ve saldırı birimleriyle rakibinizin ana üssünü yok edin!

**Oyun Mekaniği:**
- **Kule İnşası:** Haritanın stratejik noktalarına savunma ve saldırı kuleleri yerleştirin.
- **Birlik Üretimi:** Fabrikalarınızı kullanarak tanklar, hava araçları ve hafif birimler üretin.
- **Enerji Yönetimi:** Enerji kuleleri inşa ederek araçlarınızın tam güçte çalışmasını sağlayın.
- **Taktiksel Saldırı:** Kara ve hava birimlerinizi koordine ederek düşman savunmasını yıkın.

**Kontroller:**
- **Sol Tık:** Birlik seçimi, kule yerleştirme ve hedef belirleme.
- **Sağ Tık:** Hareket komutu ve saldırı yönlendirme.
- **Q/E:** Kamera döndürme.
- **WASD:** Kamera hareketi.
- **1-5 Tuşları:** Hızlı birim/kule seçimi.

**Strateji İpuçları:**
- Önce enerji kulelerini inşa edin - birimleriniz daha verimli çalışır.
- Kuleleri yüksek noktalara yerleştirerek menzil avantajı kazanın.
- Rakibin zayıf noktalarını keşfedin ve saldırılarınızı oraya yönlendirin.
- Hem hava hem kara birimlerini dengeli kullanarak rakibinizi şaşırtın.
- Fabrikalarınızı koruyun - sürekli üretim zafer için kritiktir!

## Oyun İçi Ögeler

### Savunma ve Üretim Yapıları

---

**Merkez Üs** (Ana karargah. Yok edilmesi oyunun kaybedilmesi anlamına gelir.)
   - **Can Değeri:** 800 lv
   - **Menzil:** Yok
   - **Yoğunluk:** 1 (Tek başına bulunur)
   - **Tekrar Yaratma:** Hayır
   - **Özellik:** Oyunun temel yapısıdır. Korumak için stratejik kule yerleşimi şarttır.
  
**Fabrika** (Tüm savaş araçlarının üretildiği merkez.)
   - **Can Değeri:** 400 lv
   - **Menzil:** 10 birim
   - **Yoğunluk:** 2
   - **Tekrar Yaratma:** Evet (90 saniye)
   - **Üretim Kapasitesi:** Aynı anda en fazla 2 birim üretilebilir
   - **Özellik:** Yok edilirse yeni birim üretimi durur. Mutlaka koruyun!

**Enerji Kulesi** (Araçların maksimum performansla çalışması için gerekli enerji sağlar.)
   - **Can Değeri:** 300 lv
   - **Yoğunluk:** 3
   - **Tekrar Yaratma:** Evet (50 saniye)
   - **Enerji Aktarım Kapasitesi:** Aynı anda en fazla 2 araca enerji aktarımı yapılabilir
   - **Özel Yetenek:** Enerji yoksa araçlar %50 daha yavaş çalışır
   - **Strateji:** Erken dönemde inşa edilmeli. Daha fazla enerji = Daha güçlü ordu!

### Hava Birimleri

---

**Ornithopter A** (Hızlı ama düşük dayanıklılığa sahip.)  
- **Can Değeri:** 120 lv 
- **Hasar Verme Değeri:** 40 dv
- **Hız:** 300
- **Enerji:** 500 lt
- **Atış Tekrarı:** 1 saniye
- **Menzil:** 8 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (10 saniye)
- **Üretim Maliyeti:** Orta  
- **Özel Yetenek:** Hareket Halindeyken %20 daha az hasar alır.  

**Ornithopter B** (Daha dayanıklı ama daha yavaş.)  
- **Can Değeri:** 250 lv
- **Hasar Verme Değeri:** 80 dv 
- **Hız:** 100
- **Enerji:** 400 lt
- **Atış Tekrarı:** 3 saniye
- **Menzil:** 10 birim
- **Yoğunluk:** 4  
- **Tekrar Yaratma:** Evet (20 saniye)
- **Üretim Maliyeti:** Yüksek
- **Özel Yetenek:** Kalkan Aktif: İlk 2 saniye boyunca %50 daha az hasar alır.

### Kara Birimleri

---

**Tank Heavy A** (Oyundaki en güçlü tank.)  
- **Can Değeri:** 300 lv
- **Hasar Verme Değeri:** 220 dv
- **Hız:** 20
- **Enerji:** 500 lt
- **Atış Tekrarı:** 10 saniye
- **Menzil:** 15 birim
- **Yoğunluk:** 2
- **Tekrar Yaratma:** Evet (40 saniye) 
- **Üretim Maliyeti:** Çok Yüksek
- **Özel Yetenek:** Zırh Kırıcı: Düşman zırhını %20 oranında deler. 

**Tank Heavy B** (A versiyonuna göre daha hafif ama hâlâ güçlü.)  
- **Can Değeri:** 220 lv
- **Hasar Verme Değeri:** 160 dv 
- **Hız:** 40
- **Enerji:** 400 lt
- **Atış Tekrarı:** 7 saniye
- **Menzil:** 12 birim
- **Yoğunluk:** 3
- **Tekrar Yaratma:** Evet (30 saniye)
- **Üretim Maliyeti:** Yüksek
- **Özel Yetenek:** Düşük Yakıt Modu: %50 canı altına düştüğünde %30 daha hızlı hareket eder.

**Tank Combat** (Daha hızlı bir tank.)  
- **Can Değeri:** 180 lv
- **Hasar Verme Değeri:** 120 dv
- **Hız:** 120
- **Enerji:** 300 lt
- **Atış Tekrarı:** 4 saniye  
- **Menzil:** 10 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (15 saniye) 
- **Üretim Maliyeti:** Orta
- **Özel Yetenek:** Ani Saldırı: İlk atışında %25 ekstra hasar verir.

### Hafif ve Çevik Birimler

---

**Quat** (Çevik ve esnek birim.)  
- **Can Değeri:** 140 lv
- **Hasar Verme Değeri:** 60 dv
- **Hız:** 150
- **Enerji:** 500 lt
- **Atış Tekrarı:** 4 saniye
- **Menzil:** 6 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (10 saniye)
- **Üretim Maliyeti:** Düşük
- **Özel Yetenek:** Kaçınma: Hareket halindeyken %15 daha az hasar alır.

**Infantry Light** (En hızlı kara birimi.)  
- **Can Değeri:** 120 lv
- **Hasar Verme Değeri:** 60 dv
- **Hız:** 250
- **Enerji:** 400 lt
- **Atış Tekrarı:** 2 saniye
- **Menzil:** 4 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (5 saniye)  
- **Üretim Maliyeti:** Çok Düşük
- **Özel Yetenek:** Pusu: Düşmana saldırdığında ilk atışta %50 daha fazla hasar verir.

**Trike** (Dengeli bir kara aracı.)  
- **Can Değeri:** 150 lv
- **Hasar Verme Değeri:** 70 dv
- **Hız:** 120
- **Enerji:** 300 lt
- **Atış Tekrarı:** 4 saniye
- **Menzil:** 8 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (5 saniye)
- **Üretim Maliyeti:** Orta 
- **Özel Yetenek:** İkili Atış: %10 ihtimalle iki atış yapar.