using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    //Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 1f;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    //Spread
    public float spreadIntensity;

    //Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    //Loading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    //Weapon spawn
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public enum WeaponModel
    {
        Pistol1911,
        M4
    }

    public WeaponModel thisWeaponModel;


    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
     readyToShoot = true;
     burstBulletsLeft = bulletsPerBurst;
     animator = GetComponent<Animator>();

     bulletsLeft = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon)
        {
            GetComponent<Outline>().enabled = false;
            
            if(currentShootingMode  == ShootingMode.Auto)
            {
                //Holding down left mouse button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if(currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
            {
                //Clicking left mouse button
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
            {
                Reload();
            }

            //Automatically reload if magazine is empty
            if(readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
              Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if(AmmoManager.Instance.ammoDisplay != null)
            {
                AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
            }
        }
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        // SoundManager.Instance.shootingSound1911.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        //Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        //Point at shooting direction
        bullet.transform.forward = shootingDirection;

        //Shoot bullet
        bullet .GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        //Destroy bullet
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        
        //Checking if we are done shooting
        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        //Burst mode
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft --;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload(){
        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true; 
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
        RaycastHit hit; 

        Vector3 targetPoint;

        if(Physics.Raycast(ray, out hit))
        {
            //hitting something
            targetPoint = hit.point;
        }
        else
        {
            //shooting at air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //returning direction and spread
        return direction + new Vector3(x, y, 0);

    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
