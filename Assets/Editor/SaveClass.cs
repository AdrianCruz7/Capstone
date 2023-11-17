using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveClass
{
    public int saveClassID = 0;
    public string prompt;
    public string userChoice;

    //for single generation
    public bool singleChildToggle;
    public bool singleOptionalObj;
    public int singleGO_ID;

    //single obj reference
    public SaveClass(string prompt, string userChoice, bool singleChildToggle, bool singleOptionalObj, int singleGO_ID)
    {
        this.prompt = prompt;
        this.userChoice = userChoice;
        this.singleChildToggle = singleChildToggle;
        this.singleOptionalObj = singleOptionalObj;
        this.singleGO_ID = singleGO_ID;

        saveClassID = 1;

        Debug.Log(prompt + " " + userChoice + " " + singleChildToggle + " " + singleOptionalObj + " " + singleGO_ID);
    }

    //if singleChildToggle is false;
    public Vector3 singlePosition;
    public Vector3 singleRotation;
    public Vector3 singleScale;

    //single no obj reference
    public SaveClass(string prompt, string userChoice, bool singleChildToggle, bool singleOptionalObj, Vector3 singlePosition, Vector3 singleRotation, Vector3 singleScale)
    {
        this.prompt = prompt;
        this.userChoice = userChoice;
        this.singleChildToggle = singleChildToggle;
        this.singleOptionalObj = singleOptionalObj;
        this.singlePosition = singlePosition;
        this.singleRotation = singleRotation;
        this.singleScale = singleScale;

        saveClassID = 2;

        Debug.Log(prompt + " " + userChoice + " " + singleChildToggle + " " + singleOptionalObj + " " + singlePosition + " " + singleRotation + " " + singleScale);
    }

    //placement method in multiple objects choice
    public string multiplePlacementMethod;
    //children object toggle in both methods
    public bool multipleChildToggle;

    //for multiple generation using tags
    public string multipleTag;

    //multiple object tag
    public SaveClass(string prompt, string userChoice, string multiplePlacementMethod, bool multipleChildToggle, string multipleTag)
    {
        this.prompt = prompt;   
        this.userChoice = userChoice;
        this.multiplePlacementMethod = multiplePlacementMethod;
        this.multipleChildToggle = multipleChildToggle;
        this.multipleTag = multipleTag;

        saveClassID = 3;
    }

    //for multiple generation using radius
    public int numberOfInstances;
    public bool spawnPointToggle;

    //depends on the spawn point toggle
    public int spawnPointGO_ID;
    public Vector3 spawnPointPosition;

    public bool radiusVisual;
    public float radiusValue;

    //rotation and scale settings
    public Vector3 rotationMin;
    public Vector3 rotationMax;
    public Vector3 scaleMin;
    public Vector3 scaleMax;

    public SaveClass(string prompt, string userChoice, string multiplePlacementMethod, bool spawnPointToggle, int spawnPointGO_ID, bool radiusVisual, float radiusValue, Vector3 rotationMin, Vector3 rotationMax, Vector3 scaleMin, Vector3 scaleMax)
    {
        this.prompt = prompt;
        this.userChoice = userChoice;
        this.multiplePlacementMethod = multiplePlacementMethod;
        this.spawnPointToggle = spawnPointToggle;
        this.spawnPointGO_ID = spawnPointGO_ID;
        this.radiusVisual = radiusVisual;
        this.radiusValue = radiusValue;
        this.rotationMin = rotationMin;
        this.rotationMax = rotationMax;
        this.scaleMin = scaleMin;
        this.scaleMax = scaleMax;

        saveClassID = 4;
    }
}
