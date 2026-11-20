using UnityEngine;

public class GameManager : MonoBehaviour
{
    private readonly int RotPropertyId = Shader.PropertyToID("_Rot");
    public bool toggleRot;
    private bool _rot = false;

    public static GameManager Instance;

    public bool Rot
    {
        get { return _rot; }
        set
        {

            if (_rot == value) return;

            _rot = value;

            float shaderValue = value ? 1.0f : 0.0f;

            Shader.SetGlobalFloat(RotPropertyId, shaderValue);
            Debug.Log($"Rot value: {Shader.GetGlobalFloat(RotPropertyId)}");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Shader.SetGlobalFloat(RotPropertyId, 0.0f);
        Debug.Log($"Rot value: {Shader.GetGlobalFloat(RotPropertyId)}");
    }

    private void Update()
    {
        if (toggleRot == true)
        {
            Rot = !_rot;
            toggleRot = false;
        }
    }


}

enum GameState
{
    MENU,
    LOADING,
    PLAYING,
    DEATHSCREEN,
    CREDITS
}