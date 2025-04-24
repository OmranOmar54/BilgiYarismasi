using System.Collections;
using System.Text; // JSON için Encoding
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems; // UnityWebRequest için gerekli

public class AnaMenu : MonoBehaviour
{
    public GameObject hakkindaMenusu;
    private bool hakkindaAcikMi = false;

    public TMP_InputField kullaniciAdiAlani;
    private TouchScreenKeyboard keyboard;
    string kullaniciAdi;
    string kullaniciIP;
    public GameObject IDHatasi;

    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContentPanel;
    public TextMeshProUGUI leaderboardStatusText;

    // Replit API'nizin tam URL'si (endpoint dahil)
    // Senin Gonder metodundaki URL'yi baz aldım ve /add_score ekledim.
    // Eğer Python scriptindeki endpoint farklıysa burayı güncellemelisin.
    private string apiUrl = "https://327eb718-351c-489b-9456-dab47851ab47-00-3st11zvtfmg0m.sisko.replit.dev";

    // --- Oyuna Başlama ve Kullanıcı Bilgisi Alma ---

    public void OyunaBasla()
    {
        Debug.Log("OyunaBasla çağrıldı.");
        kullaniciAdi = kullaniciAdiAlani.text;

        if (!string.IsNullOrEmpty(kullaniciAdi))
        {
            IDHatasi.SetActive(false); // Hata mesajını gizle (varsa)
            StartCoroutine(GetIPAndProceed()); // IP al ve devam et
        }
        else
        {
            IDHatasi.SetActive(true); // Kullanıcı adı boşsa hata göster
        }
    }

    IEnumerator GetIPAndProceed()
    {
        Debug.Log("GetIPAndProceed başladı.");
        UnityWebRequest ipRequest = UnityWebRequest.Get("https://api.ipify.org");
        Debug.Log("IP isteği gönderiliyor...");
        yield return ipRequest.SendWebRequest();
        Debug.Log("IP isteği tamamlandı. Sonuç: " + ipRequest.result);

        if (ipRequest.result == UnityWebRequest.Result.Success)
        {
            kullaniciIP = ipRequest.downloadHandler.text;
            Debug.Log("Kullanıcının IP Adresi: " + kullaniciIP);

            int placeholderScore = 100; // Şimdilik sabit bir skor
            StartCoroutine(SendScoreToServer(kullaniciAdi, kullaniciIP, placeholderScore));
            // --- Bitiş ÖNEMLİ NOT ---

            
            //SceneManager.LoadScene("Oyun");
        }
        else
        {
            Debug.LogError("IP Alınamadı: " + ipRequest.error);
            // İsteğe bağlı: Kullanıcıya IP alınamadığına dair bir mesaj gösterebilirsin
            // Örneğin: IDHatasi.GetComponentInChildren<TextMeshProUGUI>().text = "IP Adresi Alınamadı!";
            IDHatasi.SetActive(true);
        }

        // Kullanılmayan istek nesnesini temizle
        ipRequest.Dispose();
    }

    // --- Skor Gönderme ---

    // JSON'a çevirmek için yardımcı bir sınıf (Python scriptinin beklediği alan adlarıyla)
    [System.Serializable] // JsonUtility'nin bu sınıfı işlemesi için gerekli
    private class ScoreData
    {
        public string username;
        public string ip; // Python scripti 'ip_address' bekliyorsa burayı ona göre değiştir. Önceki örneğe göre 'ip' idi.
        public int score; // Python scripti 'score' bekliyor. İsim eşleşmeli.
    }

    // Veriyi JSON olarak sunucuya gönderen Coroutine
    IEnumerator SendScoreToServer(string username, string ip, int scoreValue)
    {
        // Gönderilecek veriyi oluştur
        ScoreData dataToSend = new ScoreData
        {
            username = username,
            ip = ip, // Python scripti 'ip_address' bekliyorsa: ip_address = ip,
            score = scoreValue
        };

        // Veriyi JSON formatına çevir
        string jsonData = JsonUtility.ToJson(dataToSend);
        byte[] jsonToSendBytes = new UTF8Encoding().GetBytes(jsonData);

        string addScoreUrl = apiUrl + "/add_score";

        // UnityWebRequest oluştur (POST metodu ile)
        using (UnityWebRequest request = new UnityWebRequest(addScoreUrl, "POST"))
        {
            // Gönderilecek veriyi (JSON byte'ları) ayarla
            request.uploadHandler = new UploadHandlerRaw(jsonToSendBytes);
            // Sunucudan gelecek yanıtı almak için download handler ayarla
            request.downloadHandler = new DownloadHandlerBuffer();

            // İsteğin başlıklarını ayarla (JSON gönderdiğimizi belirtiyoruz)
            request.SetRequestHeader("Content-Type", "application/json");
            // İsteğe bağlı: Başka gerekli başlıklar varsa burada eklenebilir
            // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN"); // Örneğin

            // İsteği gönder ve yanıtı bekle
            yield return request.SendWebRequest();

            // Sonucu kontrol et
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"API Gönderme Hatası ({addScoreUrl}): {request.error}");
                // Sunucudan gelen detaylı hata mesajını da loglayabiliriz (varsa)
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
            }
            else
            {
                Debug.Log("Skor başarıyla gönderildi!");
                // Sunucudan gelen başarılı yanıtı logla
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.Log("Sunucu Yanıtı: " + request.downloadHandler.text);
                }
                // Burada kullanıcıya başarılı olduğuna dair bir geri bildirim verebilirsin (opsiyonel)
            }
        } // using bloğu request'i otomatik olarak Dispose eder
    }


    // --- Diğer Menü Fonksiyonları ---

    public void Hakkinda()
    {
        hakkindaAcikMi = !hakkindaAcikMi; // Daha kısa toggle yöntemi
        hakkindaMenusu.SetActive(hakkindaAcikMi);
    }

    public void HataKapat()
    {
        IDHatasi.SetActive(false);
    }

    // --- Oyun Çıkış ---
    public void Cikis()
    {
        Application.Quit();
        Debug.Log("Oyundan Çıkıldı"); // Editörde test için
    }

     [System.Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public string username;
        public int score;
    }

    // JsonUtility için sarmalayıcı (JSON dizisini çözmek için)
    public static class JsonHelper
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
        StartCoroutine(FetchLeaderboardDataCoroutine());
    }

    IEnumerator FetchLeaderboardDataCoroutine()
    {
        // İstek başlamadan önce eski girdileri temizle ve durum mesajı göster
        ClearLeaderboardUI();
        if (leaderboardStatusText != null) leaderboardStatusText.text = "Yükleniyor...";

        string fullUrl = apiUrl + "/get_leaderboard";
        Debug.Log("Liderlik tablosu isteniyor: " + fullUrl);

        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"API Okuma Hatası ({fullUrl}): {request.error}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
                if (leaderboardStatusText != null) leaderboardStatusText.text = "Liderlik Tablosu yüklenemedi!\n(Hata: " + request.error + ")";
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Liderlik tablosu başarıyla alındı!");
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Sunucu Yanıtı (JSON): " + jsonResponse);

                // Durum mesajını temizle (başarılı olursa)
                if (leaderboardStatusText != null) leaderboardStatusText.text = "";

                // JSON yanıtını işle ve UI'ı güncelle
                ProcessLeaderboardJson(jsonResponse);
            }
        }
    }

    void ProcessLeaderboardJson(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Alınan JSON verisi boş.");
            if (leaderboardStatusText != null) leaderboardStatusText.text = "Liderlik tablosu boş veya alınamadı.";
            return;
        }

        try
        {
            // JsonHelper kullanarak JSON dizisini C# dizisine çevir
            LeaderboardEntry[] entries = JsonHelper.FromJson<LeaderboardEntry>(json);

            if (entries == null || entries.Length == 0)
            {
                 Debug.LogWarning("Liderlik tablosunda gösterilecek veri yok.");
                 if (leaderboardStatusText != null) leaderboardStatusText.text = "Henüz skor kaydedilmemiş.";
                 // ClearLeaderboardUI zaten çağrıldı, tekrar gerek yok.
                 return;
            }

            // Prefab kullanarak her satırı oluştur
            if (leaderboardEntryPrefab == null || leaderboardContentPanel == null)
            {
                Debug.LogError("Leaderboard Entry Prefab veya Content Panel atanmamış!");
                if (leaderboardStatusText != null) leaderboardStatusText.text = "UI Ayarları Eksik!";
                return;
            }

            // ClearLeaderboardUI zaten Coroutine başında çağrıldı.

            foreach (var entry in entries)
            {
                // Prefab'ı Content Panel'in altına oluştur (Instantiate)
                GameObject entryGO = Instantiate(leaderboardEntryPrefab, leaderboardContentPanel);

                // Prefab içindeki TextMeshPro bileşenlerini bul (İSİMLERİ KONTROL ET!)
                // Not: Find kullanımı yerine daha sağlam yöntemler (public değişkenler, GetComponentInChildren vb.) de tercih edilebilir.
                TextMeshProUGUI rankText = entryGO.transform.Find("Rank")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI nameText = entryGO.transform.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = entryGO.transform.Find("Puan")?.GetComponent<TextMeshProUGUI>();

                // Bulunan TextMeshPro bileşenlerinin içeriğini güncelle
                if (rankText != null)
                {
                    rankText.text = entry.rank.ToString() + ".";
                } else {
                    Debug.LogWarning($"Prefab '{leaderboardEntryPrefab.name}' içinde 'RankText' isimli TMP UGUI objesi bulunamadı.");
                }

                if (nameText != null)
                {
                    nameText.text = entry.username;
                } else {
                     Debug.LogWarning($"Prefab '{leaderboardEntryPrefab.name}' içinde 'NameText' isimli TMP UGUI objesi bulunamadı.");
                }

                if (scoreText != null)
                {
                    scoreText.text = entry.score.ToString();
                } else {
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

    void ClearLeaderboardUI()
    {
        if (leaderboardContentPanel == null) return;

        // Content paneli altındaki tüm çocukları yok et
        foreach (Transform child in leaderboardContentPanel)
        {
            Destroy(child.gameObject);
        }
        // Alternatif:
        // for (int i = leaderboardContentPanel.childCount - 1; i >= 0; i--)
        // {
        //     Destroy(leaderboardContentPanel.GetChild(i).gameObject);
        // }
    }

    // İsteğe bağlı: Oyun başladığında otomatik olarak yükle
    void Start()
    {
        // Başlangıçta durum mesajını ayarla (opsiyonel)
        //if (leaderboardStatusText != null) leaderboardStatusText.text = "Liderlik tablosunu yüklemek için butona basın.";

        // VEYA başlangıçta otomatik yüklemek için:
        RequestLeaderboardData();
    }

    void onTouchInput(){
        keyboard = new TouchScreenKeyboard.visible();
    }
}