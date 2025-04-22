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
            kullaniciAdi = kullaniciAdiAlani.text.ToString();
            SceneManager.LoadScene("Oyun");
            StartCoroutine(Gonder(kullaniciAdi, kullaniciIP, 100));

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
        UnityWebRequest istek = UnityWebRequest.Get("https://56a30e7c-bab9-4575-8b12-cce221eb12bd-00-2q7avn8nejj9u.pike.replit.dev/");
        yield return istek.SendWebRequest();

        if(istek.result == UnityWebRequest.Result.Success)
        {
            string ip = istek.downloadHandler.text;
            Debug.Log("Kullanıcının IP Adresi: "+ ip);
            kullaniciIP = ip;
        }
        else
        {
            Debug.LogError("IP Alınamadı: " + istek.error);
        }
    }

    IEnumerator Gonder(string kullaniciAdi, string ip, float puan)
    {
        WWWForm form = new WWWForm();
        form.AddField("kullaniciAdi", kullaniciAdi);
        form.AddField("ip", ip);
        form.AddField("puan", puan.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://seninsiten.infinityfreeapp.com/insert.php", form);
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