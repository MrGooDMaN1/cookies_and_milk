using UnityEngine.UI;
using UnityEngine;

public class UICharacterController : MonoBehaviour
{
    [SerializeField] private PressntButton left;
    [SerializeField] private PressntButton right;
    [SerializeField] private Button fire;
    [SerializeField] private Button jump;

    public PressntButton Left
    {
        get { return left; }
    }
    public PressntButton Right
    {
        get { return right; }
    }
    public Button Jump
    {
        get { return jump; }
    }
    public Button Fire
    {
        get { return fire; }
    }

    void Start()
    {
        Player.Instance.InitUIController(this);
    }
}
