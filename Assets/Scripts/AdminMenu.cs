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
using System.Linq;
using UnityEditor.SearchService;
using JetBrains.Annotations;
using System.Collections.Generic;



public class AdminMenu : MonoBehaviour
{
    public string apiUrl;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContentPanel;
    public TextMeshProUGUI leaderboardStatusText;

    //kapanıp açılacaklar
    public GameObject[] kapanacaklar;

    //açılacaklar
    public GameObject[] acilacaklar;

    public bool otoGuncelleme;
    public static bool guncelleniyorMu;

    public TMP_InputField guncellemeSikligi;

    public Transform fullscreenContent;
    public TextMeshProUGUI fullscreenStatusText;

    public GameObject ayarlar;

    public int veriyiGuncellemeAraligi;


    void Start()
    {
        apiUrl = AnaMenu.apiUrl;
        StartCoroutine(TabloyuCek(leaderboardStatusText,leaderboardContentPanel));
        StartCoroutine(TabloyuCek(fullscreenStatusText, fullscreenContent));

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

        // Bu çağrıyı beklemeden geçme!
        yield return StartCoroutine(TabloyuCek(leaderboardStatusText, leaderboardContentPanel));

        yield return new WaitForSeconds(veriyiGuncellemeAraligi);
        Debug.Log("Beklendi");

        guncelleniyorMu = false;
    }

    public IEnumerator TabloyuCek(TextMeshProUGUI statusText, Transform contentPanel)
    {
        if (statusText != null) statusText.text = "Yükleniyor...";

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
                if (statusText != null) statusText.text = "Liderlik Tablosu yüklenemedi!\n(Hata: " + request.error + ")";
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Liderlik tablosu başarıyla alındı!");
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Sunucu Yanıtı (JSON): " + jsonResponse);

                if (statusText != null) statusText.text = "";

                // === Veriyi İşleme ===
                LeaderboardEntry[] entries = JsonHelper.FromJson<LeaderboardEntry>(jsonResponse);

                if (entries == null || entries.Length == 0)
                {
                    if (statusText != null) statusText.text = "Henüz skor kaydedilmemiş.";
                    yield break;
                }

                // Mevcut prefabları haritaya al
                Dictionary<string, GameObject> existingEntries = new Dictionary<string, GameObject>();
                foreach (Transform child in contentPanel)
                {
                    TextMeshProUGUI nameText = child.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                    if (nameText != null)
                    {
                        existingEntries[nameText.text] = child.gameObject;
                    }
                }

                // Güncellenen kullanıcıları tut
                HashSet<string> updatedUsers = new HashSet<string>();

                // Her kullanıcı için prefab oluştur veya güncelle
                foreach (var entry in entries)
                {
                    GameObject entryGO;

                    if (existingEntries.TryGetValue(entry.username, out entryGO))
                    {
                        // Zaten var, sadece içeriği güncelle
                    }
                    else
                    {
                        // Yeni prefab oluştur
                        entryGO = Instantiate(leaderboardEntryPrefab, contentPanel);
                    }

                    // Ortak güncelleme işlemleri
                    entryGO.name = entry.username;

                    TextMeshProUGUI rankText = entryGO.transform.Find("Rank")?.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI nameText = entryGO.transform.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI scoreText = entryGO.transform.Find("Puan")?.GetComponent<TextMeshProUGUI>();

                    if (rankText != null) rankText.text = entry.rank.ToString() + ".";
                    if (nameText != null) nameText.text = entry.username;
                    if (scoreText != null) scoreText.text = entry.score.ToString();

                    updatedUsers.Add(entry.username);
                }

                // Artık olmayan kullanıcıların prefablarını sil
                foreach (Transform child in contentPanel)
                {
                    TextMeshProUGUI nameText = child.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                    if (nameText != null && !updatedUsers.Contains(nameText.text))
                    {
                        Destroy(child.gameObject);
                    }
                }

                // Rank'a göre sırala
                List<Transform> sortedChildren = new List<Transform>();
                foreach (Transform child in contentPanel)
                {
                    sortedChildren.Add(child);
                }

                sortedChildren.Sort((a, b) =>
                {
                    var aRank = int.Parse(a.Find("Rank")?.GetComponent<TextMeshProUGUI>().text.Replace(".", "") ?? "9999");
                    var bRank = int.Parse(b.Find("Rank")?.GetComponent<TextMeshProUGUI>().text.Replace(".", "") ?? "9999");
                    return aRank.CompareTo(bRank);
                });

                for (int i = 0; i < sortedChildren.Count; i++)
                {
                    sortedChildren[i].SetSiblingIndex(i);
                }
            }
        }
    }

    void ClearLeaderboardUI(Transform contentPanel)//liderlik tablosu temizleme
    {
        if (contentPanel == null) return;

        foreach (Transform child in contentPanel)
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


    void DatayiIsleme(string json, TextMeshProUGUI statusText, Transform contentPanel)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Alınan JSON verisi boş.");
            if (statusText != null) statusText.text = "Liderlik tablosu boş veya alınamadı.";
            return;
        }

        try
        {
            LeaderboardEntry[] entries = JsonHelper.FromJson<LeaderboardEntry>(json);

            if (entries == null || entries.Length == 0)
            {
                if (statusText != null) statusText.text = "Henüz skor kaydedilmemiş.";
                return;
            }

            if (leaderboardEntryPrefab == null || contentPanel == null)
            {
                Debug.LogError("Leaderboard Entry Prefab veya Content Panel atanmamış!");
                if (statusText != null) statusText.text = "UI Ayarları Eksik!";
                return;
            }

            Dictionary<string, GameObject> existingEntries = new Dictionary<string, GameObject>();

            // Mevcut prefabları topla (username -> GameObject)
            foreach (Transform child in contentPanel)
            {
                TextMeshProUGUI nameText = child.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    existingEntries[nameText.text] = child.gameObject;
                }
            }

            // Ekranda kalacak olan prefabları takip et
            HashSet<string> updatedUsers = new HashSet<string>();

            foreach (var entry in entries)
            {
                GameObject entryGO;

                if (existingEntries.TryGetValue(entry.username, out entryGO))
                {
                    // Güncelle
                }
                else
                {
                    // Yeni oluştur
                    entryGO = Instantiate(leaderboardEntryPrefab, contentPanel);
                }

                // Güncelleme işlemi
                entryGO.name = entry.username; // debug kolaylığı
                TextMeshProUGUI rankText = entryGO.transform.Find("Rank")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI nameText = entryGO.transform.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = entryGO.transform.Find("Puan")?.GetComponent<TextMeshProUGUI>();

                if (rankText != null) rankText.text = entry.rank.ToString() + ".";
                if (nameText != null) nameText.text = entry.username;
                if (scoreText != null) scoreText.text = entry.score.ToString();

                updatedUsers.Add(entry.username);
            }

            // Silinmesi gereken eski prefablar
            foreach (Transform child in contentPanel)
            {
                TextMeshProUGUI nameText = child.Find("KullaniciAdi")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null && !updatedUsers.Contains(nameText.text))
                {
                    Destroy(child.gameObject);
                }
            }

            // Yeniden sıralama (rank'a göre)
            List<Transform> sortedChildren = new List<Transform>();
            foreach (Transform child in contentPanel)
            {
                sortedChildren.Add(child);
            }

            sortedChildren.Sort((a, b) => {
                var aRank = int.Parse(a.Find("Rank")?.GetComponent<TextMeshProUGUI>().text.Replace(".", "") ?? "9999");
                var bRank = int.Parse(b.Find("Rank")?.GetComponent<TextMeshProUGUI>().text.Replace(".", "") ?? "9999");
                return aRank.CompareTo(bRank);
            });

            for (int i = 0; i < sortedChildren.Count; i++)
            {
                sortedChildren[i].SetSiblingIndex(i);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON işlenirken hata oluştu: {e.Message}");
            if (statusText != null) statusText.text = "Veri işlenirken hata oluştu.";
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
        StartCoroutine(TabloyuCek(leaderboardStatusText, leaderboardContentPanel));
    }
    public void FullscreenYenile(){
        StartCoroutine(TabloyuCek(fullscreenStatusText, fullscreenContent));

    }

    public void TamEkran(){
        foreach (GameObject _kapanacak in kapanacaklar){
            if (_kapanacak != null){
                _kapanacak.SetActive(false);
            }
        }
        foreach (GameObject _acilacak in acilacaklar){
            if(_acilacak != null){
                _acilacak.SetActive(true);
            }
        }
    }
    public void ExitFullscreen(){
        foreach (GameObject _kapanacak in kapanacaklar){
            if (_kapanacak != null){
                _kapanacak.SetActive(true);
            }
        }
        foreach (GameObject _acilacak in acilacaklar){
            if(_acilacak != null){
                _acilacak.SetActive(false);
            }
        }
    }
}
