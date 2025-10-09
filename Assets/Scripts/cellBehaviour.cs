using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    public bool northCantFaceEdge;
    public bool eastCantFaceEdge;
    public bool southCantFaceEdge;
    public bool westCantFaceEdge;
    public GameObject module;

    // Never  Eat   Soggy  Waffles
    // North, East, South, West
    // 0      1     2      3

    // UpdateRoom will take an array of bools mapped to each
    // dir of the cell. Arr name "isFacingEdge", will be "true"
    // if facing an edge, "false" if facing a call

    public void UpdateRoom(bool[] isFacingEdge)
    {
        float numOfRotations = 0f;
        // If an edge is not allowed to face the edge, then
        // we need to check to make sure it isn't facing an
        // edge.
        if (northCantFaceEdge == true && isFacingEdge[0])
        { numOfRotations += 1f; }
        if (eastCantFaceEdge == true && isFacingEdge[1])
        { numOfRotations += 1f; }
        if (southCantFaceEdge == true && isFacingEdge[2])
        { numOfRotations += 1f; }
        if (westCantFaceEdge == true && isFacingEdge[3])
        { numOfRotations += 1f; }
        // We only need to rotate the gameobject if we got a
        // hit in the previous checks
        if (numOfRotations > 0)
        { Transform.Rotate(Vector3.up, 90 * numOfRotations); }
    }
}
