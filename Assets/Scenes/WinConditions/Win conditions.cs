using System;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private Vector3 targetOrientation;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private int targetSceneIndex;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LoadNewScene(targetPosition,targetOrientation,targetSceneIndex);
        }
    }
}