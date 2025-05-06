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
    public int remainingTime = 90;
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

    public int dogruPuani;
    public int yanlisPuani;

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
            baslangicMesaji.SetActive(true);
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
            puan += dogruPuani ;
            geciciUyari.SetActive(false);
            StartCoroutine(UpdateScore(kullaniciAdi, puan, false, true));

        }
        else{
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yield return new WaitForSeconds(beklenecekZaman);
            yanlis += 1;
            puan += yanlisPuani;
            geciciUyari.SetActive(false);
            StartCoroutine(UpdateScore(kullaniciAdi, puan, false, true));
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
        StartCoroutine(UpdateScore(kullaniciAdi, puan, false, false));
    }

    public void AnaMenuyeDon()
    {
        if(!adminMode){
            StartCoroutine(UpdateScore(kullaniciAdi, puan, true, false));
        }
        else{
            SceneManager.LoadScene("AnaMenu");
            //AnaMenu.anketSorusu.SetActive(true);
            AnaMenu.anketAcilacakMi = true;
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

    IEnumerator UpdateScore(string username, int score, bool oyunSonuMu, bool soruArasiMi){
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
                    //AnaMenu.anketSorusu.SetActive(true);
                    AnaMenu.anketAcilacakMi = true;
                }
                else{
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(FindRank(kullaniciAdi, soruArasiMi));
                }

            }
        }
    }
    
    [System.Serializable]
    public class UsernameData
    {
        public string username;
    }

    IEnumerator FindRank(string username, bool soruArasiMi){
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
            else if(!soruArasiMi){
                var json = JSON.Parse(request.downloadHandler.text);
                int rank = json["rank"].AsInt;
                siralama = rank;
            
                yield return new WaitForSeconds(0.5f);
                oyunSonuPanel.SetActive(true);
                oyunSonuText.text = "Tebrikler " + kullaniciAdi + ", " + dogru + " doğru ve " + yanlis + " yanlış ile " + puan + " puan yapıp " + siralama +" sıraya yerleştiniz";                               
            }
            else{
                yield return new WaitForSeconds(0.5f);
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
