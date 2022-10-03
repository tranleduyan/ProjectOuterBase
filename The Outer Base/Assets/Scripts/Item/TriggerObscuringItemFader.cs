using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        //Get object we collided with, and then get all the obscuring item fader component on it and its childeren then trigger the fade out
        ObscuringItemFader[] obscuringItemFader = other.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if(obscuringItemFader.Length > 0)
        {
            for(int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeOut();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //get the gameobject we have collided with, and then get all the obscuring item fader components on it and its children - trigger the fade in
        ObscuringItemFader[] obscuringItemFader = other.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFader.Length > 0)
        {
            for (int i = 0; i < obscuringItemFader.Length; i++)
            {
                obscuringItemFader[i].FadeIn();
            }
        }
    }
}
