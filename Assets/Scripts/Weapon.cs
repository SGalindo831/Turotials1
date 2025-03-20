using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{

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
    private Animator animator;

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
    }

    // Update is called once per frame
    void Update()
    {
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

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

    }

    private void FireWeapon()
    {
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.shootingSound1911.Play();

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
