using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugFPS : MonoBehaviour
{
    public InputAction PauseAction;
    public InputAction StepAction;

    [RuntimeInitializeOnLoadMethod]
    static void SpawnOnLoad()
    {
        var prefab = Resources.Load<GameObject>("DebugFPS");
        if(prefab != null && prefab.TryGetComponent(out DebugFPS debugFPS))
        {
            var go = Instantiate(prefab);
            go.name = nameof(DebugFPS);
            DontDestroyOnLoad(go);
        }
    }

    public uint smoothedFPSFrameCount = 32;

    public Queue<float> frameTimes = new Queue<float>();

    float smoothedDeltaTime;

    private void OnEnable()
    {
        //Application.targetFrameRate = 60;
        frameTimes.Enqueue(Time.unscaledTime);
        PauseAction.Enable();
        StepAction.Enable(); 
        PauseAction.performed += PauseAction_performed;
        StepAction.performed += StepAction_performed;
    }

    private void StepAction_performed(InputAction.CallbackContext obj)
    {
        StartCoroutine(StepCoroutine());
    }

    private IEnumerator StepCoroutine()
    {
        Time.timeScale = 1.0f;
        yield return new WaitForNextFrameUnit();
        Time.timeScale = 0.0f;
    }

    private void PauseAction_performed(InputAction.CallbackContext obj)
    {
        if(Time.timeScale == 0f)
            Time.timeScale = 1f;
        else
            Time.timeScale = 0f;
    }

    private void Update()
    {
        float time = Time.unscaledTime;

        if (frameTimes.Count > smoothedFPSFrameCount)
            frameTimes.Dequeue();

        float lastTime = frameTimes.Peek();

        smoothedDeltaTime = 1000f * (time - lastTime) / frameTimes.Count;

        frameTimes.Enqueue(time);

    }

    StringBuilder _sb = new StringBuilder();


    private void OnGUI()
    {
        _sb.Clear();
        _sb.AppendLine("DEBUG INFO");
        _sb.AppendLine($"Frame Time : {smoothedDeltaTime.ToString("F2")} ms. ({(1000f/smoothedDeltaTime).ToString("F1")} FPS) ");
        _sb.AppendLine("");
        _sb.AppendLine($"F9 for Pause, F10 for Stepping 1 Frame");

        GUI.Label(new Rect(10, 10, 800, 400), _sb.ToString());
    }
}
