using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    public AvatarInfo currentAvatar;
    public Image selectedAvatarPart;
    public List<Image> avatarParts;
    public Slider hueSlider;
    public Slider satSlider;
    public Slider brightSlider;
    public GameObject sliders;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            avatarParts.Add(transform.GetChild(i).GetComponent<Image>());
        }

        for(int i = 0; i < currentAvatar.avatarParts.Count; i++)
        {
            avatarParts[i].gameObject.SetActive(true);
            avatarParts[i].alphaHitTestMinimumThreshold = 0.5f;
            avatarParts[i].sprite = currentAvatar.avatarParts[i].partSprite;
            avatarParts[i].color = currentAvatar.avatarParts[i].defaultColor;
        }
    }

    public void SelectAvatarPart(Image image)
    {
        if (selectedAvatarPart == image)
        {
            for (int i = 0; i < currentAvatar.avatarParts.Count; i++)
            {
                avatarParts[i].color = new Color(avatarParts[i].color.r, avatarParts[i].color.g, avatarParts[i].color.b, 1);
            }
            selectedAvatarPart = null;
            sliders.SetActive(false);
        }
        else
        {
            sliders.SetActive(true);
            float h, s, v;
            selectedAvatarPart = image;
            for (int i = 0; i < currentAvatar.avatarParts.Count; i++)
            {
                avatarParts[i].color = new Color(avatarParts[i].color.r, avatarParts[i].color.g, avatarParts[i].color.b, 0.2f);
            }
            selectedAvatarPart.color = new Color(selectedAvatarPart.color.r, selectedAvatarPart.color.g, selectedAvatarPart.color.b, 1);
            Color.RGBToHSV(selectedAvatarPart.color, out h, out s, out v);
            hueSlider.value = h;
            satSlider.value = s;
            brightSlider.value = v;
        }
    }

    public void UpdateAvatarPartColor()
    {
        if (selectedAvatarPart != null)
        {
            Color newColor = Color.HSVToRGB(hueSlider.value, satSlider.value, brightSlider.value);
            selectedAvatarPart.color = new Color(newColor.r, newColor.g, newColor.b);
        }
    }
}
