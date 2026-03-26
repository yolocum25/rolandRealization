using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
   public static EventManager Instance {get; private set;}

   public event Action <bool> OnFirstTimeSeen;
   
   
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
      }

   }

   public void IntroScene(bool skip)
   {
      OnFirstTimeSeen?.Invoke(skip);
   }
   
   
   
   
   
   
   
   
   
}
