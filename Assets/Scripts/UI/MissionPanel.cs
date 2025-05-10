using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.Cockpit;

public class MissionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionTitleText;
    [SerializeField] private TextMeshProUGUI missionContentText;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject hintTextPrefab;
    private string curSceneName;

    [SerializeField] private string[] missionContents;
    [SerializeField] private string[] mission1Hints;
    [SerializeField] private string[] mission2Hints;
    [SerializeField] private string[] mission3Hints;

    void Awake()
    {
        curSceneName = SceneManager.GetActiveScene().name;

        missionTitleText.text = "- " + curSceneName + " -";
        char missionNum = curSceneName[curSceneName.Length - 1];
        Debug.LogError(missionNum - '0' - 1);
        missionContentText.text = missionContents[missionNum - '0' - 1];
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            OnHintButton();
        }
    }

    public void OnHintButton()
    {
        if(!hintPanel.activeSelf)
        {
            for (int i = hintPanel.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = hintPanel.transform.GetChild(i).gameObject;
                Object.Destroy(child);
            }

            if (curSceneName == "Mission1")
            {
                foreach (var hintText in mission1Hints)
                {
                    GameObject hintTextObject = Instantiate(hintTextPrefab, hintPanel.transform);
                    hintTextObject.GetComponent<TextMeshProUGUI>().text = hintText;
                }
            }
            else if(curSceneName == "Mission2")
            {
                foreach (var hintText in mission2Hints)
                {
                    GameObject hintTextObject = Instantiate(hintTextPrefab, hintPanel.transform);
                    hintTextObject.GetComponent<TextMeshProUGUI>().text = hintText;
                }
            }
            else if (curSceneName == "Mission3")
            {
                foreach (var hintText in mission3Hints)
                {
                    GameObject hintTextObject = Instantiate(hintTextPrefab, hintPanel.transform);
                    hintTextObject.GetComponent<TextMeshProUGUI>().text = hintText;
                }
            }
        }

        hintPanel.SetActive(!hintPanel.activeSelf);
    }
}
