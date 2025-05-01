using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System;



public class AdminMenu : MonoBehaviour
{
    public string apiUrl;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContentPanel;
    public TextMeshProUGUI leaderboardStatusText;

    public bool otoGuncelleme;
    public static bool guncelleniyorMu;

    public TMP_InputField guncellemeSikligi;

    public GameObject ayarlar;

    public int veriyiGuncellemeAraligi;


    void Start()
    {
        apiUrl = AnaMenu.apiUrl;
        StartCoroutine(TabloyuCek());

    }

    public void CikisYap(){
        SceneManager.LoadScene("AnaMenu");
    }

    public void TestEt(){
        SceneManager.LoadScene("Oyun");
    }

    void Awake()
    {
        AnaMenu.adminMode = true;
    }

    void Update()
    {
        if(otoGuncelleme){
            if(!guncelleniyorMu){
                StartCoroutine(Bekle());
            }
        }
        
    }

    public IEnumerator Bekle()
    {
        Debug.Log("Bekleniyor");
        guncelleniyorMu = true;
        StartCoroutine(TabloyuCek());
        yield return new WaitForSeconds(veriyiGuncellemeAraligi);
        Debug.Log("Beklendi");
        guncelleniyorMu = false;
    }

    public IEnumerator TabloyuCek(){
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

                DatayiIsleme(jsonResponse);//gelen datayi isleme sureci
            }
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


    void DatayiIsleme(string json){
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

    public void GuncelleniyorMU(bool toggleValue){
        otoGuncelleme = toggleValue;
    }

    public void AyarlarIptal(){
        ayarlar.SetActive(false);
    }

    public void AyarlarAc(){
        ayarlar.SetActive(true);
    }

    public void Yenile(){
        StartCoroutine(TabloyuCek());
    }
}
