using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps a gameplay scene alive while the main menu is shown additively.
/// Resuming therefore restores the exact in-memory encounter state.
/// </summary>
public static class SuspendedGameSession
{
    private struct BehaviourState
    {
        public Behaviour Behaviour;
        public bool WasEnabled;

        public BehaviourState(Behaviour behaviour)
        {
            Behaviour = behaviour;
            WasEnabled = behaviour != null && behaviour.enabled;
        }
    }

    private static readonly List<BehaviourState> DisabledBehaviours = new List<BehaviourState>();
    private static Scene suspendedScene;
    private static SessionHost host;
    private static bool transitionRunning;
    private static bool sessionSuspended;

    public static bool HasSuspendedSession =>
        sessionSuspended && suspendedScene.IsValid() && suspendedScene.isLoaded;

    public static void SuspendToMenu()
    {
        if (HasSuspendedSession || transitionRunning) return;

        suspendedScene = SceneManager.GetActiveScene();
        if (!suspendedScene.IsValid() || suspendedScene.name == "Menu") return;
        sessionSuspended = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                if (controller.menu != null) controller.menu.SetActive(false);
                if (controller.stats != null) controller.stats.SetActive(true);
                if (controller.controllerMobile != null) controller.controllerMobile.SetActive(true);
            }
        }

        DisableGameplayPresentation();
        Time.timeScale = 0f;
        EnsureHost().StartCoroutine(LoadMenuAdditively());
    }

    public static bool Resume()
    {
        if (!HasSuspendedSession || transitionRunning) return false;

        Scene menuScene = SceneManager.GetSceneByName("Menu");
        SceneManager.SetActiveScene(suspendedScene);
        RestoreGameplayPresentation();
        sessionSuspended = false;
        Time.timeScale = 1f;

        if (menuScene.IsValid() && menuScene.isLoaded)
            EnsureHost().StartCoroutine(UnloadMenu(menuScene));

        return true;
    }

    /// <summary>Forget the paused scene when starting or loading another run.</summary>
    public static void Discard()
    {
        DisabledBehaviours.Clear();
        suspendedScene = default;
        sessionSuspended = false;
        transitionRunning = false;
        Time.timeScale = 1f;
    }

    private static IEnumerator LoadMenuAdditively()
    {
        transitionRunning = true;
        AsyncOperation load = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        if (load != null) yield return load;

        Scene menuScene = SceneManager.GetSceneByName("Menu");
        if (menuScene.IsValid() && menuScene.isLoaded)
            SceneManager.SetActiveScene(menuScene);
        transitionRunning = false;
    }

    private static IEnumerator UnloadMenu(Scene menuScene)
    {
        transitionRunning = true;
        AsyncOperation unload = SceneManager.UnloadSceneAsync(menuScene);
        if (unload != null) yield return unload;
        transitionRunning = false;
    }

    private static void DisableGameplayPresentation()
    {
        DisabledBehaviours.Clear();
        foreach (GameObject root in suspendedScene.GetRootGameObjects())
        {
            foreach (Camera camera in root.GetComponentsInChildren<Camera>(true))
                DisableAndRemember(camera);
            foreach (Canvas canvas in root.GetComponentsInChildren<Canvas>(true))
                DisableAndRemember(canvas);
            foreach (EventSystem eventSystem in root.GetComponentsInChildren<EventSystem>(true))
                DisableAndRemember(eventSystem);
            foreach (AudioListener listener in root.GetComponentsInChildren<AudioListener>(true))
                DisableAndRemember(listener);
            foreach (PlayerController controller in root.GetComponentsInChildren<PlayerController>(true))
                DisableAndRemember(controller);
            foreach (PlayerCombat combat in root.GetComponentsInChildren<PlayerCombat>(true))
                DisableAndRemember(combat);
        }
    }

    private static void DisableAndRemember(Behaviour behaviour)
    {
        if (behaviour == null) return;
        DisabledBehaviours.Add(new BehaviourState(behaviour));
        behaviour.enabled = false;
    }

    private static void RestoreGameplayPresentation()
    {
        foreach (BehaviourState state in DisabledBehaviours)
        {
            if (state.Behaviour != null)
                state.Behaviour.enabled = state.WasEnabled;
        }
        DisabledBehaviours.Clear();
    }

    private static SessionHost EnsureHost()
    {
        if (host != null) return host;
        GameObject hostObject = new GameObject("SuspendedGameSession");
        Object.DontDestroyOnLoad(hostObject);
        host = hostObject.AddComponent<SessionHost>();
        return host;
    }

    private sealed class SessionHost : MonoBehaviour
    {
        private void Update()
        {
            // Guard against hit-stop/dialogue coroutines that finish while the
            // additive menu is open and try to restore the gameplay clock.
            if (HasSuspendedSession && SceneManager.GetActiveScene().name == "Menu")
                Time.timeScale = 0f;
        }
    }
}
