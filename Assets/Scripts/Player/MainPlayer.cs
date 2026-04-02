using System;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    [field: SerializeField] public Transform InteractPoint { get; private set; }
    [field: SerializeField] public float InteractionRadius { get; private set; }
    
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }
    //Cuando un jugador nace..
    // private void Start()
    // {
    //     
    //     transform.position = GameManager.Instance.SavedPosition;
    //     transform.eulerAngles = GameManager.Instance.SavedOrientation;
    // }
}