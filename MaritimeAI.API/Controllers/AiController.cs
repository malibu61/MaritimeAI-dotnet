using Humanizer;
using MaritimeAI.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MaritimeAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IShipsService _shipsService;
        private readonly IDistributedCache _cache;

        public AiController(IShipsService shipsService, IDistributedCache cache)
        {
            _shipsService = shipsService;
            _cache = cache;
        }

        [HttpGet("CanakkaleStraitAnalyze")]
        public async Task<IActionResult> CanakkaleStraitAnalyze()
        {
            try
            {
                string cacheKey = "canakkale_strait_analysis";

                // Cache kontrolü
                try
                {
                    var cachedData = await _cache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        return Ok(new { analysis = cachedData, success = true });
                    }
                }
                catch (Exception)
                {
                }

                var southOfCanakkaleStrTotalShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);
                var northOfCanakkaleStrTotalShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);
                var totalShipsCount = southOfCanakkaleStrTotalShipsCount + northOfCanakkaleStrTotalShipsCount;

                var southOfCanakkaleStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);
                var northOfCanakkaleStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);
                var totalTransitShipsCount = southOfCanakkaleStrTransitShipsCount + northOfCanakkaleStrTransitShipsCount;

                var southOfCanakkaleStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);
                var northOfCanakkaleStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);
                var totalTankersCount = southOfCanakkaleStrTankersCount + northOfCanakkaleStrTankersCount;

                var southOfCanakkaleStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);
                var northOfCanakkaleStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);
                var avgSpeed = (southOfCanakkaleStrAvgSpeed + northOfCanakkaleStrAvgSpeed) / 3;

                string trafficUrl = "https://www.kiyiemniyeti.gov.tr/bogaz_trafigi";
                string trafficResponse = await _httpClient.GetStringAsync(trafficUrl);

                var currentDay = DateTime.Now.ToString("dddd", new CultureInfo("tr-TR"));
                var currentHour = DateTime.Now.Hour;
                var currentMinute = DateTime.Now.Minute;
                var currentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));

                var apiKey = "apikey";

                var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

                var prompt = $@"Sen bir deniz trafiği uzmanısın ve Çanakkale Boğazı'ndaki gemi trafiğini analiz ediyorsun. 
                            Yaptığın bu analiz, boğaz trafiğini yöneten personellere doğrudan yardımcı olacak.
                            Profesyonel cevap ver, bunu son kullanıcı görecek!!!

                            GÜNCEL VERİLER:
                            Tarih: {currentDate}
                            Saat: {currentHour}:{currentMinute:D2}
                            Gün: {currentDay}

                            GEMİ SAYILARI:
                            - Toplam gemi: {totalShipsCount}
                            - Transit gemi (Tanker + Kargo): {totalTransitShipsCount}
                              • Güney bölge: {southOfCanakkaleStrTransitShipsCount}
                              • Kuzey bölge: {northOfCanakkaleStrTransitShipsCount}
                            - Tanker sayısı: {totalTankersCount}
                              • Güney bölge: {southOfCanakkaleStrTankersCount}
                              • Kuzey bölge: {northOfCanakkaleStrTankersCount}

                            HIZ VERİLERİ:
                            - Genel ortalama hız: {avgSpeed:F1} knot
                            - Güney bölge ortalama: {southOfCanakkaleStrAvgSpeed:F1} knot
                            - Kuzey bölge ortalama: {northOfCanakkaleStrAvgSpeed:F1} knot

                            TRAFİK DURUMU (Kıyı Emniyeti Resmi Veri), Çanakkale Boğazı için bunu incele ve, gemilerin trafiğin kapalı olup olmadığını 
                            hafızana al: {trafficResponse}

                            Çanakkale BOĞAZI TRAFİK SAATLERİ:
                            Kuzey→Güney:
                            Trafik Durumu için Kıyı Emniyetinin sitesinden gelen dinamik veriyi incele.

                            Güney→Kuzey:
                            Trafik Durumu için Kıyı Emniyetinin sitesinden gelen dinamik veriyi incele.

                            NORMAL YOĞUNLUK SÖLASİ (Trafik Açıkken):
                            - 15-25 gemi → 🟢 Düşük yoğunluk (İdeal)
                            - 25-35 gemi → 🟡 Orta yoğunluk (Normal)
                            - 35-45 gemi → 🟠 Yüksek yoğunluk (Dikkat)
                            - 45+ gemi → 🔴 Çok yüksek yoğunluk (Kritik)

                            SAATE GÖRE BEKLENTİLER:
                            - 00:40-06:00: Sakin dönem (15-20 gemi)
                            - 06:00-09:00: Artış dönemi (20-30 gemi)
                            - 09:00-18:00: Yoğun dönem (30-40 gemi)
                            - 18:00-22:00: Azalma dönemi (25-35 gemi)
                            - 22:00-00:00: Gece trafiği (20-25 gemi)

                            ANALİZ GÖREVİN:
                            1. TRAFİK DURUMU: Kıyı Emniyetinin sitesinden gelen dinamik veri - Bu durum gemi sayılarını nasıl etkiliyor?
                            2. BÖLGESEL ANALİZ: Hangi bölgede (Güney/Kuzey) daha fazla yoğunluk var? Darboğaz var mı?
                            3. YOĞUNLUK DEĞERLENDİRMESİ: Toplam {totalTransitShipsCount} transit gemi yoğunluk skalasına göre nasıl?
                            4. SAATSEL KARŞILAŞTIRMA: Saat {currentHour}:{currentMinute:D2} için bu sayı normal mi?
                            5. TEHLİKELİ YÜK RİSKİ: {totalTankersCount} tanker hangi bölgede yoğunlaşmış? Risk seviyesi?
                            6. HIZ ANALİZİ: Ortalama {avgSpeed:F1} knot normal mi? (İdeal: 8-14 knot)
                            7. PERSONEL ÖNERİLERİ: Trafik yönetim personeline 3-4 spesifik öneri ver.

                            DETAYLI CEVAP FORMATI:
                            ## 1. GENEL DURUM
                            [Emoji + 1-2 cümle özet]

                            ## 2. TRAFİK AKIŞ DURUMU
                            [Açık/Askıda olan yönler + etkisi]

                            ## 3. BÖLGESEL ANALİZ
                            - Güney: [Durum + gemi sayısı]
                            - Orta: [Durum + gemi sayısı]
                            - Kuzey: [Durum + gemi sayısı]
                            - Kritik nokta: [Varsa belirt]

                            ## 4. YOĞUNLUK DEĞERLENDİRMESİ
                            [Yoğunluk seviyesi + saate göre karşılaştırma]

                            ## 5. TEHLİKELİ YÜK RİSKİ
                            [Tanker dağılımı + risk değerlendirmesi]

                            ## 6. HIZ ANALİZİ
                            [Bölgesel hız karşılaştırması]

                            ## 7. PERSONEL İÇİN ÖNERİLER
                            1. [Spesifik öneri 1]
                            2. [Spesifik öneri 2]
                            3. [Spesifik öneri 3]
                            4. [Spesifik öneri 4]

                            NOT: Profesyonel, detaylı ve uygulanabilir bir analiz yap. Max 300 kelime.";

                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                request.Headers.Add("X-goog-api-key", apiKey);

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "AI analizi başarısız",
                        statusCode = (int)response.StatusCode,
                        details = responseString
                    });
                }

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string analysis = result.candidates[0].content.parts[0].text;

                var responseData = new { analysis = analysis, success = true };

                // Cache'e kaydet
                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    };
                    await _cache.SetStringAsync(cacheKey, analysis, cacheOptions);
                }
                catch (Exception)
                {
                }

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



        [HttpGet("IstanbulStraitAnalyze")]
        public async Task<IActionResult> IstanbulStraitAnalyze()
        {
            try
            {

                string cacheKey = "istanbul_strait_analysis";

                // Cache kontrolü
                try
                {
                    var cachedData = await _cache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        return Ok(new { analysis = cachedData, success = true });
                    }
                }
                catch (Exception)
                {
                }

                var southOfIstanbulStrTotalShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);
                var middleOfIstanbulStrTotalShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);
                var northOfIstanbulStrTotalShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);
                var totalShipsCount = southOfIstanbulStrTotalShipsCount + middleOfIstanbulStrTotalShipsCount + northOfIstanbulStrTotalShipsCount;

                var southOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);
                var middleOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);
                var northOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);
                var totalTransitShipsCount = southOfIstanbulStrTransitShipsCount + middleOfIstanbulStrTransitShipsCount + northOfIstanbulStrTransitShipsCount;

                var southOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);
                var middleOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);
                var northOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);
                var totalTankersCount = southOfIstanbulStrTankersCount + middleOfIstanbulStrTankersCount + northOfIstanbulStrTankersCount;

                var southOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);
                var middleOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);
                var northOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);
                var avgSpeed = (southOfIstanbulStrAvgSpeed + middleOfIstanbulStrAvgSpeed + northOfIstanbulStrAvgSpeed) / 3;


                string trafficUrl = "https://www.kiyiemniyeti.gov.tr/bogaz_trafigi";
                string trafficResponse = await _httpClient.GetStringAsync(trafficUrl);


                var currentDay = DateTime.Now.ToString("dddd", new CultureInfo("tr-TR"));
                var currentHour = DateTime.Now.Hour;
                var currentMinute = DateTime.Now.Minute;
                var currentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));


                var apiKey = "apikey";
                var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

                var prompt = $@"Sen bir deniz trafiği uzmanısın ve İstanbul Boğazı'ndaki gemi trafiğini analiz ediyorsun. 
                            Yaptığın bu analiz, boğaz trafiğini yöneten personellere doğrudan yardımcı olacak.
                            Profesyonel cevap ver, bunu son kullanıcı görecek!!!

                            GÜNCEL VERİLER:
                            Tarih: {currentDate}
                            Saat: {currentHour}:{currentMinute:D2}
                            Gün: {currentDay}

                            GEMİ SAYILARI:
                            - Toplam gemi: {totalShipsCount}
                            - Transit gemi (Tanker + Kargo): {totalTransitShipsCount}
                              • Güney bölge: {southOfIstanbulStrTransitShipsCount}
                              • Orta bölge: {middleOfIstanbulStrTransitShipsCount}
                              • Kuzey bölge: {northOfIstanbulStrTransitShipsCount}
                            - Tanker sayısı: {totalTankersCount}
                              • Güney bölge: {southOfIstanbulStrTankersCount}
                              • Orta bölge: {middleOfIstanbulStrTankersCount}
                              • Kuzey bölge: {northOfIstanbulStrTankersCount}

                            HIZ VERİLERİ:
                            - Genel ortalama hız: {avgSpeed:F1} knot
                            - Güney bölge ortalama: {southOfIstanbulStrAvgSpeed:F1} knot
                            - Orta bölge ortalama: {middleOfIstanbulStrAvgSpeed:F1} knot
                            - Kuzey bölge ortalama: {northOfIstanbulStrAvgSpeed:F1} knot

                            TRAFİK DURUMU (Kıyı Emniyeti Resmi Veri), İstanbul Boğazı için bunu incele ve, gemilerin trafiğin kapalı olup olmadığını 
                            hafızana al: {trafficResponse}

                            İSTANBUL BOĞAZI TRAFİK SAATLERİ:
                            Kuzey→Güney:
                            Trafik Durumu için Kıyı Emniyetinin sitesinden gelen dinamik veriyi incele.

                            Güney→Kuzey:
                            Trafik Durumu için Kıyı Emniyetinin sitesinden gelen dinamik veriyi incele.

                            NORMAL YOĞUNLUK SÖLASİ (Trafik Açıkken):
                            - 15-25 gemi → 🟢 Düşük yoğunluk (İdeal)
                            - 25-35 gemi → 🟡 Orta yoğunluk (Normal)
                            - 35-45 gemi → 🟠 Yüksek yoğunluk (Dikkat)
                            - 45+ gemi → 🔴 Çok yüksek yoğunluk (Kritik)

                            SAATE GÖRE BEKLENTİLER:
                            - 00:40-06:00: Sakin dönem (15-20 gemi)
                            - 06:00-09:00: Artış dönemi (20-30 gemi)
                            - 09:00-18:00: Yoğun dönem (30-40 gemi)
                            - 18:00-22:00: Azalma dönemi (25-35 gemi)
                            - 22:00-00:00: Gece trafiği (20-25 gemi)

                            ANALİZ GÖREVİN:
                            1. TRAFİK DURUMU: Kıyı Emniyetinin sitesinden gelen dinamik veri - Bu durum gemi sayılarını nasıl etkiliyor?
                            2. BÖLGESEL ANALİZ: Hangi bölgede (Güney/Orta/Kuzey) daha fazla yoğunluk var? Darboğaz var mı?
                            3. YOĞUNLUK DEĞERLENDİRMESİ: Toplam {totalTransitShipsCount} transit gemi yoğunluk skalasına göre nasıl?
                            4. SAATSEL KARŞILAŞTIRMA: Saat {currentHour}:{currentMinute:D2} için bu sayı normal mi?
                            5. TEHLİKELİ YÜK RİSKİ: {totalTankersCount} tanker hangi bölgede yoğunlaşmış? Risk seviyesi?
                            6. HIZ ANALİZİ: Ortalama {avgSpeed:F1} knot normal mi? (İdeal: 8-14 knot)
                            7. PERSONEL ÖNERİLERİ: Trafik yönetim personeline 3-4 spesifik öneri ver.

                            DETAYLI CEVAP FORMATI:
                            ## 1. GENEL DURUM
                            [Emoji + 1-2 cümle özet]

                            ## 2. TRAFİK AKIŞ DURUMU
                            [Açık/Askıda olan yönler + etkisi]

                            ## 3. BÖLGESEL ANALİZ
                            - Güney: [Durum + gemi sayısı]
                            - Orta: [Durum + gemi sayısı]
                            - Kuzey: [Durum + gemi sayısı]
                            - Kritik nokta: [Varsa belirt]

                            ## 4. YOĞUNLUK DEĞERLENDİRMESİ
                            [Yoğunluk seviyesi + saate göre karşılaştırma]

                            ## 5. TEHLİKELİ YÜK RİSKİ
                            [Tanker dağılımı + risk değerlendirmesi]

                            ## 6. HIZ ANALİZİ
                            [Bölgesel hız karşılaştırması]

                            ## 7. PERSONEL İÇİN ÖNERİLER
                            1. [Spesifik öneri 1]
                            2. [Spesifik öneri 2]
                            3. [Spesifik öneri 3]
                            4. [Spesifik öneri 4]

                            NOT: Profesyonel, detaylı ve uygulanabilir bir analiz yap. Max 300 kelime.";

                var body = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
                };

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                request.Headers.Add("X-goog-api-key", apiKey);

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "AI analizi başarısız",
                        statusCode = (int)response.StatusCode,
                        details = responseString
                    });
                }

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string analysis = result.candidates[0].content.parts[0].text;


                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    };
                    await _cache.SetStringAsync(cacheKey, analysis, cacheOptions);
                }
                catch (Exception)
                {
                }

                // Kapsamlı çıktı
                return Ok(new
                {
                    success = true,
                    timestamp = DateTime.Now,
                    analysis = analysis

                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("CanakkaleStraitNavtexAnalyze")]
        public async Task<IActionResult> CanakkaleStraitNavtexAnalyze()
        {
            try
            {
                string cacheKey = "canakkale_strait_navtex_analysis";

                // Cache kontrolü
                try
                {
                    var cachedData = await _cache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        return Ok(new { analysis = cachedData, success = true });
                    }
                }
                catch (Exception)
                {
                }

                string trafficUrl = "https://www.kiyiemniyeti.gov.tr/bogaz_trafigi";
                string trafficResponse = await _httpClient.GetStringAsync(trafficUrl);

                string navtexUrl1 = "https://www.kiyiemniyeti.gov.tr/turkish_radio_navtex_broadcasts?page=1";
                string navtexResponse1 = await _httpClient.GetStringAsync(navtexUrl1);

                string navtexUrl2 = "https://www.kiyiemniyeti.gov.tr/turkish_radio_navtex_broadcasts?page=2";
                string navtexResponse2 = await _httpClient.GetStringAsync(navtexUrl2);

                var currentHour = DateTime.Now.Hour;
                var currentMinute = DateTime.Now.Minute;
                var currentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));

                var apiKey = "apikey";

                var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

                var prompt = $@"Sen bir deniz trafiği uzmanısın ve Çanakkale Boğazı'ndaki NAVTEX ilanlarını analiz ediyorsun. 
                                NAVTEX ilanları, gemiciler için kritik güvenlik ve trafik bilgilerini içerir.

                                GÜNCEL BİLGİLER:
                                Tarih: {currentDate}
                                Saat: {currentHour}:{currentMinute:D2}

                                NAVTEX İLANLARI (Kıyı Emniyeti Resmi Verisi):
                                {navtexResponse1}
                                {navtexResponse2}

                                TRAFİK DURUMU (Referans):
                                {trafficResponse}

                                ANALİZ GÖREVİN:
                                1. **NAVTEX İlanlarını Öncelikle İncele:**
                                   - Hangi NAVTEX ilanları Çanakkale Boğazı'nı etkiliyor?
                                   - İlan türleri: Trafik kısıtlamaları, tatbikatlar, batık/engel, bakım çalışmaları, hava/deniz durumu uyarıları
                                   - Her ilanın geçerlilik tarihi ve saati nedir?
                                   - Hangi bölgeleri (koordinatları) kapsıyor?

                                2. **Trafiğe Etkiyi Değerlendir:**
                                   - NAVTEX ilanları trafiği nasıl etkiliyor? (Tam kapanış, kısmi kısıtlama, bekleme, vb.)
                                   - Şu anki saat için hangi ilanlar aktif?
                                   - Yakın gelecekte (bugün/yarın) aktif olacak ilanlar var mı?

                                3. **Gemi Kaptanlarına Öneriler:**
                                   - Transit geçiş için uygun zaman dilimleri
                                   - Kaçınılması gereken bölgeler/saatler
                                   - Dikkat edilmesi gereken hususlar

                                4. **Özet Durum:**
                                   - 🟢 Normal / 🟡 Dikkat / 🟠 Kısıtlı / 🔴 Kapalı
                                   - Tek cümleyle mevcut durum

                                CEVAP FORMATI:
                                ## GENEL DURUM
                                [emoji + Özet durum]

                                ## AKTİF NAVTEX İLANLARI
                                [Her ilan için: İlan No, Tür, Geçerlilik, Etkilenen Bölge, Özet]

                                ## TRAFİK ETKİSİ ANALİZİ
                                [NAVTEX ilanlarının trafiğe etkisi, detaylı açıklama]

                                ## PERSONEL ÖNERİLERİ
                                [Transit için öneriler, dikkat noktaları]

                                NOT: 
                                - NAVTEX ilanlarına odaklan, trafik durumunu sadece referans olarak kullan
                                - Türkçe, profesyonel ama anlaşılır dil
                                - Konkret bilgi ver, genel laflar etme
                                - İlan numaralarını ve tarihlerini belirt
                                - Max 400 kelime";

                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                request.Headers.Add("X-goog-api-key", apiKey);

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "AI analizi başarısız",
                        statusCode = (int)response.StatusCode,
                        details = responseString
                    });
                }

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string analysis = result.candidates[0].content.parts[0].text;


                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    };
                    await _cache.SetStringAsync(cacheKey, analysis, cacheOptions);
                }
                catch (Exception)
                {
                }

                return Ok(new { analysis = analysis, success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("IstanbulStraitNavtexAnalyze")]
        public async Task<IActionResult> IstanbulStraitNavtexAnalyze()
        {
            try
            {
                string cacheKey = "istanbul_strait_navtex_analysis";

                // Cache kontrolü
                try
                {
                    var cachedData = await _cache.GetStringAsync(cacheKey);
                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        return Ok(new { analysis = cachedData, success = true });
                    }
                }
                catch (Exception)
                {
                }

                string trafficUrl = "https://www.kiyiemniyeti.gov.tr/bogaz_trafigi";
                string trafficResponse = await _httpClient.GetStringAsync(trafficUrl);

                string navtexUrl1 = "https://www.kiyiemniyeti.gov.tr/turkish_radio_navtex_broadcasts?page=1";
                string navtexResponse1 = await _httpClient.GetStringAsync(navtexUrl1);

                string navtexUrl2 = "https://www.kiyiemniyeti.gov.tr/turkish_radio_navtex_broadcasts?page=2";
                string navtexResponse2 = await _httpClient.GetStringAsync(navtexUrl2);

                var currentHour = DateTime.Now.Hour;
                var currentMinute = DateTime.Now.Minute;
                var currentDate = DateTime.Now.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));

                var apiKey = "apikey";

                var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

                var prompt = $@"Sen bir deniz trafiği uzmanısın ve İstanbul Boğazı'ndaki NAVTEX ilanlarını analiz ediyorsun. 
                                NAVTEX ilanları, gemiciler için kritik güvenlik ve trafik bilgilerini içerir.

                                GÜNCEL BİLGİLER:
                                Tarih: {currentDate}
                                Saat: {currentHour}:{currentMinute:D2}

                                NAVTEX İLANLARI (Kıyı Emniyeti Resmi Verisi):
                                {navtexResponse1}
                                {navtexResponse2}

                                TRAFİK DURUMU (Referans):
                                {trafficResponse}

                                ANALİZ GÖREVİN:
                                1. **NAVTEX İlanlarını Öncelikle İncele:**
                                   - Hangi NAVTEX ilanları İstanbul Boğazı'nı etkiliyor?
                                   - İlan türleri: Trafik kısıtlamaları, tatbikatlar, batık/engel, bakım çalışmaları, hava/deniz durumu uyarıları
                                   - Her ilanın geçerlilik tarihi ve saati nedir?
                                   - Hangi bölgeleri (koordinatları) kapsıyor?

                                2. **Trafiğe Etkiyi Değerlendir:**
                                   - NAVTEX ilanları trafiği nasıl etkiliyor? (Tam kapanış, kısmi kısıtlama, bekleme, vb.)
                                   - Şu anki saat için hangi ilanlar aktif?
                                   - Yakın gelecekte (bugün/yarın) aktif olacak ilanlar var mı?

                                3. **Gemi Kaptanlarına Öneriler:**
                                   - Transit geçiş için uygun zaman dilimleri
                                   - Kaçınılması gereken bölgeler/saatler
                                   - Dikkat edilmesi gereken hususlar

                                4. **Özet Durum:**
                                   - 🟢 Normal / 🟡 Dikkat / 🟠 Kısıtlı / 🔴 Kapalı
                                   - Tek cümleyle mevcut durum

                                CEVAP FORMATI:
                                ## GENEL DURUM
                                [emoji + Özet durum]

                                ## AKTİF NAVTEX İLANLARI
                                [Her ilan için: İlan No, Tür, Geçerlilik, Etkilenen Bölge, Özet]

                                ## TRAFİK ETKİSİ ANALİZİ
                                [NAVTEX ilanlarının trafiğe etkisi, detaylı açıklama]

                                ## PERSONEL ÖNERİLERİ
                                [Transit için öneriler, dikkat noktaları]

                                NOT: 
                                - NAVTEX ilanlarına odaklan, trafik durumunu sadece referans olarak kullan
                                - Türkçe, profesyonel ama anlaşılır dil
                                - Konkret bilgi ver, genel laflar etme
                                - İlan numaralarını ve tarihlerini belirt
                                - Max 400 kelime";

                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                request.Headers.Add("X-goog-api-key", apiKey);

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "AI analizi başarısız",
                        statusCode = (int)response.StatusCode,
                        details = responseString
                    });
                }

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string analysis = result.candidates[0].content.parts[0].text;


                try
                {
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    };
                    await _cache.SetStringAsync(cacheKey, analysis, cacheOptions);
                }
                catch (Exception)
                {
                }

                return Ok(new { analysis = analysis, success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}