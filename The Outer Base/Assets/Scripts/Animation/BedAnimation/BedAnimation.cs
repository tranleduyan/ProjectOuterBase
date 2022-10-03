using System.Collections;
using UnityEngine;

public class BedAnimation : MonoBehaviour
{
    [SerializeField] private UIInventoryBar uiInventoryBar;
    private bool doorIsOpen = false;
    private bool playerIsHere = false;

    [SerializeField] private BoxCollider2D boxCollider2D;

    private Vector3 playerEnteredPosition;
    private void Start()
    {
        uiInventoryBar = GameObject.FindObjectOfType<UIInventoryBar>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == Settings.PlayerTag)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("playerIsHere", true);
            playerIsHere = true;
            StartCoroutine(bedDoorOpen(animator));
        }
    }

    private IEnumerator bedDoorOpen(Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("BedDoorOpened")){
            yield return null;
        }
        doorIsOpen = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Settings.PlayerTag)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("playerIsHere", false);
            playerIsHere = false;
            doorIsOpen = false;
            StartCoroutine(bedDoorClose(animator));
        }
    }

    private IEnumerator bedDoorClose(Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("BedDoorClosed"))
        {
            yield return null;
        }
        UIManager.Instance.messageBox.SetActive(false);
        UIManager.Instance.promptText.text = "";
    }

    private void Update()
    {
        if (doorIsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UseBed();
            }
        }
        if(playerIsHere == true && Player.Instance.PlayerOnOtherObject == false)
        {
            //enable the bed's collider
            boxCollider2D.enabled = true;
        }
    }

    private void UseBed()
    {
        if (Player.Instance.PlayerOnOtherObject == false)
        {
            Player.Instance.PlayerOnOtherObject = true;

            //Destroy current dragged items
            uiInventoryBar.DestroyCurrentlyDraggedItems();

            //clear currenly selected items
            uiInventoryBar.ClearCurrentlySelectedItems();

            //hide inventory bar
            uiInventoryBar.HideInventoryBar();

            //show the message box and prompt text
            UIManager.Instance.messageBox.SetActive(true);
            UIManager.Instance.promptText.text = "Do you want to sleep?";
            UIManager.Instance.messagePrompt = "Bed";

            //disable bed's collider
            boxCollider2D.enabled = false;

            //Store player position at enter
            Player.Instance.playerLastPosition = Player.Instance.player.transform.position;

            //bed position stored
            Vector3 bedPosition = new Vector3(transform.position.x, transform.position.y + 0.35f, transform.position.z);

            //teleport player into bed, disable movement and make the layer to 1 and set animation
            Player.Instance.player.transform.position = bedPosition;
            Player.Instance.DisablePlayerMovement();
            Player.Instance.ChangeSortingLayer(1);
            Player.Instance.playerAnim.SetBool("isLieDown", true);
        }

        else
        {
            Player.Instance.PlayerOnOtherObject = false;

            //reset and disable the message box
            UIManager.Instance.messageBox.SetActive(false);
            UIManager.Instance.promptText.text = "";
            UIManager.Instance.messagePrompt = "";

            //enable the bed's collider
            boxCollider2D.enabled = true;

            //teleport the player to where he stand before lie down the bed, enable the movement and set the sorting layer to 0
            Player.Instance.player.transform.position = Player.Instance.playerLastPosition;
            Player.Instance.playerAnim.SetBool("isLieDown", false);
            Player.Instance.EnablePlayerMovement();
            Player.Instance.ChangeSortingLayer(0);

            //Show the inventory bar again
            uiInventoryBar.ShowInventoryBar();
        }

    }

    private void OnDestroy()
    {

    }
}
