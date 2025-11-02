using UnityEngine;

public class Tricks : MonoBehaviour
{
    public PlayerController playerController;

    public Animator trickAnimations;
    public string triggerName = "kick";

    void Start()
    {
        trickAnimations = GetComponent<Animator>();
    }

    void Update()
    {
        trickAnimations.SetBool("grounded", playerController.grounded);
        trickAnimations.SetBool("isJumping", playerController.isJumping);
        trickAnimations.SetBool("manny", playerController.manny);
        trickAnimations.SetBool("kick", playerController.kick);
    }
}
