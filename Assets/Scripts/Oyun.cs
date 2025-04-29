using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using SimpleJSON;
using System.Linq;
using JetBrains.Annotations;
using soruVeritabani;


public class Oyun : MonoBehaviour
{
    
    [Header("Oyun Zamanlayicisi")]
    public TextMeshProUGUI timerText; 
    public int remainingTime = 60;
    public bool isTimerActive = false;

    [Header("Baslangic Mesaji")]
    public TextMeshProUGUI baslangicSayaciText;
    private GameObject baslangicMesaji;

    [Header("Soru Mekanigi")]
    public TextMeshProUGUI soru;
    public TextMeshProUGUI a;
    public TextMeshProUGUI b;
    public TextMeshProUGUI c;
    public TextMeshProUGUI d;
    public Soru rastgeleSoru;
    public TextMeshProUGUI soruSayisiText;

    private int mevcutSoruSayisi = 1;

    [Header("Kullanıcı Bilgileri")]
    public string kullaniciAdi;
    public string kullaniciIP;
    public int puan;
    public int dogru;
    public int yanlis;
    public int siralama;
    public TextMeshProUGUI puanText;
    public TextMeshProUGUI dogruText;
    public TextMeshProUGUI yanlisText;

    [Header("Oyun Sonu Bilgileri")]
    public GameObject oyunSonuPanel;
    public TextMeshProUGUI oyunSonuText;
   
    [Header("Diğer Bilesenler")]
    public GameObject geciciUyari;
    public TextMeshProUGUI geciciYazi;
    public GameObject cikisMenusu;

    [Header("Admin Bilgileri")]
    public bool adminMode = false;

    private string apiUrl = "https://bilgiyarismasi-api.onrender.com";

    void Awake()
    {
        adminMode = AnaMenu.adminMode;
        Debug.Log("Awake");
        SoruDatabase.SilinenleriCagir();  // Sahne yüklendiğinde silinen soruları geri yükle
    }

    void Start()
    {
        baslangicMesaji = GameObject.Find("GirisPaneli");

        if(!adminMode){
            kullaniciAdi = AnaMenu.kullaniciAdi;
            kullaniciIP = AnaMenu.kullaniciIP;
            if(baslangicMesaji == null){
                Debug.LogError("GirisPaneli Bulunamadı");
            }
            baslangicMesaji.SetActive(true);
            StartCoroutine(BaslangicSayaci());
        }
        else{
            kullaniciAdi = "admin";
            kullaniciIP = "0.0.0.0";
            baslangicMesaji.SetActive(false);
            yeniSoruOlustur();
        }
    }

    void yeniSoruOlustur()
    {
        rastgeleSoru = SoruDatabase.RastgeleSoruGetir();

        soru.text = rastgeleSoru.soruMetni;
        a.text = rastgeleSoru.soruASikki;
        b.text = rastgeleSoru.soruBSikki;
        c.text = rastgeleSoru.soruCSikki;
        d.text = rastgeleSoru.soruDSikki;
    }

    void Update()
    {
        if(isTimerActive)
        {
            StartCoroutine(StartCountdown());
            isTimerActive = false;
        }
        soruSayisiText.text = "Soru " + mevcutSoruSayisi;
        puanText.text = puan.ToString();
        dogruText.text = dogru.ToString();
        yanlisText.text = yanlis.ToString();
        
    }

    IEnumerator BirazBekle(float beklenecekZaman, bool dogruMu){
        mevcutSoruSayisi += 1;
        if(dogruMu){
            geciciYazi.text = "Doğru";
            geciciUyari.SetActive(true);
            yield return new WaitForSeconds(beklenecekZaman);
            dogru += 1;
            puan += 100 ;
            geciciUyari.SetActive(false);
        }
        else{
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yield return new WaitForSeconds(beklenecekZaman);
            yanlis += 1;
            puan -=20;
            geciciUyari.SetActive(false);
        }
        yeniSoruOlustur();

    }

    IEnumerator BaslangicSayaci(){
        int kalanZaman = 5;
        while (kalanZaman > 0){
            baslangicSayaciText.text = kalanZaman.ToString();
            yield return new WaitForSeconds(1f);
            kalanZaman--;
        }
        baslangicSayaciText.text = "Başarılar!";
        
        yeniSoruOlustur();
        baslangicMesaji.SetActive(false);
        isTimerActive = true;
    }

    IEnumerator StartCountdown()
    {
        while (remainingTime > 0)
        {
            timerText.text = remainingTime + "s";
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        isTimerActive = false;
        timerText.text = "Süre doldu!";
        cikisMenusu.SetActive(false);
        SureBitti();
    }

    public void SureBitti(){
        StartCoroutine(UpdateScore(kullaniciAdi, puan, false));
    }

    public void AnaMenuyeDon()
    {
        if(!adminMode){
            StartCoroutine(UpdateScore(kullaniciAdi, puan, true));
        }
        else{
            SceneManager.LoadScene("AnaMenu");
        }
    }

    public void AyaBasti()
    {
        cevapKontrol('A');
    }
    public void ByeBasti()
    {
        cevapKontrol('B');
    }
    public void CyeBasti()
    {
        cevapKontrol('C');
    }
    public void DyeBasti()
    {
        cevapKontrol('D');
    }
    
    public void UyariKapandi()
    {
        geciciUyari.SetActive(false);
    }

    private void cevapKontrol(char cevap){
        if(rastgeleSoru.dogruSik == cevap)
        {
            StartCoroutine(BirazBekle(1f, true));
        }
        else
        {
            StartCoroutine(BirazBekle(1f, false));
        }
    }

    [SerializeField]
    private class GuncellenecekData{
        public string username;
        public int score;
    }

    IEnumerator UpdateScore(string username, int score, bool oyunSonuMu){
        GuncellenecekData yeniData = new GuncellenecekData
        {
        username = username,
        score =score
        };

        string updateScoreApiUrl = apiUrl + "/update_score";

         string jsonData = JsonUtility.ToJson(yeniData);
         byte[] jsonToSendBytes = new UTF8Encoding().GetBytes(jsonData);

         using (UnityWebRequest request = new UnityWebRequest(updateScoreApiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSendBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)//gelen veri hata mesaji ise
            {
                Debug.LogError($"API Gönderme Hatası ({updateScoreApiUrl}): {request.error}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
            }
            else//hata mesaji degil ise
            {
                Debug.Log("Skor başarıyla Güncellendi");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.Log("Sunucu Yanıtı: " + request.downloadHandler.text);
                }
                if(oyunSonuMu){
                    yield return new WaitForSeconds(1f);
                    Debug.Log("Kullanıcı Sıraya Kaydedildi.");
                    SceneManager.LoadScene("AnaMenu");
                }
                else{
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(FindRank(kullaniciAdi));
                }

            }
        }
    }
    
    [System.Serializable]
    public class UsernameData
    {
        public string username;
    }

    IEnumerator FindRank(string username){
        string findRankApiUrl = apiUrl + "/get_rank";

        UsernameData data = new UsernameData();

        data.username = username;

        string jsonData = JsonUtility.ToJson(data);
        byte[] jsonToSendBytes = new UTF8Encoding().GetBytes(jsonData);

        using(UnityWebRequest request = new UnityWebRequest(findRankApiUrl, "POST")){
            request.uploadHandler = new UploadHandlerRaw(jsonToSendBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)//gelen veri hata mesaji ise
            {
                Debug.LogError($"API Gönderme Hatası ({findRankApiUrl}): {request.error}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
            }
            else{
                var json = JSON.Parse(request.downloadHandler.text);
                int rank = json["rank"].AsInt;
                siralama = rank;
            
                yield return new WaitForSeconds(0.5f);
                oyunSonuPanel.SetActive(true);
                oyunSonuText.text = "Tebrikler " + kullaniciAdi + ", " + dogru + " doğru ve " + yanlis + " yanlış ile " + puan + " puan yapıp " + siralama +" sıraya yerleştiniz";                               
            }
        }
    }

    public void CikmaTusu(){
        cikisMenusu.SetActive(true);
    }
    public void CikmaTusuIptal(){
        cikisMenusu.SetActive(false);
    }
}


/*public class Soru
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
}*/
/*public static class SoruVeritabani
{
    private static List<Soru> sorular = new List<Soru>()
    {
        new Soru(1, "Türkiye'nin başkenti neresidir?", "İstanbul", "Ankara", "Sakarya", "Bursa", 'B'),
        new Soru(2, "Dünya'nın en büyük okyanusu hangisidir?", "Hint", "Atlas", "Pasifik", "Arktik", 'C'),
        new Soru(3, "2 + 2 kaç eder?", "3", "4", "5", "6", 'B'),
    };

    private static List<Soru> silinenSorular = new List<Soru>(){
    };

    public static Soru RastgeleSoruGetir()
    {
        if (sorular.Count == 0)
        {
            // Eğer sorular listesi boşsa, silinen soruları geri çağırıyoruz
            //SilinenleriCagir();
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
        Debug.Log("Silineni Çağır Çalışıyor");

        // Eğer silinen sorular varsa, bunları tekrar sorular listesine ekleyelim
        while (silinenSorular.Count > 0)
        {
            sorular.Add(silinenSorular[0]);
            silinenSorular.RemoveAt(0);  // İlk elemanı çıkararak ekle
        }

        Debug.Log("Silinen Sorular Geri Aktarıldı.");
    }
}
*/
