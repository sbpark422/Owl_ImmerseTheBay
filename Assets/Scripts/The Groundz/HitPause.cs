using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    // Start is called before the first frame update
    [Range (0.0f,1.5f)]
    float duration = 1f;
    bool isFrozen;
    float _pendingFreezeDuration = 0f;

    // Update is called once per frame
    void Update()
    {
        if (_pendingFreezeDuration > 0f && !isFrozen)
        {
            StartCoroutine(DoFreeze());
        }
    }

    

    public void Freeze(float dur)
    {
        duration = dur;
        _pendingFreezeDuration = duration;
  
    }

    IEnumerator DoFreeze()
    {
        isFrozen = true;
        var orginalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = orginalTimeScale;
        _pendingFreezeDuration = 0f;
        isFrozen = false;


    }
}
