using System.Collections;
using System.Text; // JSON için Encoding
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking; // UnityWebRequest için gerekli

public class AnaMenu : MonoBehaviour
{
    public GameObject hakkindaMenusu;
    private bool hakkindaAcikMi = false;

    public TMP_InputField kullaniciAdiAlani;
    string kullaniciAdi;
    string kullaniciIP;
    public GameObject IDHatasi;

    public TextMeshProUGUI _cache; // Bu değişkenin ne için kullanıldığına dair yorum eklemek faydalı olabilir

    // Replit API'nizin tam URL'si (endpoint dahil)
    // Senin Gonder metodundaki URL'yi baz aldım ve /add_score ekledim.
    // Eğer Python scriptindeki endpoint farklıysa burayı güncellemelisin.
    private string apiUrl = "https://73410697-51b4-486e-950f-2066c2e7d5a6-00-3jr6dh4q26quw.sisko.replit.dev/add_score";

    // --- Oyuna Başlama ve Kullanıcı Bilgisi Alma ---

    public void OyunaBasla()
    {
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
        UnityWebRequest ipRequest = UnityWebRequest.Get("https://api.ipify.org");
        yield return ipRequest.SendWebRequest();

        if (ipRequest.result == UnityWebRequest.Result.Success)
        {
            kullaniciIP = ipRequest.downloadHandler.text;
            Debug.Log("Kullanıcının IP Adresi: " + kullaniciIP);

            int placeholderScore = 100; // Şimdilik sabit bir skor
            StartCoroutine(SendScoreToServer(kullaniciAdi, kullaniciIP, placeholderScore));
            // --- Bitiş ÖNEMLİ NOT ---

            // Skor gönderildikten sonra (veya gönderim başlatıldıktan sonra) diğer sahneye geç
            SceneManager.LoadScene("Oyun");
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

        // UnityWebRequest oluştur (POST metodu ile)
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
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
                Debug.LogError($"API Gönderme Hatası ({apiUrl}): {request.error}");
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
}