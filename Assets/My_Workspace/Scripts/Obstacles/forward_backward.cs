using UnityEngine;

public class KickSystem : MonoBehaviour
{
    [Header("Kick Settings")]
    public float kickPower = 15f;
    public float upwardForce = 5f;

    [Header("Cooldown")]
    public float cooldown = 1f;

    private bool canKick = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canKick) return;

        PlayerMovement player =
            other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            // Direction from kick object to player
            Vector3 direction =
                (other.transform.position - transform.position).normalized;

            direction.y = 0f;

            // Final force
            Vector3 finalForce =
                direction * kickPower +
                Vector3.up * upwardForce;

            // Apply knockback
            //player.AddForce(finalForce);

            // Start cooldown
            StartCoroutine(KickCooldown());
        }
    }

    System.Collections.IEnumerator KickCooldown()
    {
        canKick = false;

        yield return new WaitForSeconds(cooldown);

        canKick = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);

        Collider col = GetComponent<Collider>();

        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawCube(
                box.center,
                box.size
            );

            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(
                box.center,
                box.size
            );
        }
    }
}