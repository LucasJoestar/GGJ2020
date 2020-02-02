using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public static MyCamera I = null;

    private IEnumerator DoScreenShake()
    {
        Vector3 _pos = transform.position;

        float _timer = .25f;
        while (_timer > 0)
        {
            yield return new WaitForSeconds(.075f);
            _timer -= .075f;

            transform.position = _pos + (Vector3)(Random.insideUnitCircle.normalized * .02f);
        }

        transform.position = _pos;
    }

    public void StartScreenShake()
    {
        StartCoroutine(DoScreenShake());
    }


    public void SetAspect(float _width, float _height)
    {
        float _heightRatio = (_width / _height) / (16f / 9f);

        if (_heightRatio == 1)
        {
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }
        else if (_heightRatio < 1)
        {
            Camera.main.rect = new Rect(0, (1 - _heightRatio) / 2, 1, _heightRatio);
        }
        else
        {
            float _widthRatio = 1 / _heightRatio;
            Camera.main.rect = new Rect((1 - _widthRatio) / 2, 0, _widthRatio, 1);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (I)
        {
            Destroy(this);
            return;
        }

        I = this;
        SetAspect(Screen.width, Screen.height);
    }
}
