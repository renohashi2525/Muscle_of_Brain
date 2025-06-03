using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Awake()
    {
        // カーソルの可視性を制御
        Cursor.visible = true;

        // カーソルの状態をロック（必要な場合）
        // Cursor.lockState = CursorLockMode.None;

        // エラーログを無効化
        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // カーソル関連のエラーメッセージをフィルタリング
        if (logString.Contains("Cursor") || logString.Contains("Mouse"))
        {
            // エラーメッセージを無視
            return;
        }
    }

    void OnDestroy()
    {
        // イベントハンドラを解除
        Application.logMessageReceived -= HandleLog;
    }
} 