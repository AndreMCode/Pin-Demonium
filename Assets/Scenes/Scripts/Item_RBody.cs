using System.Collections;
using UnityEngine;

public class Item_RBody : MonoBehaviour
{
    [SerializeField] Rigidbody body;
    private bool action;

    void Start()
    {
        action = false;
    }

    void FixedUpdate()
    {
        if (!action)
        {
            action = true;

            StartCoroutine(TimedDestroy());
        }
    }

    private IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(2);

        Destroy(this.gameObject);
    }
}
