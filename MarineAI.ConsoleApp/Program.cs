using System; // Temel sistem fonksiyonları için gerekli
using System.Collections.Generic; // List gibi koleksiyon sınıfları için gerekli
using System.Net.Http; // HTTP istekleri yapmak için gerekli
using System.Text.Json; // JSON serileştirme/deserileştirme için gerekli
using System.Text.RegularExpressions; // Regex işlemleri için gerekli
using System.Threading.Tasks; // Asenkron programlama için gerekli

class Ship // Gemi bilgilerini tutacak sınıf tanımı
{
    public int Type { get; set; } // Gemi tipi (0, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13 değerlerinden biri)
    public int Unknown1 { get; set; } // API'den gelen bilinmeyen bir alan
    public long MMSI { get; set; } // Geminin MMSI (Maritime Mobile Service Identity) kimlik numarası
    public string Name { get; set; } // Geminin adı
    public double Latitude { get; set; } // Geminin enlem koordinatı
    public double Longitude { get; set; } // Geminin boylam koordinatı
    public double Speed { get; set; } // Geminin hızı (knot cinsinden)
    public double Course { get; set; } // Geminin rotası/yönü (0-359 derece arası)
}

class Program // Ana program sınıfı
{
    static async Task Main(string[] args) // Asenkron ana metod
    {
        // MyShipTracking sitesinden veri çekmek için kullanılacak URL
        // URL parametreleri: minlat/maxlat (enlem sınırları), minlon/maxlon (boylam sınırları), zoom seviyesi, gemi tipleri filtresi vb.
        string url = "https://www.myshiptracking.com/requests/vesselsonmaptempTTT.php?embed=1&type=json&minlat=34.03445260967645&maxlat=42.79540065303723&minlon=22.434082031250004&maxlon=38.16650390625001&zoom=6&selid=null&seltype=null&timecode=-1&slmp=vd8dz&filters=%7B%22vtypes%22%3A%22%2C0%2C3%2C4%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%22%2C%22minsog%22%3A0%2C%22maxsog%22%3A60%2C%22minsz%22%3A10%2C%22maxsz%22%3A500%2C%22minyr%22%3A1950%2C%22maxyr%22%3A2025%2C%22flag%22%3A%22%22%2C%22status%22%3A%22%22%2C%22mapflt_from%22%3A%22%22%2C%22mapflt_dest%22%3A%22%22%7D&_=1759256854012";

        using var client = new HttpClient(); // HTTP istemci nesnesi oluşturma (using ile otomatik dispose edilecek)
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"); // Web tarayıcısı gibi görünmek için User-Agent header'ı ekleme

        var response = await client.GetStringAsync(url); // URL'den asenkron olarak veri çekme ve string olarak alma

        var ships = new List<Ship>(); // Gemi bilgilerini tutacak liste oluşturma
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries); // Split('\n )Gelen veriyi satırlara bölme, StringSplitOptions.RemoveEmptyEntries= boş satırları atlama 
                                                                                 // buradan gelen değelerin hepsini array olarak tutuyoruz artık her satırı tek tek işleyebiliriz

        foreach (var line in lines) // Her satır için döngü başlatma
        {
            var parts = Regex.Split(line.Trim(), @"\s+"); // Satırı boşluk/tab karakterlerine göre parçalara ayırma (regex ile birden fazla boşluğu tek kabul etme)
            if (parts.Length < 7) continue; // Eğer satırda minimum 7 alan yoksa bu satırı atla

            try // Hata yakalama bloğu başlangıcı
            {
                // Course (rota) değerini al - eğer 8. eleman varsa onu al, yoksa 0 kabul et
                double course = parts.Length > 7 ? double.Parse(parts[7].Replace(",", ".")) : 0;

                // Course değeri 359'dan büyükse düzeltme algoritması
                while (course > 359) // Course 359'dan büyük olduğu sürece döngüye devam et
                {
                    string courseStr = ((int)course).ToString(); // Course'u tam sayıya çevirip string'e dönüştür
                    if (courseStr.Length > 1) // Eğer birden fazla basamak varsa
                        courseStr = courseStr.Substring(0, courseStr.Length - 1); // En sağdaki basamağı sil
                    else // Tek basamak kaldıysa
                        courseStr = "0"; // Değeri sıfırla

                    course = double.Parse(courseStr); // String'i tekrar double'a çevir
                }

                var ship = new Ship // Yeni gemi nesnesi oluştur
                {
                    Type = int.Parse(parts[0]), // İlk parça: Gemi tipi (string'den int'e çevir)
                    Unknown1 = int.Parse(parts[1]), // İkinci parça: Bilinmeyen alan (string'den int'e çevir)
                    MMSI = long.Parse(parts[2]), // Üçüncü parça: MMSI numarası (string'den long'a çevir)
                    Name = parts[3], // Dördüncü parça: Gemi adı (string olarak direkt al)
                    Latitude = double.Parse(parts[4].Replace(",", ".")), // Beşinci parça: Enlem (virgülü noktaya çevirip double'a parse et)
                    Longitude = double.Parse(parts[5].Replace(",", ".")), // Altıncı parça: Boylam (virgülü noktaya çevirip double'a parse et)
                    Speed = double.Parse(parts[6].Replace(",", ".")), // Yedinci parça: Hız (virgülü noktaya çevirip double'a parse et)
                    Course = course // Düzeltilmiş rota değerini ata
                };

                ships.Add(ship); // Oluşturulan gemi nesnesini listeye ekle
            }
            catch // Parse hatası veya herhangi bir hata durumunda
            {
                continue; // Bu satırı atla ve bir sonraki satıra geç
            }
        }

        var options = new JsonSerializerOptions { WriteIndented = true }; // JSON formatında girintili (okunabilir) çıktı için ayarlar
        string json = JsonSerializer.Serialize(ships, options); // Gemi listesini JSON string'e dönüştür

        Console.WriteLine("✅ JSON verisi hazır:\n"); // Başarı mesajı göster
        Console.WriteLine(json); // JSON verisini konsola yazdır
    }
}