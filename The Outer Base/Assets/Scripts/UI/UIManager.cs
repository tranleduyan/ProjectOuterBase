using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private bool _pauseMenuOn = false;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;
    public GameObject messageBox = null;
    public TextMeshProUGUI promptText = null;
    public string messagePrompt = "";
    public GridCursor gridCursor = null;


    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        //Toggle pause menu if escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        //Destroy any currently dragged items
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        //clear currenly selected items
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerMovementIsDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        //Trigger garbage Collector
        System.GC.Collect();

        //Highlight Selected button
        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        //Destroy currently dragged items
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();
        PauseMenuOn = false;
        Player.Instance.PlayerMovementIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
    private void HighlightButtonForSelectedTab()
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }

            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }
    
    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.selectedColor;
        button.colors = colors;
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;

    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            if(i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnPressedYesButton()
    {
        promptText.text = "";
        messageBox.SetActive(false);
        if(messagePrompt == "Bed")
        {
            TimeManager.Instance.AdvanceGameDay();
            uiInventoryBar.ShowInventoryBar();
            Player.Instance.ChangeSortingLayer(0);
            Player.Instance.playerAnim.SetBool("isLieDown", false);
            messagePrompt = "";
        }
    }

    public void OnPressedNoButton()
    {
        promptText.text = "";
        messageBox.SetActive(false);
        if (messagePrompt == "Bed")
        {
            Player.Instance.PlayerOnOtherObject = false;

            Player.Instance.player.transform.position =  Player.Instance.playerLastPosition;
            Player.Instance.playerAnim.SetBool("isLieDown", false);
            Player.Instance.EnablePlayerMovement();
            Player.Instance.ChangeSortingLayer(0);

            //Show the inventory bar again
            uiInventoryBar.ShowInventoryBar();

            //reset prompt
            messagePrompt = "";
        }
    }
}
