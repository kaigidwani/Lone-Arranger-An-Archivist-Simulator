using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu,
    Inventory,
    ReferenceInterviews,
}

[CreateAssetMenu(fileName = "SceneController", menuName = "Scriptable Objects/Managers/SceneController")]
public class SceneController : ScriptableObject
{
    public async UniTask ChangeScene(Scene newScene)
    {
        await TransitionManager.Instance.DisplayOverlay();

        switch (newScene)
        {
            case Scene.MainMenu:
                await SceneManager.LoadSceneAsync("MainMenu");

                break;

            case Scene.Inventory:
                await SceneManager.LoadSceneAsync("Inventory");

                break;
        }

        await TransitionManager.Instance.HideOverlay();
    }
}
