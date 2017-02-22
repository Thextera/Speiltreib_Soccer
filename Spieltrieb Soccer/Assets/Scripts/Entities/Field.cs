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
    public float fieldLength; //real dimention of the field
    public float fieldWidth;
    Vector2 fieldLocation; //where in the world the field is
    Vector2 fieldOrigin; //world coordinates for the 0.0 of the field grid.

    public GoalZone[] gz;

    float singleUnitSizeX; //how many world units one grid unit on the field is. looks at both X and Y axis seperately.
    float singleUnitSizeY;

    void Start () {
        ////TODO changing the sprite might have an odd effect on these calculations. may have to find a better way to do them once art comes in.
        //sr = GetComponent<SpriteRenderer>();
        //
        //fieldWidth = sr.bounds.size.x;
        //fieldLength = sr.bounds.size.y;

        fieldLocation = new Vector2(transform.position.x, transform.position.y);
        fieldOrigin = new Vector2(fieldLocation.x - (fieldWidth/2),fieldLocation.y - (fieldLength/2));

        singleUnitSizeX = fieldWidth / 100;
        singleUnitSizeY = fieldLength / 100;

        gz = FindObjectsOfType<GoalZone>();

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
        float coordinateX = fieldOrigin.x + (singleUnitSizeX * coordinateIn.x);
        float coordinateY = fieldOrigin.y + (singleUnitSizeY * coordinateIn.y);

        Vector2 coordinateOut = new Vector2(coordinateX, coordinateY);

        return coordinateOut;
    }


    public Vector2 ConvertGlobalToField(Vector2 coordinateIn)
    {

        //subtract the worldspace of the input from the field. divide the resultant value from the unit size will give you the grid location of an object.
        float coordinateX = (coordinateIn.x - fieldOrigin.x) / singleUnitSizeX;
        float coordinateY = (coordinateIn.y - fieldOrigin.y) / singleUnitSizeY;
        
        //TODO consider error checking. either of these coordinates are above 100 or below 0 the object in question is no longer on the feild.

        Vector2 coordinateOut = new Vector2(coordinateX, coordinateY);

        return coordinateOut;
    }


	
}
