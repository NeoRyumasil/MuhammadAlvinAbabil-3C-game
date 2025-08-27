using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    // Set Audio
    [SerializeField] private AudioSource _footstepSFX;
    [SerializeField] private AudioSource _glideSFX;
    [SerializeField] private AudioSource _landingSFX;
    [SerializeField] private AudioSource _punchSFX;

    // Play Footstep SFX
    private void PlayFootstepSfx()
    {
        // Volume range
        _footstepSFX.volume = Random.Range(0.7f, 1f);
        _footstepSFX.pitch = Random.Range(0.5f, 2.5f);

        // Play SFX
        _footstepSFX.Play();
    }

    // PLay and Stop Glide SFX
    public void PlayGlideSFX()
    {
        _glideSFX.Play();
    }

    public void StopGlideSFX()
    {
        _glideSFX.Stop();
    }

    // Play Landing SFX
    public void PlayLandingSFX()
    {
        _landingSFX.Play();
    }

    // Play Punch SFX
    public void PlayPunchSFX()
    {
        _punchSFX.Play();
    }
}
