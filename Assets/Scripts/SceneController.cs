using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu,
    Accessioning,
    Inventory,
    ReferenceInterviews,
}

[CreateAssetMenu(fileName = "SceneController", menuName = "Scriptable Objects/Managers/SceneController")]
public class SceneController : ScriptableObject
{
    public async UniTask ChangeScene(Scene newScene)
    {
        Debug.Log("changing scene...");

        await OverlayManager.Instance.DisplayOverlay();

        switch (newScene)
        {
            case Scene.MainMenu:
                await SceneManager.LoadSceneAsync("MainMenu");

                break;

            case Scene.Inventory:
                await SceneManager.LoadSceneAsync("Inventory");

                break;
        }

        await OverlayManager.Instance.HideOverlay();
    }
}
