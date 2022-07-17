using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LaserPointer : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private Image _iconImage;

    public void SetIcon(Sprite sprite)
    {
        _iconImage.sprite = sprite;
    }

    public void SetPosition(Vector3 position)
    {
        float x = position.x;
        float y = position.y;
        if (x > Screen.width)
        {
            x = Screen.width;
        }
        else if (x < Screen.width * -1f)
        {
            x = Screen.width * -1f;
        }

        if (y > Screen.height)
        {
            y = Screen.height;
        }
        else if (y < Screen.height * -1f)
        {
            y = Screen.height * -1f;
        }
#if UNITY_EDITOR
        Debug.Log($"Laser Pointer Position ( {x}, {y} )");
#endif //UNITY_EDITOR
        _rectTransform.position = new Vector3(x, y, 0f);
    }
}
