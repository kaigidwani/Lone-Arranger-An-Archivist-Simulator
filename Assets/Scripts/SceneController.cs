using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu,
    Accessioning,
    ReadingRoom,
}

[CreateAssetMenu(fileName = "SceneController", menuName = "Scriptable Objects/Managers/SceneController")]
public class SceneController : ScriptableObject
{
    public async UniTask ChangeScene(Scene newScene)
    {
        await OverlayManager.Instance.DisplayOverlay();

        switch (newScene)
        {
            case Scene.MainMenu:
                await SceneManager.LoadSceneAsync("MainMenu");

                break;

            case Scene.Accessioning:
                await SceneManager.LoadSceneAsync("Accessioning");

                break;

            case Scene.ReadingRoom:
                await SceneManager.LoadSceneAsync("ReadingRoom");

                break;
        }

        await OverlayManager.Instance.HideOverlay();
    }
}
