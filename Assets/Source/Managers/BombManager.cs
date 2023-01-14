using System.Collections;
using System;
using UnityEngine;
using TMPro;

using RANDOM = UnityEngine.Random;

public class BombManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float minDeployTime;
    [SerializeField] private float maxDeployTime;
    [SerializeField] private float timeBetweenTicks;

    [Header("Score")]
    [SerializeField] private float initialScore;
    [SerializeField] private float tickValue;

    [Header("Conversion")]
    [SerializeField] private float scalingMultiplier;

    [Header("Effects")]
    [SerializeField] private ParticleSystem deployParticle;


    private int tickCounter;
    private float initialScale;
    private TextMeshProUGUI textMeshPro;


    public static Action OnDeployed;


    public float CurrentScore => initialScore + tickValue * tickCounter;

    private void Awake()
    {
        initialScale = transform.localScale.x;

        textMeshPro = GetComponentInChildren<TextMeshProUGUI>(true);

        UIManager.OnPlaying += Deploy;
        UIManager.OnSetting += Restart;

        Restart();
    }

    private void Restart() 
    {
        gameObject.SetActive(true);

        tickCounter = 0;
        textMeshPro.text = "x" + CurrentScore.ToString("0.0");

        transform.localScale = Vector3.one * initialScale;
    }

    private void Deploy() 
    {
        StartCoroutine(DeployingCoroutine());
    }

    private void Tick()
    {
        ++tickCounter;
        textMeshPro.text = "x" + CurrentScore.ToString("0.0");
    }

    private IEnumerator DeployingCoroutine() 
    {
        float timer = 0f;
        float deployTime = RANDOM.Range(minDeployTime, maxDeployTime);

        while (timer < deployTime)
        {
            if (timer > timeBetweenTicks * (tickCounter + 1)) Tick();

            transform.localScale = Vector3.one * (initialScale + timer * scalingMultiplier);

            timer += Time.deltaTime;

            yield return null;
        }

        OnDeployed();

        deployParticle.gameObject.transform.localScale = Vector3.one * (transform.localScale.x * scalingMultiplier);
        deployParticle.Play();
        gameObject.SetActive(false);
    }
}
