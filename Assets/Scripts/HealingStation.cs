using UnityEngine;

public class HealingStation : MonoBehaviour
{
    public Light stationLight;
    public ParticleSystem stationParticles;

    private int healAmount;
    private float healInterval;
    private int maxCharge;
    private int charge;
    private float cooldown;
    private float cooldownTimer;
    private float healTimer;
    private bool recharging;
    private PlayerHealth playerInside;
    private GUIStyle labelStyle;

    private void Start()
    {
        healAmount = 10;        
        healInterval = 1.5f;    
        maxCharge = 50;         
        cooldown = 8f;         
        charge = maxCharge;
        SetVisuals(true);
    }

    private void Update()
    {
        if (recharging)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                recharging = false;
                charge = maxCharge;
                healTimer = 0f;
                SetVisuals(true);
                Debug.Log("Estación de curación lista");
            }
            return;
        }

        if (playerInside == null || playerInside.IsDead) return;

        // Solo cura si el jugador está herido
        if (playerInside.CurrentHealth >= playerInside.MaxHealth)
        {
            healTimer = 0f;
            return;
        }

        healTimer += Time.deltaTime;
        if (healTimer >= healInterval)
        {
            healTimer = 0f;
            int given = playerInside.Heal(Mathf.Min(healAmount, charge));
            charge -= given;
            Debug.Log("Estación: +" + given + " HP (vida " + playerInside.CurrentHealth + "/" + playerInside.MaxHealth + ", carga " + charge + ")");

            if (charge <= 0) StartCooldown();
        }
    }

    private void StartCooldown()
    {
        recharging = true;
        cooldownTimer = cooldown;
        SetVisuals(false);
        Debug.Log("Estación en recarga (" + cooldown + " s)");
    }

    private void SetVisuals(bool on)
    {
        if (stationLight != null) stationLight.enabled = on;
        if (stationParticles != null)
        {
            if (on) stationParticles.Play();
            else stationParticles.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health != null)
        {
            playerInside = health;
            healTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerInside != null && other.GetComponentInParent<PlayerHealth>() == playerInside)
            playerInside = null;
    }

    private void OnGUI()
    {
        bool show = recharging || playerInside != null;
        if (!show) return;

        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 22;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
        }

        labelStyle.normal.textColor = recharging ? Color.gray : Color.green;
        string text = recharging
            ? "Estación en recarga: " + Mathf.Ceil(cooldownTimer) + " s"
            : "Curando... carga " + charge + "/" + maxCharge;
        GUI.Label(new Rect(0, 100f, Screen.width, 40f), text, labelStyle);
    }
}