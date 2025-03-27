using UnityEngine;
using static Weapon;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    public AudioSource ShootingChannel;

    public AudioClip P1911Shot;
    public AudioClip M4Shot;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }   
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch(weapon)
        {
            case WeaponModel.Pistol1911:
                ShootingChannel.PlayOneShot(P1911Shot);
                break;
            case WeaponModel.M4:
                ShootingChannel.PlayOneShot(M4Shot);
                break;
        }
    }
}
