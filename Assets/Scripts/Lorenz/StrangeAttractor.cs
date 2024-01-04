using UnityEngine;

public class StrangeAttractor : MonoBehaviour
{
    public float sigma = 10.0f;
    public float rho = 28.0f;
    public float beta = 8.0f / 3.0f;

    public float  timeAddSigma = 0.00001f;
    public float amplitudeSigma = 2.0f;

    public float speed = 3.0f;
    public float scale = 0.05f;
    public float limit = 30.0f;

    private Vector3 position;
    public Vector3 off;

    private void Start()
    {
        position = transform.position;
    }

    float i = 0;

    private void Update()
    {
        i += timeAddSigma;

        float sigmaNew = Mathf.Sin(i) * amplitudeSigma + sigma;

        float xDot = sigmaNew * (position.y - position.x);
        float yDot = position.x * (rho - position.z) - position.y;
        float zDot = position.x * position.y - beta * position.z;

        float deltaTime = Time.deltaTime * speed;

        // Update the position based on the Lorenz equations
        position.x += xDot * deltaTime;
        position.y += yDot * deltaTime;
        position.z += zDot * deltaTime;

        // Limit position values
        position.x = Mathf.Clamp(position.x, -limit, limit);
        position.y = Mathf.Clamp(position.y, -limit, limit);
        position.z = Mathf.Clamp(position.z, -limit, limit);

        // Update the transform.position relative to the parent
        transform.position = (transform.parent.position + off) + position * scale;
    }
}
