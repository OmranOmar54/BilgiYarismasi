using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;


public class Oyun : MonoBehaviour
{
    public TextMeshProUGUI timerText; 
    private int remainingTime = 60;
    public bool isTimerActive = false;

    public TextMeshProUGUI soru,a,b,c,d;

    public Soru rastgeleSoru;

    public GameObject geciciUyari;
    public TextMeshProUGUI geciciYazi;

    void Start()
    {
        yeniSoruOlustur();
    }

    void yeniSoruOlustur()
    {
        rastgeleSoru = SoruVeritabani.RastgeleSoruGetir();

        soru.text = rastgeleSoru.soruMetni;
        a.text = rastgeleSoru.soruASikki;
        b.text = rastgeleSoru.soruBSikki;
        c.text = rastgeleSoru.soruCSikki;
        d.text = rastgeleSoru.soruDSikki;
    }

    void Update()
    {
        if(isTimerActive)
        {
            StartCoroutine(StartCountdown());
            isTimerActive = false;
        }
        
    }

    IEnumerator StartCountdown()
    {
        while (remainingTime > 0)
        {
            timerText.text = remainingTime + "s";
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        isTimerActive = false;
        timerText.text = "Süre doldu!";
    }

    public void AnaMenuyeDon()
    {
        SceneManager.LoadScene("AnaMenu");
    }

    public void AyaBasti()
    {
        if(rastgeleSoru.dogruSik == 'A')
        {
            Debug.Log("Doğru");
            geciciYazi.text = "Doğru";
            geciciUyari.SetActive(true);       
            yeniSoruOlustur();
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
        }
    }
    public void ByeBasti()
    {
        if(rastgeleSoru.dogruSik == 'B')
        {
            Debug.Log("Doğru");
            geciciYazi.text = "Doğru";
            geciciUyari.SetActive(true);        
            yeniSoruOlustur();
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
        }
    }
    public void CyeBasti()
    {
        if(rastgeleSoru.dogruSik == 'C')
        {
            Debug.Log("Doğru");
            geciciYazi.text = "Doğru";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
        }
    }
    public void DyeBasti()
    {
        if(rastgeleSoru.dogruSik == 'D')
        {
            Debug.Log("Doğru");
            geciciYazi.text = "Doğru";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
        }
    }
    
    public void UyariKapandi()
    {
        geciciUyari.SetActive(false);
    }
}

public class Soru
{
    public int soruID;
    public string soruMetni;
    public string soruASikki;
    public string soruBSikki;
    public string soruCSikki;
    public string soruDSikki;
    public char dogruSik;
    public Soru(int id, string soru, string a, string b, string c, string d, char dogru)
    {
        soruID = id;
        soruMetni = soru;
        soruASikki = a;
        soruBSikki = b;
        soruCSikki = c;
        soruDSikki = d;
        dogruSik = dogru;
    }
}
public static class SoruVeritabani
{
    private static List<Soru> sorular = new List<Soru>()
    {
        new Soru(1, "Türkiye'nin başkenti neresidir?", "İstanbul", "Ankara", "İzmir", "Bursa", 'B'),
        new Soru(2, "Dünya'nın en büyük okyanusu hangisidir?", "Hint", "Atlas", "Pasifik", "Arktik", 'C'),
        new Soru(3, "2 + 2 kaç eder?", "3", "4", "5", "6", 'B'),
    };
    public static Soru RastgeleSoruGetir()
    {
        int index = Random.Range(0, sorular.Count);
        return sorular[index];
    }
}
