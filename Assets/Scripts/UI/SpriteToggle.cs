using UnityEngine;
using UnityEngine.UI;

public class SpriteToggle : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;

    private Image image;
    private bool isOn = true;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    // Call this from SettingsMenu to set initial state without clicking
    public void SetState(bool state)
    {
        if (image == null) image = GetComponent<Image>();

        isOn = state;
        image.sprite = isOn ? onSprite : offSprite;
    }

    // Call this via the Button Click Event
    public void Toggle()
    {
        isOn = !isOn;
        image.sprite = isOn ? onSprite : offSprite;
    }
}