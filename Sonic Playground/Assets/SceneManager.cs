using Oculus.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    public PokeInteractable btn1;
    public PokeInteractable btn2;
    public PokeInteractable btn3;

    void OnEnable()
    {
        if (btn1 != null)
            btn1.WhenStateChanged += OnBtn1StateChanged;

        if (btn2 != null)
            btn2.WhenStateChanged += OnBtn2StateChanged;

        if (btn3 != null)
            btn3.WhenStateChanged += OnBtn3StateChanged;
    }

    void OnDisable()
    {
        if (btn1 != null)
            btn1.WhenStateChanged -= OnBtn1StateChanged;

        if (btn2 != null)
            btn2.WhenStateChanged -= OnBtn2StateChanged;

        if (btn3 != null)
            btn3.WhenStateChanged -= OnBtn3StateChanged;
    }

    private void OnBtn1StateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
            LoadScene("exhibit1");
    }

    private void OnBtn2StateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
            LoadScene("ex2.Cave");
    }

    private void OnBtn3StateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Select)
            LoadScene("Exhibit3_BasicStructure");
    }

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneManager: Scene Not Found");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"SceneManager: Scene '{sceneName}' Not in Build Settings! ");
        }
    }
}
