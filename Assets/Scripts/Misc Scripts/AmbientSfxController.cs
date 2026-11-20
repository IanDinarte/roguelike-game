using UnityEngine;

public class AmbientSfxController : MonoBehaviour
{
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] sounds;

    [SerializeField] private float soundCooldown;
    [SerializeField] private float musicCooldown;

    private void Start()
    {
        SoundCooldown();
        MusicCooldown();
    }

    private async void SoundCooldown()
    {
        soundCooldown = Random.Range(120, 361);

        await Awaitable.WaitForSecondsAsync(soundCooldown);

        int soundIndex = Random.Range(0, sounds.Length);

        PlayAudio(soundIndex);
    }

    private async void PlayAudio(int soundIndex)
    {
        ambientSource.clip = sounds[soundIndex];
        ambientSource.Play();

        while (ambientSource.isPlaying)
        {
            await Awaitable.NextFrameAsync();
        }

        SoundCooldown();
    }

    private async void PlayMusic()
    {
        musicSource.Play();

        while (musicSource.isPlaying)
        {
            await Awaitable.NextFrameAsync();
        }

        MusicCooldown();
    }

    private async void MusicCooldown()
    {
        if (musicSource.clip != null)
        {
            musicCooldown = Random.Range(300, 601);

            await Awaitable.WaitForSecondsAsync(musicCooldown);

            PlayMusic();
        }
    }
}