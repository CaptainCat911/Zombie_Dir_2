using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    public int targetFrameRate;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}

