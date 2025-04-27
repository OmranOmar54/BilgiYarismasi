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
    public TextMeshProUGUI kullaniciAdiAlani;
    string kullaniciAdi;
    string kullaniciIP;
    public GameObject IDHatasi;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContentPanel;
    public TextMeshProUGUI leaderboardStatusText;

    public GameObject klavye;

    private string apiUrl = "https://327eb718-351c-489b-9456-dab47851ab47-00-3st11zvtfmg0m.sisko.replit.dev";

    public void OyunaBasla()
    {
        Debug.Log("OyunaBasla çağrıldı.");
        kullaniciAdi = kullaniciAdiAlani.text;

        if (!string.IsNullOrEmpty(kullaniciAdi))
        {
            IDHatasi.SetActive(false);
            StartCoroutine(GetIPAndProceed());
        }
        else
        {
            IDHatasi.SetActive(true);
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

            int placeholderScore = 100; 
            StartCoroutine(SendScoreToServer(kullaniciAdi, kullaniciIP, placeholderScore));
        }
        else
        {
            Debug.LogError("IP Alınamadı: " + ipRequest.error);
            IDHatasi.SetActive(true);
        }
        ipRequest.Dispose();
    }

    [System.Serializable]
    private class ScoreData
    {
        public string username;
        public string ip;
        public int score;
    }

    IEnumerator SendScoreToServer(string username, string ip, int scoreValue)
    {
        ScoreData dataToSend = new ScoreData
        {
            username = username,
            ip = ip,
            score = scoreValue
        };

        string jsonData = JsonUtility.ToJson(dataToSend);
        byte[] jsonToSendBytes = new UTF8Encoding().GetBytes(jsonData);

        string addScoreUrl = apiUrl + "/add_score";

        using (UnityWebRequest request = new UnityWebRequest(addScoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSendBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"API Gönderme Hatası ({addScoreUrl}): {request.error}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Sunucu Hata Yanıtı: " + request.downloadHandler.text);
                }
            }
            else
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

    public void Hakkinda()
    {
        hakkindaAcikMi = !hakkindaAcikMi;
        hakkindaMenusu.SetActive(hakkindaAcikMi);
    }

    public void HataKapat()
    {
        IDHatasi.SetActive(false);
    }

     [System.Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public string username;
        public int score;
    }

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

                if (leaderboardStatusText != null) leaderboardStatusText.text = "";

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
                GameObject entryGO = Instantiate(leaderboardEntryPrefab, leaderboardContentPanel);

                TextMeshProUGUI rankText = entryGO.transform.Find("Rank")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI nameText = entryGO.transform.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = entryGO.transform.Find("Puan")?.GetComponent<TextMeshProUGUI>();

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

        foreach (Transform child in leaderboardContentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void Start()
    {
        RequestLeaderboardData();
    }

    public void KlavyeAcilipKapanmasi()
    {
        klavye.SetActive(true);
    }
    
}