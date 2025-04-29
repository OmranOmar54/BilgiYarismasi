using UnityEngine;
using System.Collections.Generic;

public class Soru
{
    public int soruID;
    public string soruMetni;
    public string soruASikki;
    public string soruBSikki;
    public string soruCSikki;
    public string soruDSikki;
    public char dogruSik;
    public Soru(int id, string soru, string a, string b, string c, string d, char dogru)
    {
        soruID = id;
        soruMetni = soru;
        soruASikki = a;
        soruBSikki = b;
        soruCSikki = c;
        soruDSikki = d;
        dogruSik = dogru;
    }
}

namespace soruVeritabani
{
    public static class SoruDatabase
    {
        // sorular listesine dışarıdan erişim sağlamak için public yapıyoruz
        public static List<Soru> sorular = new List<Soru>()
        {
            new Soru(1, "Hangi elementin kimyasal sembolü 'O'dur?", "Oksijen", "Ozon", "Okyanus", "Opiat", 'A'),
            new Soru(2, "Periyodik tabloda hangi grup halojenler olarak bilinir?", "1. grup", "7. grup", "2. grup", "6. grup", 'B'),
            new Soru(3, "Suyun kimyasal formülü nedir?", "H2O", "CO2", "H2O2", "NH3", 'A'),
            new Soru(4, "Karbon atomu hangi grupta yer alır?", "14. grup", "16. grup", "12. grup", "1. grup", 'A'),
            new Soru(5, "Hangi gaz atmosferde en fazla miktarda bulunur?", "Oksijen", "Azot", "Karbon dioksit", "Amonyak", 'B'),
            new Soru(6, "Hangi elementin kimyasal sembolü 'Na'dır?", "Nikel", "Nadir", "Sodyum", "Nitrit", 'C'),
            new Soru(7, "Su ile birleşerek asidik özellik gösteren gaz nedir?", "Karbon dioksit", "Oksijen", "Azot", "Metan", 'A'),
            new Soru(8, "Hidrojen gazı hangi özelliklere sahiptir?", "Yanıcıdır", "Zehirlidir", "Renksizdir", "Tüm şıkları içerir", 'D'),
            new Soru(9, "Hangi elementin kimyasal sembolü 'Fe'dir?", "Bakır", "Demir", "Kurşun", "Çinko", 'B'),
            new Soru(10, "Bir asidin sudaki çözeltisi hangi iyonu içerir?", "OH⁻", "H⁺", "Na⁺", "CO₃²⁻", 'B'),
            new Soru(11, "Sodium klorür (NaCl) hangi tür bir bileşiktir?", "Asit", "Baz", "Tuz", "Alkali", 'C'),
            new Soru(12, "Hangi elementin sembolü 'Cl'dir?", "Klor", "Karbon", "Sodyum", "Kükürt", 'A'),
            new Soru(13, "Hidrojen ve oksijenin birleşerek su oluşturmasına ne denir?", "Kondensasyon", "Evaporizasyon", "Yoğunlaşma", "Sentez", 'D'),
            new Soru(14, "Hangi gaz yanıcıdır?", "Oksijen", "Karbon dioksit", "Amonyak", "Metan", 'D'),
            new Soru(15, "Kimyasal reaksiyon sırasında enerji salınımına ne denir?", "Endoterm", "Egzoterm", "Anhidrit", "Moleküler", 'B'),
            new Soru(16, "İçeriğinde bir veya daha fazla hidrojen iyonu bulunan bileşikler nedir?", "Asitler", "Bazlar", "Tuzlar", "Alkol", 'A'),
            new Soru(17, "Hidrojen peroksit'in kimyasal formülü nedir?", "H2O", "H2O2", "CO2", "NH3", 'B'),
            new Soru(18, "İnorganik bileşikler için doğru bir tanım nedir?", "Yalnızca karbon ve hidrojen içerir", "Canlılar tarafından üretilir", "Genellikle su, tuz ve mineraller içerir", "Her zaman organik maddelerle reaksiyona girer", 'C'),
            new Soru(19, "Bir maddenin erime noktası, bu maddenin hangi özelliğini gösterir?", "Elektriksel iletkenlik", "Fiziksel durum değişimi", "Kimyasal yapı", "Boiling point", 'B'),
            new Soru(20, "Hangi element sıvı halde oda sıcaklığında bulunur?", "Civa", "Sodyum", "Oksijen", "Karbon", 'A'),
            new Soru(21, "Atomun çekirdeğinde hangi parçacıklar bulunur?", "Elektron ve proton", "Nötron ve proton", "Elektron ve nötron", "Proton ve iyon", 'B'),
            new Soru(22, "Oksijen, hangi element grubuna aittir?", "Halojenler", "Soy gazlar", "Alkali metaller", "Kalkojenler", 'D'),
            new Soru(23, "Hangi madde doğal gaz olarak bilinir?", "Metan", "Etanol", "Aseton", "Karbon dioksit", 'A'),
            new Soru(24, "Hangi gaz su buharı ile birleşerek asidik özellik gösterir?", "Karbon dioksit", "Oksijen", "Amonyak", "Azot", 'A'),
            new Soru(25, "Hidrojen iyonunun gösterimi nedir?", "OH⁻", "H⁺", "Na⁺", "O²⁻", 'B'),
            new Soru(26, "Oksitlerin tanımı nedir?", "Sadece karbon ve hidrojen içerir", "Genellikle asidik özellik gösterir", "Oksijen ile birleşmiş elementlerden oluşur", "Asidik çözeltisi vardır", 'C'),
            new Soru(27, "Nötr bir çözelti hangi pH değerine sahiptir?", "0", "7", "14", "4", 'B'),
            new Soru(28, "Hangi maddeler elektronları kabul eder?", "Asitler", "Bazlar", "Alkoler", "Amonyum bileşenleri", 'A'),
            new Soru(29, "Kimyasal reaksiyon sonucu meydana gelen maddeye ne denir?", "Ürün", "Reaktif", "İyon", "Molekül", 'A'),
            new Soru(30, "Hangi madde genellikle karbon dioksit (CO2) üretir?", "Asidik çözeltiler", "Organik asitler", "Su buharı", "Gazlı metaller", 'B'),
            new Soru(31, "Yalnızca bir tür atomdan oluşan maddelere ne denir?", "Bileşikler", "Elementler", "Karışımlar", "Çözeltiler", 'B'),
            new Soru(32, "Bir atomun kimyasal sembolü nedir?", "Birkaç harften oluşan simge", "Yalnızca sayılar", "Her zaman iki harften oluşur", "Karışık simgeler", 'C'),
            new Soru(33, "Sülfürik asidin kimyasal formülü nedir?", "HCl", "H2SO4", "HNO3", "H3PO4", 'B'),
            new Soru(34, "Sodyum, hangi grup elementidir?", "Alkali metal", "Halojen", "Soy gaz", "Toprak alkali metal", 'A'),
            new Soru(35, "Karbon dioksit hangi gazın ürünü olabilir?", "Yanma", "Bozulma", "Soğutma", "Buharlaşma", 'A'),
            new Soru(36, "Periyodik tablodaki hangi grup soy gazlar olarak bilinir?", "1. grup", "2. grup", "17. grup", "18. grup", 'D'),
            new Soru(37, "Beyaz fosfor, hangi tür bir madde örneğidir?", "Element", "Bileşik", "Karışım", "Çözelti", 'A'),
            new Soru(38, "Bir maddede yalnızca bir tür molekül bulunuyorsa, bu maddeye ne denir?", "Karışım", "Element", "Bileşik", "Çözelti", 'B'),
            new Soru(39, "Bir molekülün kimyasal formülünde atom sayılarının oranını ne gösterir?", "İyonik bağ", "Mol oranı", "Sembolizm", "Stoikiyometri", 'D'),
            new Soru(40, "Hangi elementin kimyasal sembolü 'Ca'dır?", "Kalsiyum", "Karbon", "Krom", "Çinko", 'A'),
            new Soru(41, "Hidrojen peroksit hangi maddelerle reaksiyona girer?", "Oksijen", "Aseton", "Sülfür", "Asitler", 'A'),
            new Soru(42, "Etil alkol hangi tip bir bileşiktir?", "Asit", "Alkol", "Karbonat", "Tuz", 'B'),
            new Soru(43, "Kimyasal bağlar ne tür enerji üretir?", "Elektrik", "Termal enerji", "Kimyasal enerji", "Mekanik enerji", 'C'),
            new Soru(44, "Hangi element gaz halindedir ve 0°C'de kokusuzdur?", "Karbon", "Amonyak", "Oksijen", "Azot", 'D'),
            new Soru(45, "Alkali metaller hangi özellikleriyle bilinir?", "Yüksek erime noktası", "Su ile reaksiyona girerek hidrojen gazı oluşturma", "Elektriksel iletkenlik", "Asidik özellik", 'B'),
            new Soru(46, "Hidrojen klorür (HCl) hangi özelliklere sahiptir?", "Asidik", "Bazik", "Alkali", "Nötr", 'A'),
            new Soru(47, "Hangi maddeler suya çözündüklerinde asidik özellik gösterir?", "Tuzlar", "Bazlar", "Asitler", "Alkol", 'C'),
            new Soru(48, "Hangi madde sıvı halde oda sıcaklığında bulunur?", "Aseton", "Civa", "Karbon", "Metan", 'B'),
            new Soru(49, "Etil alkolün formülü nedir?", "CH3OH", "C2H5OH", "C3H7OH", "C4H9OH", 'B'),
            new Soru(50, "Hangi elementin atom numarası 8'dir?", "Karbon", "Amonyak", "Oksijen", "Kükürt", 'C'),
            new Soru(51, "Periyodik tabloda hangi grup soy gazlar olarak bilinir?", "1. grup", "2. grup", "17. grup", "18. grup", 'D'),
            new Soru(52, "Hidrojen gazı hangi gazlar ile reaksiyona girer?", "Azot", "Karbon dioksit", "Oksijen", "Helyum", 'C'),
            new Soru(53, "Bromun kimyasal sembolü nedir?", "Br", "B", "Be", "Bi", 'A'),
            new Soru(54, "Hidrojen peroksit ne tür reaksiyonlarla bozunur?", "Asidik", "Oksidasyon", "Redüksiyon", "Polimerizasyon", 'B'),
            new Soru(55, "Periyodik tablodaki hangi grup halojenler olarak bilinir?", "1. grup", "7. grup", "2. grup", "18. grup", 'B'),
            new Soru(56, "Hidrojen gazının kimyasal formülü nedir?", "H2O", "H2", "CO2", "O2", 'B'),
            new Soru(57, "Kimyasal reaksiyonlarda hangi madde reaksiyona girer?", "Ürün", "Katkı", "Reaktif", "Katalizör", 'C'),
            new Soru(58, "Hangi asit çürük dişlerin başlıca nedenidir?", "Sülfürik asit", "Askorbik asit", "Asetik asit", "Laktik asit", 'D'),
            new Soru(59, "Karbonat iyonunun kimyasal sembolü nedir?", "CO3²⁻", "CO2", "HCO3⁻", "H2O", 'A'),
            new Soru(60, "Kimyasal reaksiyonun hızını etkileyen faktörlerden biri nedir?", "Isı", "Reaktiflerin rengi", "Kimyasal bağlar", "Molekül ağırlığı", 'A')
        };

        public static List<Soru> silinenSorular = new List<Soru>(){};

        public static Soru RastgeleSoruGetir()
        {
            if (sorular.Count == 0)
            {
                return new Soru(0, "", "", "", "", "", 's');
            }

            int index = Random.Range(0, sorular.Count);
            Soru selectedSoru = sorular[index];  // Soruyu seç
            silinenSorular.Add(selectedSoru);  // Seçilen soruyu silinen sorulara ekle
            sorular.RemoveAt(index);  // Seçilen soruyu listeden çıkar
            return selectedSoru;  // Seçilen soruyu geri döndür
        }

        public static void SilinenleriCagir()
        {
            while (silinenSorular.Count > 0)
            {
                sorular.Add(silinenSorular[0]);
                silinenSorular.RemoveAt(0);  // İlk elemanı çıkararak ekle
            }
        }
    }
}