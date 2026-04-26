using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    public enum Character
    {
        Svistik, Shell, Dyrka, MrPi, 
        Kulich, Magomedova, Morena, Radmir
    }
    
    public static Character selectedCharacter;
    
    [Header("Кнопки персонажей (перетащить вручную)")]
    public Button btnSvistik;
    public Button btnShell;
    public Button btnDyrka;
    public Button btnMrPi;
    public Button btnKulich;
    public Button btnMagomedova;
    public Button btnMorena;
    public Button btnRadmir;
    
    [Header("Тексты описания")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI buffText;
    
    [Header("Кнопки управления")]
    public Button startButton;
    public Button backButton;
    
    [Header("Описания")]
    private string[] names = {
        "СВИСТИК",
        "ШЕЛЛИ", 
        "ДЫРКА",
        "МИСТЕР ПИ",
        "КУЛИЧ",
        "МАГОМЕДОВА",
        "МУРЕНА",
        "РАДМИР"
    };
    
    private string[] buffs = {
        "8-ка считается как перерыв",
        "+20% к удачному промо",
        "+8% эффективности на всех точках",
        "Шкала ярости",
        "Шанс ивентов x2",
        "Меньший штраф с проебов",
        "Увеличенная стамина",
        "Увеличенное восстановление стамины"
    };
    
    private Button[] characterButtons;
    private int selectedIndex = -1;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Собираем кнопки в массив
        characterButtons = new Button[] {
            btnSvistik, btnShell, btnDyrka, btnMrPi,
            btnKulich, btnMagomedova, btnMorena, btnRadmir
        };
        
        // Назначаем обработчики
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            if (characterButtons[i] != null)
                characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }
        
        // Кнопки управления
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGameClick);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClick);
        
        // Начальный текст
        if (descriptionText != null)
            descriptionText.text = "ВЫБЕРИ ПЕРСОНАЖА";
        
        if (buffText != null)
            buffText.text = "";
    }
    
    void SelectCharacter(int index)
    {
        selectedIndex = index;
        selectedCharacter = (Character)index;
        
        if (descriptionText != null)
            descriptionText.text = names[index];
        
        if (buffText != null)
            buffText.text = buffs[index];
        
        Debug.Log($"Выбран: {names[index]} - {buffs[index]}");
        
        // Подсветка выбранной кнопки
        for (int i = 0; i < characterButtons.Length; i++)
        {
            if (characterButtons[i] != null)
            {
                ColorBlock colors = characterButtons[i].colors;
                colors.normalColor = (i == index) ? Color.green : Color.white;
                characterButtons[i].colors = colors;
            }
        }
    }
    
    public void OnStartGameClick()
    {
        SceneManager.LoadScene("GameScene");

        if (selectedIndex == -1)
        {
            Debug.LogWarning("Сначала выбери персонажа!");
            if (descriptionText != null)
                descriptionText.text = "СНАЧАЛА ВЫБЕРИ ПЕРСОНАЖА!";
            return;
        }
        
        Debug.Log($"Запуск с персонажем: {names[selectedIndex]}");
        SceneManager.LoadScene("GameScene");
    }
    
    public void OnBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}