using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningPlatform : MonoBehaviour
{
    [Header("Straight Movement")]
    Vector3 startingPosition;
    public Vector2 movementVector;
    [Range(0, 1)] float movementFactor;
    [SerializeField] float period = 2f;
    bool inScene;


    bool stopAtEnd;



    private void Start()
    {
        startingPosition = transform.position;
        inScene = true;
    }
    private void Update()
    {

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = (rawSinWave + 1f) / 2;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;



    }
    private void OnDrawGizmos()
    {
        if (inScene)
        {
            InGameGizmo();
            return;
        }

        OutOfSceneGizmo();
    }
    private void OutOfSceneGizmo()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(movementVector.x, movementVector.y, 0));
    }
    private void InGameGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startingPosition, startingPosition + new Vector3(movementVector.x, movementVector.y, 0));
    }
}
