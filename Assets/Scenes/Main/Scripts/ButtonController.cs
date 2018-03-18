using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private float _pressedHeight;
    [SerializeField] private float _pressingSpeed;
    [SerializeField] private RGB _color;

    private bool _pressed = false;
    private GameController _gc;

    private void Start()
    {
        _gc = GameObject.FindGameObjectWithTag("GC").GetComponent<GameController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "Ball" && _pressed == false)
        {
            RGB ballColor = other.GetComponent<BallController>().GetColor();
            if (ballColor == _color)
            {
                _pressed = true;
                _gc.PlusOnePoint();
                StartCoroutine(Press());
            }
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
    }
}
