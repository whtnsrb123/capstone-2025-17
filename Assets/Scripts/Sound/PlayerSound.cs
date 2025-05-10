using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private string walkingSound;
    [SerializeField] private string throwSound;
    public void FootStep()
    {
        soundPlayer.PlaySFX(walkingSound);
    }
    public void ThrowObject()
    {
        soundPlayer.PlaySFX(throwSound);
    }
}
