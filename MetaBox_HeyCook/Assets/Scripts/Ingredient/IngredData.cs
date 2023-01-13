using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObj/IngredientData", fileName = "IngredientData")]
public class IngredData : ScriptableObject
{
    public string ingredName;

    public Sprite ingredientImage;
    public Sprite trimedImage;

    public int taskCount;

    public int trimedLevel;
}
