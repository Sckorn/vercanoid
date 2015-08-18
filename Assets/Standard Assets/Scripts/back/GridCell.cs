using UnityEngine;
using System.Collections;
using System;

public class GridCell {
    private bool hasBrick = false;
    private GameObject realObjRef;
    private GridCellCoords cellCoords;

    public bool HasBrick
    {
        get { return this.hasBrick; }
        set { this.hasBrick = value; }
    }

    public GridCell(int _x, int _y, bool _hasBrick, char brickTypeChar = '\0')
    {
        this.cellCoords.x = _x;
        this.cellCoords.y = _y;
        this.hasBrick = _hasBrick;
        
        Morpher morph = GameObject.Find("Morpher").GetComponent<Morpher>();

        try
        {
            if (this.hasBrick)
                this.realObjRef = morph.MorphBrick(this.cellCoords.x, this.cellCoords.y, brickTypeChar);
        }
        catch (UnityException e)
        {
            this.cellCoords.x = -1;
            this.cellCoords.y = -1;

#if UNITY_EDITOR
            Debug.LogError("Can't instantiate Brick in Cell");
            Debug.LogError(e.Message);
            return;
#else
            //do later: handle exceptions to the main menu with a popup informing of the exception
            Application.Quit();
#endif
        }
        finally
        {
            if (this.realObjRef != null)
            {
                this.realObjRef.GetComponent<BrickObjectHandler>().SetCoordinates(this.cellCoords);
            }
        }
    }	
}
