using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class that describes the feild. Contains values representitve of locations on the field.
/// </summary>
public class Field : MonoBehaviour{

    #region SINGLETON PATTERN
    //simple singleton pattern. This allows functions in this class to be called globally as THERE CAN BE ONLY ONE!!!
    public static Field _instance;
    public static Field Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Field>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("Field");
                    _instance = container.AddComponent<Field>();
                }
            }

            return _instance;
        }
    }
    #endregion


    [Header("Components")]
    SpriteRenderer sr;

    [Header("Stored Values")]
    float fieldLength; //real dimention of the field
    float fieldWidth; 
    Vector2 fieldLocation; //where in the world the field is
    Vector2 fieldOrigin; //world coordinates for the 0.0 of the field grid.

    float singleUnitSizeX; //how many world units one grid unit on the field is. looks at both X and Y axis seperately.
    float singleUnitSizeY;

    void Start () {
        sr = GetComponent<SpriteRenderer>();

        fieldWidth = sr.bounds.size.x;
        fieldLength = sr.bounds.size.y;
        fieldLocation = new Vector2(transform.position.x, transform.position.y);
        fieldOrigin = new Vector2(fieldLocation.x - (fieldWidth/2),fieldLocation.y - (fieldLength/2));

        singleUnitSizeX = fieldWidth / 100;
        singleUnitSizeY = fieldLength / 100;

    }

    /// <summary>
    /// Converts Coordinates on the field grid to global cooridinates. 
    /// Incoming coordinates cannot have values above 100 or less thean 0.
    /// </summary>
    public Vector2 ConvertFieldCoordinateToGlobal(Vector2 coordinateIn)
    {
        //error checkign to make sure we arnt converting values off the grid.
        if (coordinateIn.x > 100 || coordinateIn.x < 0 || coordinateIn.y > 100 || coordinateIn.y < 0)
        {
            throw new UnityException("Coordinate Index out of bounds. Please ensure all values are beteween 0 and 100");
        }

        //use the world origin of the feild then add the grid units (in world space) to convert the coordinates;
        float coordinateX = fieldOrigin.x + (singleUnitSizeX*coordinateIn.x);
        float coordinateY = fieldOrigin.y + (singleUnitSizeY * coordinateIn.y);

        Vector2 coordinateOut = new Vector2(coordinateX, coordinateY);

        return coordinateOut;
    }

    /// <summary>
    /// checks if player is in the easternmost portion of the field.
    /// </summary>
    /// <returns></returns>
    bool CheckMidE()
    {
        return false;
    }
	
}
