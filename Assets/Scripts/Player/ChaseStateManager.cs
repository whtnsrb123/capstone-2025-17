using UnityEngine;
using TMPro;

public class ChaseStateManager : MonoBehaviour
{
    public enum ChaseState { Undetected, Obstructed, Detected }
    
    private GameObject yellowExclamationText;
    private GameObject redExclamationText;
    
    void Start()
    {
        yellowExclamationText = transform.Find( "Canvas/Yellow Exclamation" ).gameObject;
        redExclamationText = transform.Find( "Canvas/Red Exclamation" ).gameObject;
        
        yellowExclamationText.SetActive( false );
        redExclamationText.SetActive( false );
    }

    public void SetChaseState( ChaseState state )
    {
        if(state == ChaseState.Undetected)
        {
            yellowExclamationText.SetActive( false );
            redExclamationText.SetActive( false );
        } else if(state == ChaseState.Obstructed)
        {
            yellowExclamationText.SetActive( true );
            redExclamationText.SetActive( false );
        }
        else
        {
            yellowExclamationText.SetActive( false );
            redExclamationText.SetActive( true );
        }
    }
}
