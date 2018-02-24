using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private float _pressedHeight;
    [SerializeField] private float _pressingSpeed;

    private bool _pressed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball" && _pressed == false)
        {
            _pressed = true;
            StartCoroutine(Press());    
        }
    }

    private IEnumerator Press()
    {
        float startHeight = transform.position.y;
        float t = 0.0f;
        while (t < 1.0f)
        {
            transform.position = new Vector3(transform.position.x,
                                             Mathf.Lerp(startHeight, _pressedHeight, t),
                                             transform.position.z);
            t += Time.deltaTime * _pressingSpeed;
            yield return null;
        }

        Debug.Log("CONGRATULATIONS!!!");
    }
}
