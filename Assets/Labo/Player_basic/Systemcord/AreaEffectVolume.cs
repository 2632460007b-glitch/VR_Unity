using UnityEngine;
using UnityEngine.Rendering.PostProcessing; // ← ここをVRChat標準のPostProcessingに変更！

public class AreaEffectVolume : MonoBehaviour
{
    [Header("連携設定")]
    [Tooltip("操作したいPostProcessVolumeコンポーネント")]
    public PostProcessVolume targetVolume; // ← Volume から PostProcessVolume に変更！

    [Header("エフェクト設定")]
    [Tooltip("コライダー内部に入ったときのWeight（真っ暗にするなら1.0）")]
    public float effectWeight = 1.0f;

    [Tooltip("コライダー外部に出たときのWeight（明るくするなら0.0）")]
    public float normalWeight = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetVolume != null)
            {
                targetVolume.weight = effectWeight;
                Debug.Log("プレイヤーがエリアに入りました。画面を暗くします。");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetVolume != null)
            {
                targetVolume.weight = normalWeight;
                Debug.Log("プレイヤーがエリアを出ました。画面の明るさを戻します。");
            }
        }
    }
}