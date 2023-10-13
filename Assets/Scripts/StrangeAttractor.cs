using UnityEngine;

public class StrangeAttractor : MonoBehaviour
{
    public float a = 10.0f;
    public float b = 28.0f;
    public float c = 8.0f / 3.0f;

    public float  timeAddA=0.00001f;
    public float amplitudeA = 2.0f;
    public float timeAddB = 0.00001f;
    public float amplitudeB = 2.0f;
    public float timeAddC = 0.00001f;
    public float amplitudeC = 2.0f;
    public float speed = 1.0f;


    float i = 0;
    float aNew = 0.0f;
    float ib = 0;
    float bNew = 0.0f; 
    float ic = 0;
    float cNew = 0.0f;


    private Vector3 position;

    private void Start()
    {
        position = new Vector3(1, 1, 1); // Initial position of the object

    }

    private void Update()
    {
        i+=timeAddA;
        ib+=timeAddB;
        ic+=timeAddC;

        aNew = Mathf.Sin(i) * amplitudeA+a;
        bNew = Mathf.Sin(ib) * amplitudeB + b;
        cNew = Mathf.Sin(ic) * amplitudeC + c;

        float deltaTime = Time.deltaTime;

        float xDot = aNew * (position.y - position.x);
        float yDot = (position.x * (b - position.z)) - position.y;
        float zDot = (position.x * position.y) - (c * position.z);

        // Check for NaN values and handle them
        if (float.IsNaN(xDot) || float.IsNaN(yDot) || float.IsNaN(zDot) || position.x > 10000 || position.x < -10000 || position.y > 10000 || position.y < -10000 || position.z > 10000 || position.z < -10000)
        {
            // Handle NaN values by resetting the position.
            position = new Vector3(1, 1, 1); // Reset to the initial position.
        }
        else
        {
            // Check for Infinity values as well (optional).
            if (float.IsInfinity(xDot) || float.IsInfinity(yDot) || float.IsInfinity(zDot))
            {
                // Handle Infinity values if needed.
                // For example, you can clamp them to prevent runaway values.
                xDot = Mathf.Clamp(xDot, -float.MaxValue, float.MaxValue);
                yDot = Mathf.Clamp(yDot, -float.MaxValue, float.MaxValue);
                zDot = Mathf.Clamp(zDot, -float.MaxValue, float.MaxValue);
            }

            position.x += xDot * deltaTime*speed;
            position.y += yDot * deltaTime * speed;
            position.z += zDot * deltaTime * speed;

            // Update the transform.position only if no NaN or Infinity values are encountered.
            transform.position = position;
        }
    }
}
