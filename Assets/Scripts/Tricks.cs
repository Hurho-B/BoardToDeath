using UnityEngine;

public class Tricks : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public Animator trickAnimations;

    void Start()
    {
        trickAnimations = GetComponent<Animator>();
    }

    void Update()
    {
        trickAnimations.SetBool("isGrounded", playerMovement.isGrounded);
        trickAnimations.SetBool("isJumping", playerMovement.isJumping);
    }
}
