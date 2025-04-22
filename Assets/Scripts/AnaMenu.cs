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

    public void OyunaBasla()
    {
        if(kullaniciAdiAlani.text.ToString() != "")
        {
            StartCoroutine(GetIP());

        }
        else
        {
            IDHatasi.SetActive(true);
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
    }

    IEnumerator Gonder(string kullaniciadi, string ip, float puan)
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
    }
}