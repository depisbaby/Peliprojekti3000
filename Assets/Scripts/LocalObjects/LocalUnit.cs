using UnityEngine;

public class LocalUnit : MonoBehaviour
{
    public Vector2 gridPosition;

    public int globalId;
    public int ownerId;

    public Vector3 targetPosition;

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*5);
    }
}
