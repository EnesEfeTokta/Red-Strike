![Red Strike Logo](https://via.placeholder.com/150)

https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white
https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white
https://img.shields.io/badge/microsoft%20azure-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white
https://img.shields.io/badge/Visual_Studio_Code-0078D4?style=for-the-badge&logo=visual%20studio%20code&logoColor=white

# Red Strike

**Red Strike**, Mars’ın kırmızı topraklarında geçen, gerçek zamanlı bir strateji oyunudur. İki oyuncu, modern tanklar, uçaklar ve teçhizatlarla donanmış ordularını kontrol ederek gezegenin hakimiyeti için çarpışır. Unity ile geliştirilen bu oyun, hızlı karar verme ve taktiksel zekayı ödüllendirir. Kızıl gezegendeki savaş seni bekliyor!

## Özellikler
- **Gerçek Zamanlı Strateji:** Hızlı tempolu maçlar, anlık kararlarla dolu.
- **Modern Savaş Teknolojisi:** Tanklar, insansız hava araçları ve yüksek teknolojili üniteler.
- **Mars Ortamı:** Kızıl çöller, kraterler ve üslerle dolu dinamik bir savaş alanı.
- **Çok Oyunculu Deneyim:** 1v1 online maçlarla arkadaşlarına veya rakiplerine meydan oku.
- **Stratejik Derinlik:** Kaynak yönetimi, üs inşası ve birlik konuşlandırma.

## Ekran Görüntüleri
![Gameplay](https://via.placeholder.com/600x300?text=Gameplay+Screenshot)
![Mars Base](https://via.placeholder.com/600x300?text=Mars+Base+Screenshot)

## Kurulum
Red Strike’ı yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin:

### Gereksinimler
- Unity 2022.3 veya üstü
- Git/GitHub
- Photon PUN 2 (multiplayer için, opsiyonel)
- Azure PlayFab

### Adımlar
1. Bu depoyu klonlayın:
   ```bash
   git clone https://github.com/kullanici-adi/red-strike.git
   ```
2. Unity Hub’ı açın ve projeyi "Add" butonuyla ekleyin.
3. Unity Editor’de projeyi açın.
4. Gerekli paketleri (örneğin Photon PUN 2) Unity Package Manager’dan indirin.
5. "Scenes" klasöründen ana sahneyi açıp "Play" tuşuna basın!

## Oynanış
**Amaç:** Rakibin ana üssünü yok et veya kaynaklarını tüketerek üstünlük sağla.

**Kontroller:**
*Sol Tık:* Birlik seçimi ve komut verme.

*Sağ Tık:* Hareket ve saldırı yönlendirme.

*Q/E:* Kamera döndürme.

**Strateji İpuçları:**
Erken oyunda kaynak toplamaya odaklan.
Uçaklarla rakibin savunmasını aşmayı dene.

## Oyun İçi Ögeler

### Yapılar

---

**Merkez Yapı** (Ana üssü temsil eder. Kaybedilirse oyun biter.)
   - **Can Değeri:** 800 lv
   - **Korunma Değeri:** 500 hv
   - **Hasar Verme Değeri:** 0 dv
   - **Menzil:** Yok
   - **Yoğunluk:** 1 (Tek başına bulunur)
   - **Tekrar Yaratma:** Hayır
  
**Fabrika** (Araç üretimi sağlar.)
   - **Can Değeri:** 400 lv
   - **Korunma Değeri:** 300 hv
   - **Hasar Verme Değeri:** 15 dv
   - **Menzil:** 10 birim
   - **Yoğunluk:** 1 (Her oyuncunun bir tane olabilir.)
   - **Tekrar Yaratma:** Evet (90 saniye)
   - **Üretim Kapasitesi:** Aynı anda en fazla 2 birim üretilebilir.

**Enerji Kulesi** (Araçların çalışması için enerji üretir.)
   - **Can Değeri:** 300 lv
   - **Korunma Değeri:** 150 hv
   - **Hasar Verme Değeri:** 0 dv
   - **Menzil:** Yok
   - **Yoğunluk:** 3
   - **Tekrar Yaratma:** Evet (50 saniye)
   - **Enerji Aktarım Kapasitesi:** Aynı anda en fazla 2 araca enerji aktarımı yapılabilir.
   - **Özel Yetenek:** Enerji yoksa araçlar %50 daha yavaş çalışır.

### Hava Birimleri

---

**Ornithopter A** (Hızlı ama düşük dayanıklılığa sahip.)  
- **Can Değeri:** 120 lv 
- **Korunma Değeri:** 50 hv 
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
- **Korunma Değeri:** 150 hv
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
- **Korunma Değeri:** 300 hv
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
- **Korunma Değeri:** 200 hv 
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
- **Korunma Değeri:** 150 hv
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
- **Korunma Değeri:** 100 hv
- **Hasar Verme Değeri:** 60 dv
- **Hız:** 150
- **Enerji:** 500 lt
- **Atış Tekrarı:** 4 saniye
- **Menzil:** 6 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (10 saniye)
- **Üretim Maliyeti:** Düşük
- **Özel Yetenek:** Kaçınma: Hareket halindeyken %15 daha az hasar alır.

**🔟 Infantry Light** (En hızlı kara birimi.)  
- **Can Değeri:** 120 lv
- **Korunma Değeri:** 50 hv
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
- **Korunma Değeri:** 100 hv
- **Hasar Verme Değeri:** 70 dv
- **Hız:** 120
- **Enerji:** 300 lt
- **Atış Tekrarı:** 4 saniye
- **Menzil:** 8 birim
- **Yoğunluk:** 5
- **Tekrar Yaratma:** Evet (5 saniye)
- **Üretim Maliyeti:** Orta 
- **Özel Yetenek:** İkili Atış: %10 ihtimalle iki atış yapar.