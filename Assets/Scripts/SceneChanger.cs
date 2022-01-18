using Mirror;
using UnityEngine;

public class SceneChanger : NetworkBehaviour
{
    public Animator Animator;
    [SyncVar] private string _sceneToLoad;

    public string SceneToLoad { get => _sceneToLoad; set => _sceneToLoad = value; }

    public void FadeToLevel(string sceneToLoad)
    {
        SceneToLoad = sceneToLoad;
        Animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        NetworkClient.ready = false; 
        NetworkManager.singleton.ServerChangeScene(SceneToLoad);
        if (NetworkServer.active)
        {
            //NetworkManager.singleton.onlineScene = SceneToLoad;
            NetworkClient.ready = true;
        }
    }
}