using UnityEngine;

public class PowerCardSystem : MonoBehaviour
{
    public GameManager gameManager;
    void Start()
    {
        
    }

    void Update()
    {

    }
    public void LokiCard()
    {
        if (gameManager.powerCards.Contains("loki") && (gameManager.selectedPlayerCard == 1 || gameManager.selectedPlayerCard == 3 || gameManager.selectedPlayerCard == 5 || gameManager.selectedPlayerCard == 7 || gameManager.selectedPlayerCard == 9 || gameManager.selectedPlayerCard == 11 || gameManager.selectedPlayerCard == 13))
        {
            gameManager.multiplier += 2;
            Debug.Log("Loki Hit!!");
        }
        else
        {
            Debug.Log("Not Odd number");
        }
    }

    public void IcarisCard()
    {
        int mult = UnityEngine.Random.Range(2, 11);

        if (mult < 2 && gameManager.powerCards.Contains("icaris"))
        {
            gameManager.multiplier = mult;
            Debug.Log("Multiplier is :" + gameManager.multiplier);
        }
        else if (mult >= 2 && gameManager.powerCards.Contains("icaris"))
        {
            gameManager.multiplier = mult;
            int index = gameManager.powerCards.IndexOf("icaris");
            gameManager.powerCards.RemoveAt(index);
            Debug.Log("Multiplier is :" + gameManager.multiplier);
        }
    }
}
