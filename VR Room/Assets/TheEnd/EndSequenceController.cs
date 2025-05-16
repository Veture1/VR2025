using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EndSequenceController : MonoBehaviour
{
    [Header("UI References")]
    public Image blackScreen;         
    public GameObject theEndText;     
    public GameObject playAgainBtn;   
    public GameObject quitBtn;
    
    public XROrigin playerXROrigin;
    public XRRayInteractor leftUIRayInteractor;
    public XRRayInteractor rightUIRayInteractor;
    
    private bool alreadyTriggered = false;

    
    void Start()
    {
        blackScreen.gameObject.SetActive(false);
        theEndText.SetActive(false);
        playAgainBtn.SetActive(false);
        quitBtn.SetActive(false);
        leftUIRayInteractor.gameObject.SetActive(false);
        rightUIRayInteractor.gameObject.SetActive(false);
        
    }
    public void TriggerEnd()
    {
        if (!alreadyTriggered)
        {
            alreadyTriggered = true;
            Debug.Log("Trigger Ending");
            StartCoroutine(EndSequence());
        }
    }

    private IEnumerator EndSequence()
    {
        blackScreen.gameObject.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            blackScreen.color = new Color(0, 0, 0, Mathf.Clamp01(t));
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        
        theEndText.SetActive(true);
        playAgainBtn.SetActive(true);
        quitBtn.SetActive(true);
        
        leftUIRayInteractor.gameObject.SetActive(true);
        rightUIRayInteractor.gameObject.SetActive(true);
        
        playerXROrigin.gameObject.GetComponent<CharacterController>().enabled = false;
    }
    
    
    public void OnPlayAgain()
    {
        Debug.Log("Play Again Button Clicked");
        SceneManager.LoadScene("RoomSample");   
    }

    public void OnQuit()
    {
        Debug.Log("Quit Button Clicked");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}