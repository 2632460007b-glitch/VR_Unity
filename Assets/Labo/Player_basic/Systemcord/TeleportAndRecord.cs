using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportAndRecord : MonoBehaviour
{
    [Header("連携設定")]
    public Transform playerObject;
    public Transform teleportDestination;
    public PlayerRecorder playerRecorder;

    [Header("移動・操作制御")]
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;
    [Tooltip("プレイヤーの大元についているCharacterController")]
    public CharacterController characterController;

    [Header("フェード演出設定（マテリアル方式）")]
    [Tooltip("フェード用の黒いボックスのMeshRenderer")]
    public Renderer fadeBoxRenderer;

    private bool isTransitioning = false; 
    private Material fadeMaterial;

    private void Start()
    {
        if (fadeBoxRenderer != null)
        {
            fadeMaterial = fadeBoxRenderer.material;
            // SetActive(false)は削除し、最初から透明(Alpha=0)にしておく
            SetAlpha(0f);
        }
    }

    public void OnButtonPressed()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        // 1. 操作を無効化
        if (moveProvider != null) moveProvider.enabled = false;
        if (turnProvider != null) turnProvider.enabled = false;

        // 2. フェードアウト（テレポート前5秒のうち、最初の3秒で0から1へ）
        float elapsedTime = 0f;
        float fadeDuration = 1.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0.0f, 1.0f, elapsedTime / fadeDuration));
            yield return null;
        }
        SetAlpha(1.0f); // 確実に真っ黒にする

        // 3. 残りの2秒間は真っ黒のまま待機（これでテレポート前合計5秒）
        yield return new WaitForSeconds(2.0f);

        // 4. 真っ黒な状態でテレポート
        if (playerObject != null && teleportDestination != null)
        {
            if (characterController != null) characterController.enabled = false;

            playerObject.position = teleportDestination.position;
            playerObject.rotation = teleportDestination.rotation;

            // ★操作不能バグ対策：CharacterControllerの再有効化前に1フレーム待つ
            yield return null; 

            if (characterController != null) characterController.enabled = true;
        }

        // 5. テレポート後5秒のうち、最初の2秒間は真っ黒のまま待機
        yield return new WaitForSeconds(2.0f);

        // 6. フェードイン（テレポート後5秒のうち、最後の3秒で1から0へ）
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration));
            yield return null;
        }
        SetAlpha(0.0f); // 確実に透明にする

        // 7. 記録開始 ＆ 操作を復帰
        if (playerRecorder != null)
        {
            playerRecorder.StartRecording();
        }

        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;

        isTransitioning = false;
    }

    private void SetAlpha(float alpha)
    {
        if (fadeMaterial != null)
        {
            // UI/Lit/Transparentなど、_Colorプロパティを持つ一般的なシェーダー用
            if (fadeMaterial.HasProperty("_Color"))
            {
                Color currentColor = fadeMaterial.color;
                currentColor.a = alpha;
                fadeMaterial.color = currentColor;
            }
        }
    }
}