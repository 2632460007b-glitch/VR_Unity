using UnityEngine;
using TMPro; // UI用
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class GoalSystem : MonoBehaviour
{
    [Header("UI設定")]
    public TMP_Text countdownText; // カウントダウン表示用

    public GameObject goalTextObject; // TextMeshProオブジェクト

    public PlayerRecorder recorder;

    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;

    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // 1. すでに発動済みなら無視
        if (isActivated) return;

        // 2. プレイヤーかどうか確認
        if (other.CompareTag("Player"))
        {
            isActivated = true;

            // テキスト表示
            goalTextObject.SetActive(true);

            // もしレコーダーが付いていたら、停止命令を出す
            if (recorder != null)
            {
                recorder.StopRecording();
                recorder.Save();

                moveProvider.enabled = false;
                turnProvider.enabled = false;
            }

            // 3. 自分（ゴール）の仕事である「終了カウントダウン」を始める
            StartCoroutine(ShutdownSequence());
        }
    }

    IEnumerator ShutdownSequence()
    {
        float timeLeft = 5.0f;
        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = $"システム終了まで: {timeLeft:F0}秒";
            
            yield return new WaitForSeconds(1.0f);
            timeLeft -= 1.0f;
        }

        Debug.Log("アプリケーション終了");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}