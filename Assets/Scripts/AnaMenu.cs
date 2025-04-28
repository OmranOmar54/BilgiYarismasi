using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class AnaMenu : MonoBehaviour
{
    public GameObject hakkindaMenusu;
    private bool hakkindaAcikMi = false;
    public TextMeshProUGUI kullaniciAdiAlani; 
    public static string kullaniciAdi;
    public static string kullaniciIP;
    public int kullaniciScore;
    public GameObject IDHatasi;
    public TextMeshProUGUI IDHatasiText;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContentPanel;
    public TextMeshProUGUI leaderboardStatusText;
    public GameObject klavye;
    public static string apiUrl = "https://bilgiyarismasi-api.onrender.com";

    public static bool KullaniciAdiUygunMu(string input){
        if (string.IsNullOrEmpty(input)){
            return false;
        }
        return Regex.IsMatch(input, @"[çğıöşüÇĞİÖŞÜ\s]");
    }


    public void OyunaBasla() //Oyuna basla butonunda cagirilacak script
    {
        kullaniciAdi = kullaniciAdiAlani.text; //kullanici adi textini aliyor
        if(!KullaniciAdiUygunMu(kullaniciAdi)){
            if (!string.IsNullOrEmpty(kullaniciAdi)) //kullanici adi bos degil ise
            {
                if(!Regex.IsMatch(kullaniciAdi, @"[qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM]")){
                    IDHatasi.SetActive(true);
                    IDHatasiText.text = "Kullanıcı Adı Yalnızca Rakam veya Karakter İçeremez";
                }
                else{
                    StartCoroutine(CheckUsername(kullaniciAdi));
                    IDHatasi.SetActive(false); 
                }
            }

            else//kullanici adi bos ise
            {
                IDHatasi.SetActive(true);
                IDHatasiText.text = "Kullanıcı Adı Boş Bırakılamaz";

            }
        }
        else{
            Debug.LogError("Kullanıcı Adı Uygun Değil");
            IDHatasi.SetActive(true);
            if(Regex.IsMatch(kullaniciAdi, @"[\s]")){
                IDHatasiText.text = "Kullanıcı Adı Boşluk Barındıramaz";
            }
            else if(Regex.IsMatch(kullaniciAdi, @"[çğıöşüÇĞİÖŞÜ]")){
                IDHatasiText.text = "Kullanıcı Adı Türkçe Karakter Barındıramaz";
            }
        }
        
    }

    public class UsernameToSend{
        public string username;
    }

    [System.Serializable]
    public class ResponseData{
        public bool success;
        public string message;
    }

    IEnumerator CheckUsername(string username)
    {
        yield return null;
        UsernameToSend nameToSend = new UsernameToSend{
            username = username
        };

        string getUsernameApi = apiUrl + "/check_username"; 

        string jsonData = JsonUtility.ToJson(nameToSend);
        byte[] jsonToSendBytes = new UTF8Encoding().GetBytes(jsonData);

        using(UnityWebRequest request = new UnityWebRequest(getUsernameApi, "POST")){
            request.uploadHandler = new UploadHandlerRaw(jsonToSendBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
        
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            ResponseData response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            Debug.Log(response);

            if(response.success)
            {
                Debug.LogWarning("Kullanıcı Bulundu.");
                IDHatasi.SetActive(true);
                IDHatasiText.text = "Bu Kullanıcı Adı Zaten Kullanılmış. Lütfen Başka Kullanıcı Adı Seçiniz";
            }
            else
            {
                Debug.LogWarning("Kullanıcı Bulunamadı.");
                StartCoroutine(GetIPAndProceed()); 
            }    
        }
    }

    IEnumerator GetIPAndProceed()//kullanicinin bilgilerini topla
    {
        Debug.Log("GetIPAndProceed başladı.");
        UnityWebRequest ipRequest = UnityWebRequest.Get("https://api.ipify.org");//ip alma api'si
        Debug.Log("IP isteği gönderiliyor...");
        yield return ipRequest.SendWebRequest();
        Debug.Log("IP isteği tamamlandı. Sonuç: " + ipRequest.result);

        if (ipRequest.result == UnityWebRequest.Result.Success)
        {
            kullaniciIP = ipRequest.downloadHandler.text;
            Debug.Log("Kullanıcının IP Adresi: " + kullaniciIP);

            kullaniciScore = 0;
            StartCoroutine(SendScoreToServer(kullaniciAdi, kullaniciIP, kullaniciScore));//kullanici verilerini database'e gonderme islemini baslat
        
        }

        else
        {
            Debug.LogError("IP Alınamadı: " + ipRequest.error);
            IDHatasi.SetActive(true);
        }

        ipRequest.Dispose();
    }

    [System.Serializable]
    private class ScoreData//yeni data turu tanimlama
    {
        public string username;
        public string ip;
        public int score;
    }

    public static IEnumerator SendScoreToServer(string username, string ip, int scoreValue)
    {
        ScoreData dataToSend = new ScoreData
        {
            username = username,
            ip = ip,
            score = scoreValue
        };

        string jsonData = JsonUtility.ToJson(dataToSend);//datayi json fomatina donusturme
        byte[] jsonToSendBytes = new UTF8Encoding().GetBytes(jsonData);//json dosyasini uygun forma getirme

        string addScoreUrl = apiUrl + "/add_score";//datayi api'nin ilgili alt basligina yonlendirme

        using (UnityWebRequest request = new UnityWebRequest(addScoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSendBytes);//veriyi gonderme
            request.downloadHandler = new DownloadHandlerBuffer();//gelen veriyi takip etme

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)//gelen veri hata mesaji ise
            {
                Debug.LogError($"API Gönderme Hatası ({addScoreUrl}): {request.error}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
            }
            else//hata mesaji degil ise
            {
                Debug.Log("Skor başarıyla gönderildi!");
                SceneManager.LoadScene("Oyun");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.Log("Sunucu Yanıtı: " + request.downloadHandler.text);
                }
            }
        }
    }

    public void Hakkinda()//hakkinda tusu
    {
        hakkindaAcikMi = !hakkindaAcikMi;
        hakkindaMenusu.SetActive(hakkindaAcikMi);
    }

    public void HataKapat()//id hatasi kapatma tusu
    {
        IDHatasi.SetActive(false);
    }

     [System.Serializable]
    public class LeaderboardEntry//yeni data turu tanimliyoruz
    {
        public int rank;
        public string username;
        public int score;
    }

    public static class JsonHelper//json dosyayi stringe cevirme
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

      public void RequestLeaderboardData()
    {
        StartCoroutine(FetchLeaderboardDataCoroutine());//databaseden datayi isteme islemi
    }

    IEnumerator FetchLeaderboardDataCoroutine()
    {
        ClearLeaderboardUI();//liderlik tablosu dolu ise temizle
        if (leaderboardStatusText != null) leaderboardStatusText.text = "Yükleniyor...";

        string fullUrl = apiUrl + "/get_leaderboard";
        Debug.Log("Liderlik tablosu isteniyor: " + fullUrl);

        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)//gelen data hata iceriyorsa
            {
                Debug.LogError($"API Okuma Hatası ({fullUrl}): {request.error}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
                if (leaderboardStatusText != null) leaderboardStatusText.text = "Liderlik Tablosu yüklenemedi!\n(Hata: " + request.error + ")";
            }
            else if (request.result == UnityWebRequest.Result.Success)//hata icermiyorsa
            {
                Debug.Log("Liderlik tablosu başarıyla alındı!");
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Sunucu Yanıtı (JSON): " + jsonResponse);

                if (leaderboardStatusText != null) leaderboardStatusText.text = "";

                ProcessLeaderboardJson(jsonResponse);//gelen datayi isleme sureci
            }
        }
    }

    void ProcessLeaderboardJson(string json)
    {
        if (string.IsNullOrEmpty(json))//json verisi bos mu
        {
            Debug.LogError("Alınan JSON verisi boş.");
            if (leaderboardStatusText != null) leaderboardStatusText.text = "Liderlik tablosu boş veya alınamadı.";
            return;
        }

        try
        {
            LeaderboardEntry[] entries = JsonHelper.FromJson<LeaderboardEntry>(json);

            if (entries == null || entries.Length == 0)
            {
                 Debug.LogWarning("Liderlik tablosunda gösterilecek veri yok.");
                 if (leaderboardStatusText != null) leaderboardStatusText.text = "Henüz skor kaydedilmemiş.";
                 return;
            }

            if (leaderboardEntryPrefab == null || leaderboardContentPanel == null)
            {
                Debug.LogError("Leaderboard Entry Prefab veya Content Panel atanmamış!");
                if (leaderboardStatusText != null) leaderboardStatusText.text = "UI Ayarları Eksik!";
                return;
            }

            foreach (var entry in entries)
            {
                GameObject entryGO = Instantiate(leaderboardEntryPrefab, leaderboardContentPanel);//gelen verilere gore prefab ekleme

                TextMeshProUGUI rankText = entryGO.transform.Find("Rank")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI nameText = entryGO.transform.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = entryGO.transform.Find("Puan")?.GetComponent<TextMeshProUGUI>();

                if (rankText != null)
                {
                    rankText.text = entry.rank.ToString() + ".";
                } 
                else 
                {
                    Debug.LogWarning($"Prefab '{leaderboardEntryPrefab.name}' içinde 'RankText' isimli TMP UGUI objesi bulunamadı.");
                }

                if (nameText != null)
                {
                    nameText.text = entry.username;
                }
                else 
                {
                     Debug.LogWarning($"Prefab '{leaderboardEntryPrefab.name}' içinde 'NameText' isimli TMP UGUI objesi bulunamadı.");
                }

                if (scoreText != null)
                {
                    scoreText.text = entry.score.ToString();
                } 
                else 
                {
                     Debug.LogWarning($"Prefab '{leaderboardEntryPrefab.name}' içinde 'ScoreText' isimli TMP UGUI objesi bulunamadı.");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON işlenirken veya UI güncellenirken hata oluştu: {e.Message}\nStackTrace: {e.StackTrace}\nGelen JSON: {json}");
            if (leaderboardStatusText != null) leaderboardStatusText.text = "Veri işlenirken bir hata oluştu.";
        }
    }

    void ClearLeaderboardUI()//liderlik tablosu temizleme
    {
        if (leaderboardContentPanel == null) return;

        foreach (Transform child in leaderboardContentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void Start()
    {
        RequestLeaderboardData();//sahne acilir acilmaz liderlik tablosu verisini iste
    }

    public void KlavyeAcilipKapanmasi()//kullanici adi alanina tiklayinca acilan kisim
    {
        klavye.SetActive(true);
    }
}