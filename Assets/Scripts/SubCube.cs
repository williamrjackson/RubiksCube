using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCube : MonoBehaviour
{
    public static List<SubCube> AllSubCubes = new List<SubCube>();

    private Transform m_stationaryParent = null;
    private BoxCollider m_collider = null;

    private List<RotateRow> rows = new List<RotateRow>();

    // Start is called before the first frame update
    void Start()
    {
        m_stationaryParent = transform.parent;
        AllSubCubes.Add(this);
    }
    public void Reparent()
    {
        if (transform.parent != m_stationaryParent)
        {
            transform.parent = m_stationaryParent;
        }
    }
}
