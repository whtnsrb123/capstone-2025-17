using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
public class AddButtonScalerToAll : MonoBehaviour
{
    [MenuItem("Tools/Add To All UI Buttons In Scene")]
    private static void AddScalerToButtons()
    {
        // 씬 상의 모든 GameObject 중에서 Button이 붙은 오브젝트 찾기
        Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);

        int scalerAdded = 0;
        int scalerRemoved = 0;

        int sfxAdded = 0;
        int sfxRemoved = 0;

        foreach (var button in allButtons)
        {
            // 이미 ButtonScaler가 붙어있으면 스킵
            if (button.GetComponent<ButtonScaler>() != null)
            {
                var scaler = button.GetComponent<ButtonScaler>();
                Undo.DestroyObjectImmediate(scaler);
                scalerRemoved++;
            }
            if (button.GetComponent<ButtonScaler>() == null)
            {
                Undo.AddComponent<ButtonScaler>(button.gameObject); 
                scalerAdded++;
            }

            if (button.GetComponent<ButtonClickSFX>() != null)
            {
                var sfx = button.GetComponent<ButtonClickSFX>();
                Undo.DestroyObjectImmediate(sfx);
                sfxRemoved++;
            }
            if (button.GetComponent<ButtonClickSFX>() == null)
            {
                Undo.AddComponent<ButtonClickSFX>(button.gameObject);
                sfxAdded++;
            }
        }

        Debug.Log($"ButtonScaler : 추가 {scalerAdded}개 / 삭제 {scalerRemoved}.\n" +
            $"ButtonSFX : 추가 {sfxAdded}개 / 삭제 {sfxRemoved}.");
    }
}
#endif