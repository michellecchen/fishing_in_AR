using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{

    public ParticleSystem splash;
    public ParticleSystem ripple;
    
    public ParticleSystem confetti;

    public void Splash() {
        StartCoroutine(PlaySplash());
    }

    public void Ripple() {
        StartCoroutine(PlayRipple());
    }

    IEnumerator PlaySplash() {
        splash.Play(true);
        yield return new WaitForSeconds(2);
        splash.Stop();
    }

    IEnumerator PlayRipple() {
        ripple.Play(true);
        yield return new WaitForSeconds(2);
        ripple.Stop();
    }

    public void Confetti() {
        StartCoroutine(PlayConfetti());
    }

    IEnumerator PlayConfetti() {
        confetti.Play(true);
        yield return new WaitForSeconds(1);
        confetti.Stop();
    }

}
