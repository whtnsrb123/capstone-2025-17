using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
public class AddButtonScalerToAll : MonoBehaviour
{
    [MenuItem("Tools/Add ButtonScaler To All UI Buttons In Scene")]
    private static void AddScalerToButtons()
    {
        // 씬 상의 모든 GameObject 중에서 Button이 붙은 오브젝트 찾기
        Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);

        int addedCount = 0;

        foreach (var button in allButtons)
        {
            // 이미 ButtonScaler가 붙어있으면 스킵
            if (button.GetComponent<ButtonScaler>() == null)
            {
                Undo.AddComponent<ButtonScaler>(button.gameObject); // 되돌리기 가능하게
                addedCount++;
            }
        }

        Debug.Log($"ButtonScaler 추가 완료: {addedCount}개 버튼에 적용됨.");
    }
}
#endif