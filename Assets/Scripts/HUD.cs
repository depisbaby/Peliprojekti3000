using UnityEngine;
using System.Collections.Generic;


public class HUD : MonoBehaviour
{
    public static HUD Instance;
    private void Awake()
    {
        Instance = this;
    }

    private Queue<string> hudMessages = new Queue<string>();
    [SerializeField] private TMPro.TMP_Text hudMessageTMP;
    [SerializeField] private TMPro.TMP_Text timerTMP;
    float showRemaining;
    void Update()
    {
        if(showRemaining > 0.0f) //if hud message should be visible
        {
            showRemaining -= Time.deltaTime;
            hudMessageTMP.color = Color.Lerp(hudMessageTMP.color, Color.white, Time.deltaTime*4); //turn opaque
        }
        else//hud message should be invisible
        {
            hudMessageTMP.color = Color.Lerp(hudMessageTMP.color, new Color(0,0,0,0), Time.deltaTime*4);//turn transparent
            if(hudMessageTMP.color.a < 0.1f && hudMessages.Count > 0)
            {
                PickMessage(); // pick a message from queue
            }
        }

    }

    public void DisplayHUDMessage(string message)
    {
        hudMessages.Enqueue(message);
    }

    void PickMessage() //picks a message from queue and displays it
    {
        string message = hudMessages.Dequeue();
        showRemaining = 3.0f + message.Length * 0.05f;
        hudMessageTMP.text = message;
    }
}



