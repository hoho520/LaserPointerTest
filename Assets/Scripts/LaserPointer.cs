using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LaserPointer : MonoBehaviour
{
    private const float MAX_X = 850f;
    private const float MAX_Y = 430f;

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
        if (x > MAX_X || x < (MAX_X * -1f))
        {
            x = MAX_X;
        }

        if (y > MAX_Y || y < (MAX_Y * -1f))
        {
            y = MAX_Y;
        }

        _rectTransform.position = new Vector3(x, y, 0f);
    }
}
