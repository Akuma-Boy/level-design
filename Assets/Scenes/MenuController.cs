using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // Adicione esta linha para corrigir o erro

public class MenuController : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private string sceneToLoad = "dentro";
    [SerializeField] private Button playButton;

    [Header("Transição")]
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private Animator transitionAnimator;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Botão 'Jogar' não atribuído no Inspector!");
        }
    }

    public void StartGame()
    {
        if (playButton != null)
        {
            playButton.interactable = false;
        }

        StartCoroutine(LoadSceneWithTransition());
    }

    // Corrigido: Agora com o using System.Collections, IEnumerator será reconhecido
    private IEnumerator LoadSceneWithTransition()
    {
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
        }

        yield return new WaitForSeconds(transitionTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadSceneDirectly()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}