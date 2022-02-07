using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    public int targetFrameRate;

    private void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = targetFrameRate;
    }
}

