using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] private float rotCamXAxisSpeed = 2;  // 상 하  감도
    [SerializeField] private float rotCamYAxisSpeed = 2;  // 좌 우 감도

    [SerializeField] private float limitMinX = -50;  // 수직 회전 최소 각도
    [SerializeField] private float limitMaxX = 25;   // 수직 회전 최대 각도

    private float eulerAngleX;  // 현재 X축 회전 각도
    private float eulerAngleY;  // 현재 Y축 회전 각도
    public Animator animator; 

    public void UpdateRotate(float mouseX, float mouseY)
    {
        // Y축 회전
        eulerAngleY += mouseX * rotCamYAxisSpeed;

        // X축 회전
        eulerAngleX -= mouseY * rotCamXAxisSpeed;  // 마우스 Y축을 반대로 적용 

        eulerAngleX = Mathf.Clamp(eulerAngleX, limitMinX, limitMaxX);
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }
}