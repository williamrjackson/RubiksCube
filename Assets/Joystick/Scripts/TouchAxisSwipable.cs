using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAxisSwipable : MonoBehaviour
{
    public void MoveUp()
    {
        transform.position += Vector3.up;
    }
    public void MoveDown()
    {
        transform.position -= Vector3.up;
    }
    public void MoveRight()
    {
        transform.position += Vector3.right;
    }
    public void MoveLeft()
    {
        transform.position -= Vector3.right;
    }
}
