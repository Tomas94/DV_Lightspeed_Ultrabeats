using UnityEngine;
using System.Collections;

public class Player : Entity
{
    [SerializeField] GameObject _gameOverScreen;
    public Animator shield;

    public InGameUI_Controller gameUI;
    PU_Shield _shieldPU = new PU_Shield(5);

    [SerializeField] float _fireRate = 0.3f;
    [SerializeField] float _maxLife;
    public bool _isShielded;
    public bool _charging;

    [SerializeField] AudioClip _playerDisparoAC;
    [SerializeField] AudioClip _playerShieldAC;
    [SerializeField] AudioClip _playerShieldDEAC;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource _audioSourceShield;

    public float MaxLife { get { return _maxLife; } }

    private void Awake()
    {
        GetComponentInChildren<MeshRenderer>().material = GameManager.Instance.playerskin;
        currentLife = _maxLife;
    }

    private void Start()
    {
        StartCoroutine(ChargeShot(_fireRate));
        StartCoroutine(RechargeShield());
    }

    void Update()
    {
        _isShielded = _shieldPU._isActive;
        if (_isShielded == false && gameUI.shieldFillCircle.fillAmount < 1f)
        {
            if (!_charging)
            {
                StartCoroutine(RechargeShield());
                shield.SetBool("IsActive", false);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (_isShielded) return;
        base.TakeDamage(damage);
        if (currentLife <= 0) Die(0);
    }

    public override void Die(int deathpoints)
    {
        _gameOverScreen.SetActive(true);
        SceneManagerr.Pause();
        this.gameObject.SetActive(false);
    }

    public override void Disparar()
    {
        DisparoSonido();
        var bala = OP_BulletManager.Instance.bulletPools[0].pool.Get();
        bala.Initialize(OP_BulletManager.Instance.bulletPools[0].pool);
        bala.transform.position = transform.position;
        bala.transform.forward = transform.forward;
    }

    public void ActivateShield()
    {
        if (gameUI.shieldFillCircle.fillAmount == 1f)
        {
            gameUI.shieldFillCircle.fillAmount = 0;
            StartCoroutine(_shieldPU.Activate());
            _audioSourceShield.PlayOneShot(_playerShieldAC);
            shield.SetBool("IsActive", true);
        }
    }

    public void DisparoSonido()
    {
        float randomPitch = Random.Range(0.7f, 1.2f);
        float randomVolume = Random.Range(0.1f, 0.3f);

        audioSource.pitch = randomPitch;
        audioSource.volume = randomVolume;

        audioSource.PlayOneShot(_playerDisparoAC);
    }

    public IEnumerator RechargeShield()
    {
        _audioSourceShield.PlayOneShot(_playerShieldDEAC);
        _charging = true;
        var timer = 0f;
        while (gameUI.shieldFillCircle.fillAmount < 1f)
        {
            Debug.Log("Cargando");
            timer += Time.deltaTime;
            gameUI.shieldFillCircle.fillAmount = timer / 15f;
            yield return null;
        }
        _charging = false;
    }
}