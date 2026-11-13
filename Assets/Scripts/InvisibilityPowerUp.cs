using System.Collections;
using UnityEngine;

public class InvisibilityPowerUp : MonoBehaviour
{
    private float duration;        
    private Color ghostColor;    
    private bool collected;
    private float endTime;

    private GUIStyle labelStyle;

    private void Start()
    {
        duration = 8f;
        ghostColor = new Color(0.6f, 0.8f, 1f);
        collected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        StartCoroutine(InvisibilityRoutine(player));
    }

    private IEnumerator InvisibilityRoutine(PlayerMovement player)
    {
        collected = true;

        Renderer itemRenderer = GetComponentInChildren<Renderer>();
        if (itemRenderer != null) itemRenderer.enabled = false;
        GetComponent<Collider>().enabled = false;

        Renderer playerRenderer = player.GetComponentInChildren<Renderer>();
        Color originalColor = playerRenderer != null ? playerRenderer.material.color : Color.white;

        player.Stats.Invisible = true;
        if (playerRenderer != null) playerRenderer.material.color = ghostColor;
        endTime = Time.time + duration;
        Debug.Log("Invisibilidad activada: " + duration + " s");

        yield return new WaitForSeconds(duration);

        // Restablece el estado original
        player.Stats.Invisible = false;
        if (playerRenderer != null) playerRenderer.material.color = originalColor;
        Debug.Log("Invisibilidad terminada");

        Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (!collected) return;

        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 22;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.normal.textColor = Color.cyan;
        }

        float remaining = Mathf.Max(0f, endTime - Time.time);
        GUI.Label(new Rect(20, 60, 600, 40), "INVISIBLE  " + remaining.ToString("0.0") + " s", labelStyle);
    }
}