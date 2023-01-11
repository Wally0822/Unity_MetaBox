using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptableObj", menuName = "ScriptableObjects/ScriptableObj", order = 5)]
public class ScriptableObj : ScriptableObject
{
    public Sprite hideThief;
    public Sprite nomalThief;
    public Sprite wantedThief;
}
