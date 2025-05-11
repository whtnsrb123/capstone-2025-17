using UnityEngine;
using UnityEngine.UI;

public class GearAlpha : MonoBehaviour
{
    private float time = 0;
    private Image myImage;
    void Start()
    {
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time >= 3.0f)
        {
            Color tempColor = myImage.color;
            tempColor.a = time - 3.0f;
            myImage.color = tempColor;
        }
        if(myImage.color.a > 1.0f)
        {
            Destroy(this);
        }
    }
}
