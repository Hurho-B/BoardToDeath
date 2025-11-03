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
        trickAnimations.SetBool("grounded", playerController.isOnGround);
        trickAnimations.SetBool("isJumping", playerController.doingJump);
        trickAnimations.SetBool("manny", playerController.doingManual);
        trickAnimations.SetBool("kick", playerController.doingKickflip);
    }
}
