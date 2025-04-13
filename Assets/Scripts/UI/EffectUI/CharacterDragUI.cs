using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDragUI : MonoBehaviour, IDragHandler
{
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    GameObject player;


    public void OnDrag(PointerEventData eventData)
    {
        float mouseX = eventData.delta.x * Time.deltaTime * speed;

        player.transform.Rotate(0, -mouseX, 0);

        Vector3 rotate = player.transform.rotation.eulerAngles;
        player.transform.rotation = Quaternion.Euler(rotate.x, rotate.y, 0);
    }

}
