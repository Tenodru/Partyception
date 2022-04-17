using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvatarPart
{
    public Sprite partSprite;
    public Color defaultColor;
}

[CreateAssetMenu(fileName = "New Avatar Info", menuName = "AvatarInfo")]
public class AvatarInfo : ScriptableObject
{
    public List<AvatarPart> avatarParts;
}