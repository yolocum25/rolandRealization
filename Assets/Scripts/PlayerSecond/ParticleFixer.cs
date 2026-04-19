using UnityEngine;
public class FollowPlayer : MonoBehaviour 
{
    public Transform playerTransform;
    void Update() {
        if(playerTransform != null)
            transform.position = playerTransform.position;
    }
}