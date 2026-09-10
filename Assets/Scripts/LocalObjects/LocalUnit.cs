using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class LocalUnit : MonoBehaviour
{
    public Vector2 gridPosition;

    public int globalId;
    public int ownerId;

    public Vector3 targetPosition;

    public MeshRenderer meshRenderer;
    public Material unitMaterial;

    public void Awake()
    {
        meshRenderer.material = new Material(unitMaterial); //create new material instance
    }

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*5);
    }
}
