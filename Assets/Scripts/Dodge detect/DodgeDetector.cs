using UnityEngine;

public class DodgeDetector : MonoBehaviour
{
    [Header("Feedback Settings")]
    [Tooltip("Drag your scene's AudioSource here.")]
    public AudioSource dodgeAudioSource;

    [Tooltip("The specific subtle sound clip to play.")]
    public AudioClip dodgeSoundClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if the object passing through the line is an obstacle
        if (collision.CompareTag("Obstacle"))
        {
            // 2. Play the satisfying dodge sound
            if (dodgeAudioSource != null && dodgeSoundClip != null)
            {
                dodgeAudioSource.PlayOneShot(dodgeSoundClip);
            }
            else
            {
                Debug.LogWarning("DodgeDetector: Missing AudioSource or Audio Clip!");
            }

            // Note: If you want to add +1 to the score or fill the Surge Meter 
            // for every dodge later, you can easily call that function right here!
        }
    }
}