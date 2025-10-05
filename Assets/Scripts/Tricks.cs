using UnityEngine;

public class Tricks : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public Animator trickAnimations;
    public string triggerName = "kick";

    void Start()
    {
        trickAnimations = GetComponent<Animator>();
    }

    void Update()
    {
        trickAnimations.SetBool("grounded", playerMovement.grounded);
        trickAnimations.SetBool("isJumping", playerMovement.isJumping);
        trickAnimations.SetBool("manny", playerMovement.manny);
        trickAnimations.SetBool("kick", playerMovement.kick);
    }
}
