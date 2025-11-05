using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    public float gravity = 9.81f;

    private void Update()
    {
        transform.position += Vector3.down * gravity * Time.deltaTime;
    }


}
