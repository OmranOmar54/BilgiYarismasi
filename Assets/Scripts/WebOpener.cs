using System.Runtime.InteropServices;
using UnityEngine;

public class WebOpener : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void YeniSekmedeAc(string url);
#endif

    public void LinkiAc(string url){
#if UNITY_WEBGL && !UNITY_EDITOR
        YeniSekmedeAc(url);
#else
        Application.OpenURL(url);
#endif
    }
}