using UnityEngine;

public class DisableMessages : MonoBehaviour
{
    void Awake()
    {
        // デバッグログを無効化
        Debug.unityLogger.logEnabled = false;
        
        // または特定のログレベルのみを無効化
        // Debug.unityLogger.filterLogType = LogType.Error | LogType.Assert;
    }
} 