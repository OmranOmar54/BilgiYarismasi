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
    string kullaniciID;
    string kullaniciIP;
    public GameObject IDHatasi;

    public void OyunaBasla()
    {
        if(kullaniciAdiAlani.text.ToString() != "")
        {
            SceneManager.LoadScene("Oyun");
            StartCoroutine(GetIP());
            kullaniciID = kullaniciAdiAlani.text.ToString();
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
        UnityWebRequest istek = UnityWebRequest.Get("http://bilgiyarismasi.free.nf/get_ip.php?i=1");
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
}
