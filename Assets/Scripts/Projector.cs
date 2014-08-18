using UnityEngine;
using System.Collections;

public class Projector : MonoBehaviour 
{
    public GameObject projectionBase;
    public int projectionNodes = 0;
    private Vector3 velocity, position;
    private bool useGravity;
    private ProjectionBase projectionScript;

    void Start()
    {
        projectionBase = (GameObject)GameObject.Instantiate(projectionBase);
        projectionScript = projectionBase.GetComponent<ProjectionBase>();
        projectionScript.Initialize(this.gameObject, projectionNodes);
    }

    void OnEnable()
    {
        PlayerScript.Show_Projection += ShowProjection;
    }

    void OnDisable()
    {
        PlayerScript.Show_Projection -= ShowProjection;
    }

    void ShowProjection(bool truthiness)
    {
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
        transform.position = position;
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
