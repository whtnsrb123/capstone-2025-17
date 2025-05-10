using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private string walkingSound;
    public void FootStep()
    {
        soundPlayer.PlaySFX(walkingSound);
    }
}
