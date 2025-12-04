using UnityEngine;

public class Volume : MonoBehaviour
{
    public void SetVolume(float v)
    {
        var src = FindFirstObjectByType<AudioSource>();
        if (src) src.volume = v;
    }
}
