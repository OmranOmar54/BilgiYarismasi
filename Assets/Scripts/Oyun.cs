using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering;


public class Oyun : MonoBehaviour
{
    
    [Header("Oyun Zamanlayicisi")]
    public TextMeshProUGUI timerText; 
    public int remainingTime = 60;
    public bool isTimerActive = false;

    [Header("Baslangic Mesaji")]
    public TextMeshProUGUI baslangicSayaciText;
    private GameObject baslangicMesaji;

    [Header("Soru Mekanigi")]
    public TextMeshProUGUI soru;
    public TextMeshProUGUI a;
    public TextMeshProUGUI b;
    public TextMeshProUGUI c;
    public TextMeshProUGUI d;
    public Soru rastgeleSoru;
    public TextMeshProUGUI soruSayisiText;

    private int mevcutSoruSayisi = 1;

    [Header("Gecici Bilesenler")]
    public GameObject geciciUyari;
    public TextMeshProUGUI geciciYazi;

    void Start()
    {
        baslangicMesaji = GameObject.Find("GirisPaneli");
        if(baslangicMesaji == null){
            Debug.LogError("GirisPaneli Bulunamadı");
        }
        baslangicMesaji.SetActive(true);
        StartCoroutine(BaslangicSayaci());
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
        soruSayisiText.text = "Soru " + mevcutSoruSayisi;
        
    }

    IEnumerator BaslangicSayaci(){
        int kalanZaman = 5;
        while (kalanZaman > 0){
            baslangicSayaciText.text = kalanZaman.ToString();
            yield return new WaitForSeconds(1f);
            kalanZaman--;
        }
        baslangicSayaciText.text = "Başarılar!";
        
        yeniSoruOlustur();
        baslangicMesaji.SetActive(false);
        isTimerActive = true;
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
            mevcutSoruSayisi += 1;

        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            yeniSoruOlustur();
            mevcutSoruSayisi += 1;

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
            mevcutSoruSayisi += 1;
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            mevcutSoruSayisi += 1;
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
            mevcutSoruSayisi += 1;
            yeniSoruOlustur();
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            mevcutSoruSayisi += 1;
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
            mevcutSoruSayisi += 1;
            yeniSoruOlustur();
        }
        else
        {
            Debug.Log("Yanlış");
            geciciYazi.text = "Yanlış";
            geciciUyari.SetActive(true);
            mevcutSoruSayisi += 1;
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
        if(sorular.Count != 0){
            Soru selectedSoru = sorular[index];  // Soruyu seç
            sorular.RemoveAt(index);  // Seçilen soruyu listeden çıkar
            return selectedSoru;  // Seçilen soruyu geri döndür
        }

        else{
            Debug.LogError("Soru Kalmadı");
            return new Soru(0, "Soru Kalmadı", "", "", "", "", 'e');
        }
    }
}
