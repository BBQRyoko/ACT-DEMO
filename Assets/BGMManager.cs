using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BGMManager : MonoBehaviour
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void BGMPlay(AudioClip bgm) 
    {
        if (!audioSource.clip)
        {
            audioSource.clip = bgm;
            audioSource.Play();
            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0.2f, 0.75f).SetEase(Ease.InQuad);
        }
        else 
        {
            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0.05f, 0.5f).SetEase(Ease.OutQuad).onComplete =
            delegate ()
            {
                audioSource.clip = bgm;
                audioSource.Play();
                DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0.2f, 0.25f).SetEase(Ease.InQuad);
            };
        }
    }

    public void BGMStop() 
    {
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0f, 5f).SetEase(Ease.OutQuad).onComplete =
        delegate ()
        {
            audioSource.clip = null;
            audioSource.Stop();
        };
    }
}
