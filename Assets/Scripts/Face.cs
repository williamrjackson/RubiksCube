using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    public enum FaceColor { Red, Orange, Blue, White, Yellow, Green }
    [SerializeField]
    private FaceColor faceColor = FaceColor.Red;

    public FaceColor Color => faceColor; 
    //TODO: Check all faces and see if they face the same direction of all like colors to detect win state
}
