using System.Collections.Generic;
using UnityEngine;

namespace Sorular.Soru
{
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
}