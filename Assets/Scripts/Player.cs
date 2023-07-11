using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 2.5f;
    public float Speed
    {
        get { return speed; }
        set
        {
            if (value > 0.5)
                speed = value;
        }
    }
    [SerializeField] private float force;
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private float minimalHeight;
    [SerializeField] private bool isCheatMod;
    [SerializeField] private GD GD;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Arrow arrow;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float shootForce = 5;
    [SerializeField] private float cooldown;
    [SerializeField] private float damageForce;
    [SerializeField] private int arrowsCount = 3;
    [SerializeField] private Health health;
    [SerializeField] private Item item;
    [SerializeField] private BuffReciever buffReciever;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject menuDead;
    public Health Health { get { return health; } }
    private Arrow currentArrow;
    private float bonusForce;
    private float bonusHealth;
    private float bonusDamage;
    private List<Arrow> arrowPool;
    private bool isJumping;
    private bool isCooldown;
    private bool isBlockMovement;
    private UICharacterController controller;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        arrowPool = new List<Arrow>();
        for (int i = 0; i < arrowsCount; i++)
        {
            var arrowTemp = Instantiate(arrow, arrowSpawnPoint);
            arrowPool.Add(arrowTemp);
            arrowTemp.gameObject.SetActive(false);
        }

        health.OnTakeHit += TakeHit;

        buffReciever.OnBuffsChanged += ApplyBuffs;
    }

    public void InitUIController(UICharacterController uiController)
    {
        controller = uiController;
        controller.Jump.onClick.AddListener(Jump);
        controller.Fire.onClick.AddListener(CheckShoot);
    }

    #region Singleton
    public static Player Instance { get; set; }
    #endregion
    private void ApplyBuffs()
    {
        var forceBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Force);
        var damageBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Damage);
        var armorBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Armor);
        bonusForce = forceBuff == null ? 0 : forceBuff.additiveBonus;
        bonusHealth = armorBuff == null ? 0 : armorBuff.additiveBonus;
        health.SetHealth((int)bonusHealth);
        bonusDamage = damageBuff == null ? 0 : damageBuff.additiveBonus;
    }

    private void TakeHit(int damage, GameObject attacker)
    {
        animator.SetBool("GetDamage", true);
        animator.SetTrigger("TakeHit");
        isBlockMovement = true;
        rigidbody.AddForce(transform.position.x < attacker.transform.position.x ?
            new Vector2(-damageForce, 0) : new Vector2(damageForce, 0), ForceMode2D.Impulse);
    }

    public void UnBlockMovement()
    {
        isBlockMovement = false;
        animator.SetBool("GetDamage", false);
    }

    private void FixedUpdate()
    {
        Move();
        animator.SetFloat("Speed", Mathf.Abs(rigidbody.velocity.x));
        CheckFall();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance.OnClickExitPause();
        if (Input.GetKeyDown(KeyCode.E))
            Jump();
#endif
    }

    private void Move()
    {
        animator.SetBool("isGrounded", GD.isGrounded);
        if (!isJumping && !GD.isGrounded)
        {
            animator.SetTrigger("StartFall");
        }
        isJumping = isJumping && !GD.isGrounded;
        direction = Vector3.zero; // (0;0)
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.A))
            direction = Vector3.left; // (-1;0)
        if (Input.GetKey(KeyCode.D))
            direction = Vector3.right; // (1;0)
#endif
        if (controller.Left.IsPressed)
            direction = Vector3.left; // (-1;0)
        if (controller.Right.IsPressed)
            direction = Vector3.right; // (1;0)
        direction *= speed;
        direction.y = rigidbody.velocity.y;
        if (!isBlockMovement)
            rigidbody.velocity = direction;

        if (direction.x > 0)
            spriteRenderer.flipX = false;
        if (direction.x < 0)
            spriteRenderer.flipX = true;
    }

    private void Jump()
    {
        if (GD.isGrounded)
        {
            rigidbody.AddForce(Vector2.up * (force + bonusForce), ForceMode2D.Impulse);
            animator.SetTrigger("StartJump");
            isJumping = true;
        }
    }

    private void CheckShoot()
    {
        if (!isCooldown)
        {
            animator.SetTrigger("StartShoot");
        }
    }

    public void InitArrow()
    {
        currentArrow = GetArrowFromPool();
        currentArrow.SetImpulse(Vector2.right, 0, 0, this);
    }

    private void Shoot()
    {
        currentArrow.SetImpulse
            (Vector2.right, spriteRenderer.flipX ?
            -force * shootForce : force * shootForce, (int)bonusDamage, this);
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isCooldown = false;
    }

    private Arrow GetArrowFromPool()
    {
        if (arrowPool.Count > 0)
        {
            var arrowTemp = arrowPool[0];
            arrowPool.Remove(arrowTemp);
            arrowTemp.gameObject.SetActive(true);
            arrowTemp.transform.parent = null;
            arrowTemp.transform.position = arrowSpawnPoint.transform.position;
            return arrowTemp;
        }
        return Instantiate
            (arrow, arrowSpawnPoint.position, Quaternion.identity);
    }

    public void ReturnArrowToPool(Arrow arrowTemp)
    {
        if (!arrowPool.Contains(arrowTemp))
            arrowPool.Add(arrowTemp);

        arrowTemp.transform.parent = arrowSpawnPoint;
        arrowTemp.transform.position = arrowSpawnPoint.transform.position;
        arrowTemp.gameObject.SetActive(false);
    }

    void CheckFall()
    {
        if (transform.position.y < minimalHeight && isCheatMod)
        {
            rigidbody.velocity = new Vector2(x: 0, y: 0);
            transform.position = new Vector2(x: 0, y: 0);
        }
        else if (transform.position.y < minimalHeight)
            Destroy(gameObject);
    }

    public void MenuDead()
    {
        menuDead.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        playerCamera.transform.parent = null;
        playerCamera.enabled = true;
        menuDead.SetActive(true);
    }
}