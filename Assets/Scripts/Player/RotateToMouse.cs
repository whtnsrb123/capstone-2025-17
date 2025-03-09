using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float rotCamXAxisSpeed = 5;  // 수직(X축) 회전 속도 (마우스 상하 움직임)
    [SerializeField]
    private float rotCamYAxisSpeed = 3;  // 수평(Y축) 회전 속도 (마우스 좌우 움직임)

    private float limitMinX = -30;  // 수직 회전 최소 각도 (아래쪽 제한)
    private float limitMaxX = 30;   // 수직 회전 최대 각도 (위쪽 제한)

    private float eulerAngleX;  // 현재 X축 회전 각도 (수직)
    private float eulerAngleY;  // 현재 Y축 회전 각도 (수평)

    // 마우스 입력을 받아 회전 적용
    public void UpdateRotate(float mouseX, float mouseY)
    {
        // 마우스 X축 움직임에 따라 캐릭터의 좌우 회전 (Y축 회전)
        eulerAngleY += mouseX * rotCamYAxisSpeed;

        // 마우스 Y축 움직임에 따라 캐릭터의 상하 회전 (X축 회전)
        eulerAngleX -= mouseY * rotCamXAxisSpeed;  // 마우스 Y축을 반대로 적용 (상하 반전)
        
        // 수직 회전(X축)을 제한하여 부자연스러운 뒤집힘 방지
        eulerAngleX = Mathf.Clamp(eulerAngleX, limitMinX, limitMaxX);

        // 계산된 회전 각도를 Transform에 적용
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }
}
