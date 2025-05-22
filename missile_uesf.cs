using UnityEngine;

public class missile_uesf : MonoBehaviour
{
    public float ejectionForce = 3f;
    public float engineDelay = 1f;
    public float thrustForce = 1000f;
    public float turnSpeed = 1f; // Vitesse à laquelle le missile tourne vers la cible
    public ParticleSystem engineEffect;

    private Rigidbody rb;
    private bool engineOn = false;
    private bool launched = false;
    private Transform target;

    void Start()
    {
       rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Assure-toi que l'effet des particules est désactivé au départ
        // if (engineEffect != null)
        //     engineEffect.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!launched && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Touche E pressée — Lancement du missile !");
            transform.parent = null;      // Détache du vaisseau
            rb.isKinematic = false;       // Active la physique
            LaunchMissile();
        }
        else if (launched) // Correction : "launched" utilisé ici
        {
            rb.AddForce(transform.forward * thrustForce, ForceMode.Force);
        }
    }

    void LaunchMissile()
    {
        launched = true;

        // Applique une impulsion initiale vers l'avant
        rb.AddForce(transform.forward * ejectionForce, ForceMode.Impulse);

        // Cherche la cible la plus proche avec le tag "AE"
        target = FindClosestTarget("AE");

        // Démarre le moteur après un délai
        StartCoroutine(ActivateEngine());
    }

    System.Collections.IEnumerator ActivateEngine()
    {
        yield return new WaitForSeconds(engineDelay);
        engineOn = true;

        // Assure-toi que l'effet des particules est activé et qu'il commence à jouer
        if (engineEffect != null)
        {
            // engineEffect.gameObject.SetActive(true); // Active les particules
            // engineEffect.Play(); // Joue l'effet des particules
        }
    }

    void FixedUpdate()
    {
        if (engineOn)
        {
            // Si une cible existe, le missile s’oriente vers elle
            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime);
            }

            // Applique la poussée dans la direction actuelle
            rb.AddForce(transform.forward * thrustForce * Time.fixedDeltaTime);
        }
    }

    Transform FindClosestTarget(string tag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject t in targets)
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = t.transform;
            }
        }

        return closest;
    }
}
