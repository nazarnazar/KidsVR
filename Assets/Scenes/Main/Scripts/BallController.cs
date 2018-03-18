using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour 
{
    private new Renderer renderer;

    public Material inactiveMaterial;
    public Material gazedAtMaterial;

    [SerializeField] private float _liftHeight;
    [SerializeField] private float _liftSpeed;
    [SerializeField] private float _catchSpeed;
    [SerializeField] private RGB _color;

    private bool _selected = false;
    private bool _gazedAt = false;
    private Coroutine _liftRoutine = null;
    private Coroutine _movingRoutine = null;
    private float _ballXLimit = 40.0f;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        SetGazedAt(false);
    }

    public void SetGazedAt(bool gazedAt)
    {
        _gazedAt = gazedAt;
        if (_selected == false && inactiveMaterial != null && gazedAtMaterial != null)
        {
            renderer.material = gazedAt ? gazedAtMaterial : inactiveMaterial;
        }
    }

    public void Select()
    {
        _selected = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        _liftRoutine = StartCoroutine(Lift(BindToCamera));
    }


    public void Diselect()
    {
        _selected = false;
        if (_liftRoutine != null)
        {
            StopCoroutine(_liftRoutine);
            _liftRoutine = null;
        }
        if (_movingRoutine != null)
        {
            StopCoroutine(_movingRoutine);
            _movingRoutine = null;
        }
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        SetGazedAt(_gazedAt);
    }

    public RGB GetColor()
    {
        return _color;
    }

    private IEnumerator Lift(Action callback)
    {
        Vector3 startPosition = transform.position;
        Vector3 finishPosition = new Vector3(startPosition.x, _liftHeight, startPosition.z);
        float yDistance = Mathf.Abs(finishPosition.y - startPosition.y);
        float t = 0.0f;
        while (t < 1.0f)
        {
            transform.position = Vector3.Lerp(startPosition, finishPosition, t);
            t += Time.deltaTime * _liftSpeed / yDistance;
            yield return null;
        }
        _liftRoutine = null;
        callback();
    }

    private void BindToCamera()
    {
        _movingRoutine = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float perpendicularToCamera = transform.position.z;

        while (true)
        {
            float cameraRotation = (Camera.main.transform.eulerAngles.y / 180.0f) * Mathf.PI;
            float targetBallPosition = perpendicularToCamera * Mathf.Tan(cameraRotation);

            float positionDiff = Mathf.Abs(transform.position.x - targetBallPosition);

            if (positionDiff > 0.1f)
            {
                float increment = (_catchSpeed * Time.deltaTime > positionDiff) ? positionDiff : _catchSpeed * Time.deltaTime;
                if (targetBallPosition < transform.position.x)
                    increment *= -1;
                transform.position = new Vector3(Mathf.Clamp(transform.position.x + increment, -_ballXLimit, _ballXLimit),
                                                 transform.position.y,
                                                 transform.position.z);
            }
            yield return null;
        }
    }
}
