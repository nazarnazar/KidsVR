using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour 
{
    private Vector3 startingPosition;
    private new Renderer renderer;

    public Material inactiveMaterial;
    public Material gazedAtMaterial;

    [SerializeField] private float _liftHeight;
    [SerializeField] private float _liftSpeed;
    [SerializeField] private float _catchSpeed;

    private bool _selected = false;
    private bool _gazedAt = false;
    private Coroutine _liftRoutine = null;
    private Coroutine _movingRoutine = null;

    void Start()
    {
        startingPosition = transform.localPosition;
        renderer = GetComponent<Renderer>();
        SetGazedAt(false);
    }

    public void SetGazedAt(bool gazedAt)
    {
        Debug.Log("SphereController: SetGazedAt");
        _gazedAt = gazedAt;
        if (_selected == false && inactiveMaterial != null && gazedAtMaterial != null)
        {
            renderer.material = gazedAt ? gazedAtMaterial : inactiveMaterial;
            return;
        }
    }

    public void Select()
    {
        Debug.Log("Select");
        _selected = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        _liftRoutine = StartCoroutine(Lift(BindToCamera));
    }


    public void Diselect()
    {
        Debug.Log("Diselect");
        if (_liftRoutine != null)
            StopCoroutine(_liftRoutine);
        if (_movingRoutine != null)
            StopCoroutine(_movingRoutine);
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && _selected == true)
        {
            _selected = false;
            SetGazedAt(_gazedAt); 
        }
    }

    private IEnumerator Lift(Action callback)
    {
        Vector3 startPosition = transform.position;
        Vector3 finishPosition = new Vector3(startPosition.x, _liftHeight, startPosition.z);
        float t = 0.0f;
        while (t < 1.0f)
        {
            transform.position = Vector3.Lerp(startPosition, finishPosition, t);
            t += Time.deltaTime * _liftSpeed;
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
                transform.position = new Vector3(transform.position.x + increment,
                                                 transform.position.y,
                                                 transform.position.z);
            }
            yield return null;
        }
    }
}
