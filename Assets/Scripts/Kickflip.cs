using UnityEngine;
using UnityEngine.InputSystem;

public class Kickflip : MonoBehaviour
{
    public InputActionAsset InputActions;

    public InputAction kickflipAction;

    public Animator trickAnimations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputActions = GetComponent<InputActionAsset>();
        trickAnimations = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
