using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private float interactRange;
    private LayerMask interactableMask;
    private IInteractable current;
    private GUIStyle promptStyle;

    private void Start()
    {
        interactRange = 3f;
        interactableMask = LayerMask.GetMask("Interactable");

        if (interactableMask == 0)
            Debug.LogWarning("PlayerInteraction: no existe la capa Interactable. Crearla y asignarla a la terminal.");
    }

    private void Update()
    {
        current = null;
        if (Time.timeScale == 0f) return; 

        Vector3 origin = transform.position - transform.forward * 0.6f;
        float range = interactRange + 0.6f;

        RaycastHit hit;
        if (interactableMask != 0 &&
            Physics.Raycast(origin, transform.forward, out hit, range, interactableMask, QueryTriggerInteraction.Ignore))
        {
            current = hit.collider.GetComponentInParent<IInteractable>();
        }

        Debug.DrawRay(origin, transform.forward * range, current != null ? Color.yellow : Color.gray);

        if (current != null && Input.GetKeyDown(KeyCode.E))
            current.Interact();
    }

    private void OnGUI()
    {
        if (current == null || string.IsNullOrEmpty(current.Prompt)) return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.label);
            promptStyle.fontSize = 24;
            promptStyle.fontStyle = FontStyle.Bold;
            promptStyle.alignment = TextAnchor.MiddleCenter;
            promptStyle.normal.textColor = Color.yellow;
        }

        GUI.Label(new Rect(0, Screen.height - 120f, Screen.width, 40f), "E · " + current.Prompt, promptStyle);
    }
}