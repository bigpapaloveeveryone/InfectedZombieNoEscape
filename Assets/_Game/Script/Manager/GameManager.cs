using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] UserData userData;
    //[SerializeField] CSVData csv;
    public GameState gamestate = GameState.Start;

    // Start is called before the first frame update
    public bool isActive;
    public PlayerMovement player;
    public ZombieSpawnController spawnController;

    protected void Awake()
    {
        //base.Awake();
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        int maxScreenHeight = 1280;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }

        //csv.OnInit();
        //userData?.OnInitData();

        //ChangeState(GameState.MainMenu);
        if (isActive)
        {
            UIManager.Ins.OpenUI<MainCanvas>();
            UIManager.Ins.OpenUI<StartSceneCanvas>();
        }
    }

    public void ChangeState(GameState state)
    {
        gamestate = state;
    }

    public void StartPlay()
    {
        player.GetComponentInChildren<Animator>().Play(CacheString.TAG_INTRO);
        spawnController.enabled = true;
    }

    //public static bool IsState(GameState state)
    //{
    //    return gameState == state;
    //}

    public void Restart()
    {
        SceneManager.LoadScene("Scene1");
    }
}

public enum GameState
{
    Start,
    Play
}

