using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private string walkingSound = "walking";
    [SerializeField] private string throwSound = "throw";
    [SerializeField] private string jumpSound = "jump";
    public void FootStep()
    {
        soundPlayer.PlaySFX(walkingSound);
    }

    public void ThrowObject()
    {
        soundPlayer.PlaySFX(throwSound);
    }

    public void Jump()
    {
        soundPlayer.PlaySFX(jumpSound);
    }
}
