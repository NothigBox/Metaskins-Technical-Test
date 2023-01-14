using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField betInput;
    [SerializeField] private Button playBtn;
    [SerializeField] private Button plantBtn;
    [SerializeField] private Button retryBtn;
    [SerializeField] private float timeToActivateRetry;


    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI plantLabel;
    [SerializeField] private TextMeshProUGUI prizeLabel;

    private int betValue;
    private float plantValue;

    public static Action OnPlaying;
    public static Action OnSetting;

    private string PlantText => "x" + plantValue.ToString("0.0");

    public int Prize => (int) Math.Round(betValue * plantValue);

    private void Awake()
    {
        BombManager.OnDeployed += () => SetGamePhase((int) GamePhase.Reviewing);
    }

    private void Start()
    {
        SetGamePhase((int) GamePhase.Setting);
    }

    public void SetGamePhase(int phase) 
    {
        TurnOffButtons();
        TurnOffLabels();

        switch ((GamePhase) phase)
        {
            // When introducing a valid Bet before Play
            case GamePhase.Setting:
                plantValue = 0f;

                playBtn.gameObject.SetActive(true);

                betInput.interactable = true;

                OnSetting();
                break;

            // When Bomb is deploying and until Plant
            case GamePhase.Playing:
                if (int.TryParse(betInput.text, out betValue) == false) 
                {
                    playBtn.gameObject.SetActive(true);
                    return;
                }
                else if (betValue <= 0) 
                {
                    playBtn.gameObject.SetActive(true);
                    return;
                }

                plantBtn.gameObject.SetActive(true);

                betInput.interactable = false;

                OnPlaying();
                break;

            // When Plant and unitl Bomb deployed
            case GamePhase.Waiting:
                plantValue = FindObjectOfType<BombManager>().CurrentScore;

                plantLabel.gameObject.SetActive(true);

                plantLabel.text = PlantText;
                break;

            // When Bomb deployed
            case GamePhase.Reviewing:
                retryBtn.gameObject.SetActive(true);
                retryBtn.interactable = false;
                Invoke(nameof(ActivateRetry), timeToActivateRetry);

                plantLabel.gameObject.SetActive(true);
                prizeLabel.gameObject.SetActive(true);

                plantLabel.text = PlantText;
                prizeLabel.text = "$ " + Prize;
                break;
        }
    }

    private void TurnOffButtons()
    {
        playBtn.gameObject.SetActive(false);
        plantBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);
    }

    private void TurnOffLabels()
    {
        plantLabel.gameObject.SetActive(false);
        prizeLabel.gameObject.SetActive(false);
    }

    private void ActivateRetry() 
    {
        retryBtn.interactable = true;
    }
}

public enum GamePhase { Setting = 0, Playing = 1, Waiting = 2, Reviewing = 3 }
