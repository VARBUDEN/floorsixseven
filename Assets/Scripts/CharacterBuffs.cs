using UnityEngine;

public class CharacterBuffs : MonoBehaviour
{
    private StaminaNew stamina;
    private GameManager gameManager;
    private AngerSystem angerSystem;
    private CharacterSelect.Character selectedCharacter;

    void Start()
    {
        stamina = GetComponent<StaminaNew>();
        gameManager = FindAnyObjectByType<GameManager>();
        angerSystem = FindAnyObjectByType<AngerSystem>();
        selectedCharacter = CharacterSelect.selectedCharacter;
    if (stamina == null)
    {
        Debug.LogError("[CharacterBuffs] StaminaNew не найден на Player!");
        return;
    }
    if (gameManager == null)
    {
        Debug.LogError("[CharacterBuffs] GameManager не найден на сцене!");
        return;
    }
    if (angerSystem == null)
    {
        Debug.LogError("[CharacterBuffs] AngerSystem не найден на сцене!");
        return;
    }
        ApplyAllBuffs();
    }

    void ApplyAllBuffs()
    {
        switch (selectedCharacter)
        {
            case CharacterSelect.Character.Svistik:
                stamina.EnableSvistikBuff();
                break;
            case CharacterSelect.Character.Shell:
                gameManager.SetPromoBonus(35f);
                break;
            case CharacterSelect.Character.Dyrka:
                stamina.EnableDyrkaBuff(0.8f);
                break;
            case CharacterSelect.Character.MrPi:
                gameManager.EnableMrPiBuff();
                break;
            case CharacterSelect.Character.Kulich:
                Debug.Log("[Бафф] Кулич: x2 ивенты (будет позже)");
                break;
            case CharacterSelect.Character.Magomedova:
                angerSystem.EnableAminaBuff();
                break;
            case CharacterSelect.Character.Morena:
                stamina.EnableMorenaBuff(200f, 200f, 1.3f);
                break;
            case CharacterSelect.Character.Radmir:
                stamina.EnableRadminBuff(2f);
                break;
        }
    }
}