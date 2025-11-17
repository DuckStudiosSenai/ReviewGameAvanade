using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CatchPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput;

    public RandomFood randomFood;

    public CatchManager gameManager;

    [SerializeField] private Animator animator;

    [SerializeField] private AudioClip collectSound; // som ao coletar
    [SerializeField] private AudioClip collectHit;
    [SerializeField] private AudioSource stepSong;
    [SerializeField] private AudioClip patoQuack; // Som do pato


    private AudioSource audioSource;                 // MOD: referência ao AudioSource

    private float timer;          // contador do tempo
    private float nextQuackTime;  // próximo tempo para o som


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>(); // MOD

        nextQuackTime = Random.Range(3f, 8f);

    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        CheckAlive();
        AliveAnimator();

        if (Input.GetKeyDown(KeyCode.K)) 
        {
            CatchManager.lives -= 20;
        }

        // atualiza o timer
        timer += Time.deltaTime;

        // se passou do tempo, faz o som e define outro tempo aleatório
        if (timer >= nextQuackTime)
        {
            FazerSom();
            timer = 0f;
            nextQuackTime = Random.Range(3f, 8f); // tempo até o próximo som
        }
    }

    void FazerSom()
    {
        if (patoQuack != null)
        {
            audioSource.PlayOneShot(patoQuack);
        }
    }



    void FixedUpdate()
    {
        if (isAlive) 
        {
            HandleMovementFlip();

            

            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            if (moveInput != 0)
            {
                if (!stepSong.isPlaying)
                    stepSong.Play();

                animator.SetBool("isRunning", true);
            }
            else
            {
                if (stepSong.isPlaying)
                    stepSong.Stop();
                animator.SetBool("isRunning", false);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            CatchManager.lives += 20;
            if (CatchManager.lives >= 100)
            {
                CatchManager.lives = 100;
            }

            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

        }


        if (collision.CompareTag("Trash"))
        {
            Destroy(collision.gameObject);
            DepleteLife();
            

        }

    }



    public void OnDeathAnimationEnd()
    {
        // Aqui ele desaparece de vez
        gameObject.SetActive(false);
    }

    private void Death()
    {


        Debug.Log("Morreu!");
        //randomFood.StopCoroutine(randomFood.StartGeneratingFood());
        randomFood.StopFoodGeneration(); 
        animator.SetTrigger("Death");
        


        FindObjectOfType<DeathScreen>().ShowDeathScreen();
    }

    private void AliveAnimator()
    {
        animator.SetBool("isAlive", isAlive);
    }

    public void DepleteLife()
    {
        if (isAlive)
        {
            CatchManager.lives -= 20;
            animator.SetTrigger("Damage");

            if (collectHit != null)
            {
                audioSource.PlayOneShot(collectHit);
            }

        }    
    }

    private bool isAlive = true;
    private void CheckAlive()
    {
        if (CatchManager.lives <= 0 && isAlive)
        {
            isAlive = false;
            Death();


        }
    }

    private bool isFacingRight = false;
    private void HandleMovementFlip()
    {
        

        if (moveInput >= 0 && !isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = true;

            
        }
        else if (moveInput < 0 && isFacingRight)
        {
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                isFacingRight = false;

                

            }
        }
    }
}
