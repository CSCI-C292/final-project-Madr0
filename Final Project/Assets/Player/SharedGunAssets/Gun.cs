using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] public float _damage;
    [SerializeField] public float _range;
    [SerializeField] public float _fireRate;

    [SerializeField] public float _kickBack;
    [SerializeField] public float _kickUpAngle;
    [SerializeField] public GameObject _player;
    [SerializeField] public GameObject _gunModel;
    [SerializeField] public Camera _camera;
    [SerializeField] public ParticleSystem _muzzelFlash;
    [SerializeField] public GameObject _impactEffectMetal;
    [SerializeField] public GameObject _impactEffectBlood;
    [SerializeField] public AudioSource _soundManager;
    [SerializeField] public AudioClip _gunSound;

    protected float nextTimeToFire = 0f;
    protected Vector3 smoothDownVelocity;
    protected float angleSmoothDownVelocity;
    protected float recoilAngle;
    protected Vector3 gunInitialPos;

    void Start() {
        gunInitialPos = _gunModel.transform.localPosition;
    }
    protected void ShootRaycast() {
        _muzzelFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, _range)) {
            GameObject impactGO;
            impactGO = Instantiate(_impactEffectMetal, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO,2.0f);
        }
    }

    protected void fireSemi() {
        if(Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1f/_fireRate;
            ShootRaycast();
            _soundManager.PlayOneShot(_gunSound);
            _gunModel.transform.localPosition -= Vector3.forward*_kickBack;
            recoilAngle -= _kickUpAngle;
        }
        calcRecoil(3f);
    }

    protected void fireAutomatic() {
        if(Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1f/_fireRate;
            ShootRaycast();
            _soundManager.PlayOneShot(_gunSound);
            _gunModel.transform.localPosition -= Vector3.forward*_kickBack;
            recoilAngle -= _kickUpAngle;
        }
        calcRecoil(1.5f);
    }

    protected void calcRecoil(float mult) {
        
        _gunModel.transform.localPosition = Vector3.SmoothDamp(_gunModel.transform.localPosition, gunInitialPos, ref smoothDownVelocity, 1f/(_fireRate*mult));
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref angleSmoothDownVelocity, 1f/(_fireRate*mult));
        _gunModel.transform.localEulerAngles = new Vector3(recoilAngle,-6,0);
    }
}
