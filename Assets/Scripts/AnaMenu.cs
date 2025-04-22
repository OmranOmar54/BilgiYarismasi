using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AnaMenu : MonoBehaviour
{
    public GameObject hakkindaMenusu;
    private bool hakkindaAcikMi = false;

    public  TMP_InputField kullaniciAdiAlani;
    string kullaniciAdi;
    string kullaniciIP;
    public GameObject IDHatasi;

    public TextMeshProUGUI _cache;

    private string apiUrl ="https://327eb718-351c-489b-9456-dab47851ab47-00-3st11zvtfmg0m.sisko.replit.dev/"

    public void OyunaBasla()
    {
        kullaniciAdi = kullaniciAdiAlani.text;

        if (!string.IsNullOrEmpty(kullaniciAdi)) // Boş veya null kontrolü
        {
            IDHatasi.SetActive(false); // Hata mesajını gizle (varsa)
            StartCoroutine(GetIPAndProceed()); // IP al ve devam et
        }
        else
        {
            IDHatasi.SetActive(true); // Kullanıcı adı boşsa hata göster
        }

    }

    public void Hakkinda()
    {
        if(hakkindaAcikMi == false)
        {
            hakkindaMenusu.SetActive(true);
            hakkindaAcikMi = true;
        }
        else
        {
            hakkindaMenusu.SetActive(false);
            hakkindaAcikMi = false;
        }
    }

    public void HataKapat()
    {
        IDHatasi.SetActive(false);
    }

    IEnumerator GetIP()
    {   
        UnityWebRequest istek = UnityWebRequest.Get("https://api.ipify.org");
        yield return istek.SendWebRequest();

        if(istek.result == UnityWebRequest.Result.Success)
        {
            string ip = istek.downloadHandler.text;
            Debug.Log("Kullanıcının IP Adresi: "+ ip);
            kullaniciIP = ip;
            kullaniciAdi = kullaniciAdiAlani.text.ToString();
            StartCoroutine(Gonder(kullaniciAdi, kullaniciIP, 100f));
            SceneManager.LoadScene("Oyun");

        }
        else
        {
            Debug.LogError("IP Alınamadı: " + istek.error);
        }
        istek.Dispose();
    }

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


    /*IEnumerator Gonder(string kullaniciadi, string ip, float puan)
    {
        WWWForm form = new WWWForm();
        form.AddField("kullaniciAdi", kullaniciadi);
        form.AddField("ip", ip);
        form.AddField("puan", puan.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://73410697-51b4-486e-950f-2066c2e7d5a6-00-3jr6dh4q26quw.sisko.replit.dev", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Hata: " + www.error);
        }
        else
        {
            Debug.Log("Sunucu cevabı: " + www.downloadHandler.text);
        }
    }*/
}