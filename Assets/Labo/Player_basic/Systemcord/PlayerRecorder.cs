using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PlayerRecorder : MonoBehaviour
{
    [Header("ターゲット設定")]
    [Tooltip("プレイヤーのTransform（X, Z座標用）")]
    public Transform playerTransform;

    [Tooltip("カメラのTransform（Y軸回転角用）")]
    public Transform cameraTransform;

    [Header("ファイル保存設定")]
    [Tooltip("ファイル名の基本部分（日付が自動で付与されます）")]
    public string fileBaseName = "PlayerRecord";

    [Tooltip("チェックを入れると指定したフォルダに保存します。外すと標準のPersistentDataPathになります。")]
    public bool useCustomPath = false;

    [Tooltip("保存先のフォルダパス（useCustomPathが有効な場合に使用）")]
    public string customFolderPath = @"C:\Data_Logs";

    private List<string> csvData = new List<string>();
    private int currentIndex = 0;
    private bool isRecording = false;

    void Start()
    {
        if (playerTransform == null) playerTransform = this.transform;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        
        csvData.Add("---,X,Y,Z,T");
        // StartRecording();
    }

    public void StartRecording()
    {
        isRecording = true;
        StartCoroutine(RecordCoroutine());
    }

    IEnumerator RecordCoroutine()
    {
        while (isRecording)
        {
            float posX = playerTransform.position.x;
            float posZ = playerTransform.position.z;
            float rotY = cameraTransform.eulerAngles.y;

            string line = string.Format("{0},{1:F3},{2:F3},{3:F3},{4}",
                currentIndex, posX, posZ, rotY, 0.1f);

            csvData.Add(line);
            currentIndex++;

            yield return new WaitForSeconds(0.1f);
        }
    }

    // ★重要：外部（ゴール）から呼ばれるための関数
    public void StopRecording()
    {
        if (!isRecording) return;
        isRecording = false;
    }
    
    public void Save()
    {
        SaveToCSV();
    }

    private void SaveToCSV()
    {
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
        string finalFileName = $"{timeStamp}-{fileBaseName}.csv";

        string directoryPath = useCustomPath && !string.IsNullOrEmpty(customFolderPath)
            ? customFolderPath
            : Application.persistentDataPath;

        if (!Directory.Exists(directoryPath))
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"フォルダを作成しました: {directoryPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"フォルダ作成エラー: {e.Message}");
                directoryPath = Application.persistentDataPath;
            }
        }

        string fullPath = Path.Combine(directoryPath, finalFileName);

        try
        {
            File.WriteAllLines(fullPath, csvData, Encoding.UTF8);
            Debug.Log($"CSVを保存しました: {fullPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CSV保存エラー: {e.Message}");
        }
    }
}


// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using UnityEngine.UI;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif

// public class PlayerRecorder : MonoBehaviour
// {
//     [Header("ターゲット設定")]
//     [Tooltip("プレイヤーのTransform（X, Z座標用）")]
//     public Transform playerTransform;

//     [Tooltip("カメラのTransform（Y軸回転角用）")]
//     public Transform cameraTransform;

//     [Header("UI設定")]
//     [Tooltip("カウントダウンを表示するTextコンポーネント")]
//     public Text countdownText;

//     [Header("ファイル保存設定")]
//     [Tooltip("ファイル名の基本部分（日付が自動で付与されます）")]
//     public string fileBaseName = "PlayerRecord";

//     [Tooltip("チェックを入れると指定したフォルダに保存します。外すと標準のPersistentDataPathになります。")]
//     public bool useCustomPath = false;

//     [Tooltip("保存先のフォルダパス（useCustomPathが有効な場合に使用）")]
//     public string customFolderPath = @"C:\Data_Logs";

//     private List<string> csvData = new List<string>();
//     private int currentIndex = 0;
//     private bool isRecording = false;
//     private bool isShuttingDown = false;

//     void Start()
//     {
//         if (playerTransform == null)
//             playerTransform = this.transform;

//         if (cameraTransform == null && Camera.main != null)
//             cameraTransform = Camera.main.transform;

//         // ヘッダー
//         csvData.Add("Index,X,Z,RotY,Interval");

//         StartRecording();
//     }

//     public void StartRecording()
//     {
//         if (!isRecording)
//         {
//             isRecording = true;
//             StartCoroutine(RecordCoroutine());
//             Debug.Log("記録を開始しました");
//         }
//     }

//     IEnumerator RecordCoroutine()
//     {
//         while (isRecording)
//         {
//             float posX = playerTransform.position.x;
//             float posZ = playerTransform.position.z;
//             float rotY = cameraTransform.eulerAngles.y;

//             string line = string.Format("{0},{1:F3},{2:F3},{3:F3},{4}",
//                 currentIndex, posX, posZ, rotY, 0.1f);

//             csvData.Add(line);
//             currentIndex++;

//             yield return new WaitForSeconds(0.1f);
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!isShuttingDown && other.CompareTag("Player"))
//         {
//             StartCoroutine(ShutdownSequence());
//         }
//     }

//     IEnumerator ShutdownSequence()
//     {
//         isShuttingDown = true;
//         isRecording = false;

//         Debug.Log("エリア到達。記録を停止し、保存します。");
//         SaveToCSV();

//         float timeLeft = 5f;

//         while (timeLeft > 0)
//         {
//             if (countdownText != null)
//                 countdownText.text = $"システム終了まで: {timeLeft:F0}";

//             yield return new WaitForSeconds(1f);
//             timeLeft -= 1f;
//         }

//         if (countdownText != null)
//             countdownText.text = "終了中...";

//         Debug.Log("アプリケーションを終了します。");

// #if UNITY_EDITOR
//         EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//     }

//     public void SaveToCSV()
//     {
//         string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
//         string finalFileName = $"{timeStamp}-{fileBaseName}.csv";

//         string directoryPath = useCustomPath && !string.IsNullOrEmpty(customFolderPath)
//             ? customFolderPath
//             : Application.persistentDataPath;

//         if (!Directory.Exists(directoryPath))
//         {
//             try
//             {
//                 Directory.CreateDirectory(directoryPath);
//                 Debug.Log($"フォルダを作成しました: {directoryPath}");
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError($"フォルダ作成エラー: {e.Message}");
//                 directoryPath = Application.persistentDataPath;
//             }
//         }

//         string fullPath = Path.Combine(directoryPath, finalFileName);

//         try
//         {
//             File.WriteAllLines(fullPath, csvData, Encoding.UTF8);
//             Debug.Log($"CSVを保存しました: {fullPath}");
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"CSV保存エラー: {e.Message}");
//         }
//     }
// }

// #if UNITY_EDITOR
// [CustomEditor(typeof(PlayerRecorder))]
// public class PlayerRecorderEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();

//         EditorGUILayout.LabelField("ターゲット設定", EditorStyles.boldLabel);
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("playerTransform"));
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraTransform"));

//         EditorGUILayout.Space();

//         EditorGUILayout.LabelField("UI設定", EditorStyles.boldLabel);
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("countdownText"));

//         EditorGUILayout.Space();

//         EditorGUILayout.LabelField("ファイル保存設定", EditorStyles.boldLabel);
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("fileBaseName"));

//         SerializedProperty usePathProp =
//             serializedObject.FindProperty("useCustomPath");
//         EditorGUILayout.PropertyField(usePathProp);

//         GUI.enabled = usePathProp.boolValue;
//         EditorGUILayout.PropertyField(
//             serializedObject.FindProperty("customFolderPath"));
//         GUI.enabled = true;

//         serializedObject.ApplyModifiedProperties();
//     }
// }
// #endif