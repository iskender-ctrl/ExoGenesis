using UnityEngine;
using TMPro;

public class RocketUI
{
    private TextMeshProUGUI speedText;
    private Rigidbody rocketRigidbody;

    public RocketUI(TextMeshProUGUI speedText, Rigidbody rocketRigidbody)
    {
        this.speedText = speedText;
        this.rocketRigidbody = rocketRigidbody;
    }

    public void UpdateRocketSpeedUI ()
    {
        if (speedText != null && rocketRigidbody != null)
        {
            float speed = rocketRigidbody.linearVelocity.magnitude;
            speedText.text = $"{speed:F1} m/s";
        }
    }
}