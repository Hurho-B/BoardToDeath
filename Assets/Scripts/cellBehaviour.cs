using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    public bool northCanFaceEdge;
    public bool eastCanFaceEdge;
    public bool southCanFaceEdge;
    public bool westCanFaceEdge;

    // Never  Eat   Soggy  Waffles
    // North, East, South, West
    // 0      1     2      3

    // UpdateRoom will take an array of bools mapped to each
    // dir of the cell. Arr name "isFacingEdge", will be "true"
    // if facing an edge, "false" if facing a call

    public void UpdateRoom(bool[] isFacingEdge)
    {
        int numOfRotations = 0;
        // If an edge is not allowed to face the edge, then
        // we need to check to make sure it isn't facing an
        // edge.
        if (northCanFaceEdge == false && isFacingEdge[0])
        { numOfRotations += 1; }
        if (eastCanFaceEdge == false && isFacingEdge[1])
        { numOfRotations += 1; }
        if (southCanFaceEdge == false && isFacingEdge[2])
        { numOfRotations += 1; }
        if (westCanFaceEdge == false && isFacingEdge[3])
        { numOfRotations += 1; }
        // We only need to rotate the gameobject if we got a
        // hit in the previous checks
        if (numOfRotations > 0)
        { transform.RotateAround(transform.position, Vector3.up, 90 * numOfRotations); }
    }
}
