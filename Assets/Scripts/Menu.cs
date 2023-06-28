using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private InputField nameField;
    [SerializeField] private Toggle toggle;
    private int sounds;
    public int restartLevel;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("SoundOnOff"))
        {
            if (PlayerPrefs.GetInt("SoundOnOff") == 1)
                toggle.isOn = true;
            else toggle.isOn = false;
        }
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(restartLevel);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Player_Name"))
            nameField.text = PlayerPrefs.GetString("Player_Name");
    }
    public void OnEndEditName()
    {
        PlayerPrefs.SetString("Player_Name", nameField.text);
    }
    public void OnClickPlay()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void OnClickGoToMenu()         //Метод для выхода в главное меню
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
    public void OnClickSoundButton()      //Метод для включения/выключения звука
    {
        if (toggle.isOn)
        {
            sounds = 1;
            PlayerPrefs.SetInt("SoundOnOff", sounds);
            Debug.Log("Музыка включена" + sounds);
        }
        else
        {
            sounds = 2;
            PlayerPrefs.SetInt("SoundOnOff", sounds);
            Debug.Log("Музыка выключена" + sounds);
        }
    }
}
