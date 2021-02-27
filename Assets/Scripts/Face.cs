using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    public enum FaceColor { Red, Orange, Blue, White, Yellow, Green }
    [SerializeField]
    private FaceColor faceColor = FaceColor.Red;

    public FaceColor Color => faceColor;

    private static List<Face> allFaces = new List<Face>();

    private void Start()
    {
        allFaces.Add(this);
    }

    public static bool IsWinState
    {
        get
        {
            Vector3 redDir = Vector3.zero;
            Vector3 orangeDir = Vector3.zero;
            Vector3 blueDir = Vector3.zero;
            Vector3 whiteDir = Vector3.zero;
            Vector3 yellowDir = Vector3.zero;
            Vector3 greenDir = Vector3.zero;
            foreach (Face face in allFaces)
            {
                switch (face.Color)
                {
                    case FaceColor.Red:
                        if (redDir == Vector3.zero)
                        {
                            redDir = face.transform.forward;
                        }
                        else if (face.transform.forward != redDir)
                        {
                            return false;
                        }
                        break;
                    case FaceColor.Orange:
                        if (orangeDir == Vector3.zero)
                        {
                            orangeDir = face.transform.forward;
                        }
                        else if (face.transform.forward != orangeDir)
                        {
                            return false;
                        }
                        break;
                    case FaceColor.Blue:
                        if (blueDir == Vector3.zero)
                        {
                            blueDir = face.transform.forward;
                        }
                        else if (face.transform.forward != blueDir)
                        {
                            return false;
                        }
                        break;
                    case FaceColor.White:
                        if (whiteDir == Vector3.zero)
                        {
                            whiteDir = face.transform.forward;
                        }
                        else if (face.transform.forward != whiteDir)
                        {
                            return false;
                        }
                        break;
                    case FaceColor.Yellow:
                        if (yellowDir == Vector3.zero)
                        {
                            yellowDir = face.transform.forward;
                        }
                        else if (face.transform.forward != yellowDir)
                        {
                            return false;
                        }
                        break;
                    case FaceColor.Green:
                        if (greenDir == Vector3.zero)
                        {
                            greenDir = face.transform.forward;
                        }
                        else if (face.transform.forward != greenDir)
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
    }
}
