using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    public bool destroyOnDeath;
    public const double maxHealth = 100.0;

    [SyncVar(hook = "OnChangeHealth")]
    public double currentHealth = maxHealth;

    public RectTransform healthBar;

    void Start() {

    }

    public void TakeDamage(double amount){
        if (!isServer){
            return;
        }

        currentHealth -= amount;
        if (currentHealth <= 0){
            print("Se me acabo las vidas");
            if (destroyOnDeath){
                Destroy(gameObject);
            }
        }
    }

    private void OnChangeHealth(double currentHealth)
    {
        healthBar.sizeDelta = new Vector2((float)currentHealth, healthBar.sizeDelta.y);
    }
}
