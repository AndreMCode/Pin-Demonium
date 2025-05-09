using UnityEngine;

public class SpinOnXAxis : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime * Vector3.right);
    }
}
