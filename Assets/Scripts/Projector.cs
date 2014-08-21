using UnityEngine;
using System.Collections;

public class Projector : MonoBehaviour 
{
    private GameObject projectionBase;
    public int projectionNodes = 0;
    private Vector3 velocity, position;
    private bool useGravity;
    private ProjectionBase projectionScript;
    bool enabled = false;

    void Start()
    {
        projectionBase = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("PreFabs/Gravity/ProjectionBase"));
        projectionScript = projectionBase.GetComponent<ProjectionBase>();
        projectionScript.Initialize(this.gameObject, projectionNodes);
    }

    void OnEnable()
    {
        Enable();
    }

    void OnDisable()
    {
        Disable();
    }

    public void Enable()
    {
        if (!enabled) PlayerScript.Show_Projection += ShowProjection;
        enabled = true;
    }

    public void Disable()
    {
        if (enabled) PlayerScript.Show_Projection -= ShowProjection;
        enabled = false;
    }

    void ShowProjection(bool truthiness)
    {
        position = transform.position;
        velocity = rigidbody.velocity;
        useGravity = rigidbody.useGravity;
        projectionBase.transform.position = this.transform.position;
        projectionBase.SetActive(truthiness);
        if (truthiness) StartCoroutine(ProjectionCycle());
    }

    IEnumerator ProjectionCycle()
    {
        while (projectionBase.activeSelf)
        {
            projectionScript.Project(position, velocity, useGravity);
            yield return null;
        }
    }

    public void SetPosition(Vector3 position)
    {
        this.position = position;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void SetUseGravity(bool truthiness)
    {
        this.useGravity = truthiness;
    }
}
