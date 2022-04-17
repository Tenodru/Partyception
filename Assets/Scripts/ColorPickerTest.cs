using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorPickerTest : MonoBehaviour
{
    //public Texture2D tex;
    public Image colorImage;
    public Image testImage;
    public Canvas mainCanvas;

    Texture2D colorTex;
    bool colorPickMode = false;
    Color pixColor = new Color();

    public GraphicRaycaster rayCaster;
    PointerEventData eventData;
    EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        colorTex = colorImage.sprite.texture;
        eventSystem = GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            colorPickMode = !colorPickMode;
        }
        if (colorPickMode)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                Renderer rend = hit.transform.GetComponent<Renderer>();

                Texture2D testTex = rend.material.mainTexture as Texture2D;
                Vector2 pix = hit.point;
                pix.x *= testTex.width;
                pix.y *= testTex.height;

                pixColor = colorImage.sprite.texture.GetPixel((int)pix.x, (int)pix.y);
                Debug.Log("Detected Color: " + pixColor);

            }
            if (Input.GetMouseButtonDown(0))
            {
                eventData = new PointerEventData(eventSystem);
                eventData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                Debug.Log("Results: " + results);

                rayCaster.Raycast(eventData, results);

                foreach (RaycastResult r in results)
                {
                    if (r.gameObject == colorImage.gameObject)
                    {
                        //pixColor = colorImage.sprite.texture.GetPixel((int)(r.screenPosition.x + colorImage.rectTransform.rect.x * (colorImage.rectTransform.rect.width / Screen.width)), (int)(r.screenPosition.y + colorImage.rectTransform.rect.y * (colorImage.rectTransform.rect.height / Screen.height)));
                        Vector2 adjustedPos = RectTransformUtility.PixelAdjustPoint(r.screenPosition, colorImage.rectTransform, mainCanvas);
                        pixColor = colorImage.sprite.texture.GetPixel((int)adjustedPos.x, (int)adjustedPos.y);
                        Debug.Log("Detected Color: " + pixColor);
                        Debug.Log("X: " + r.screenPosition.x + " Y: " + r.screenPosition.y);
                        Debug.Log("Adjusted X: " + adjustedPos.x + " Adjusted Y: " + adjustedPos.y);
                        Vector2 testVect = new Vector2(0, 24);
                        Vector3 testPos;
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(colorImage.rectTransform, Input.mousePosition, Camera.main, out testPos);
                        Debug.Log("Transform Utility Test: " + testPos);
                        Debug.Log("Multiplier: " + colorImage.rectTransform.rect.width * (colorImage.rectTransform.rect.width / Screen.width));
                        pixColor = colorImage.sprite.texture.GetPixel((int)(testPos.x / (colorImage.rectTransform.rect.width / Screen.width)), (int)(testPos.y / (colorImage.rectTransform.rect.height / Screen.height)));
                        Debug.Log("New Color: " + pixColor);
                        testImage.color = pixColor;
                        //Debug.Log("Screen Width: " + Screen.width);
                        //Debug.Log("Test: " + RectTransformUtility.WorldToScreenPoint(Camera.main, colorImage.rectTransform.position));
                    }
                }
            }
        }
    }
}
