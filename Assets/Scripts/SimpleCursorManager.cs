using UnityEngine;

public class SimpleCursorManager : MonoBehaviour
{
    void Start()
    {
        // カーソルの基本設定
        Cursor.visible = true;        // カーソルを表示
        Cursor.lockState = CursorLockMode.Confined;  // カーソルをウィンドウ内に制限
    }

    void Update()
    {
        // エラーが発生しやすい状況を防ぐ
        if (Cursor.lockState != CursorLockMode.Confined)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
} 