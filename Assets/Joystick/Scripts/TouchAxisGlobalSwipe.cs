using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAxisGlobalSwipe : MonoBehaviour
{
    [SerializeField]
    TouchAxisCtrl swipe = null;
    // Start is called before the first frame update
    void Start()
    {
        swipe.OnSwipe += OnSwipe;
    }

    private void OnSwipe(TouchAxisCtrl.Direction direction)
    {
        switch (direction)
        {
            case TouchAxisCtrl.Direction.Up:
                transform.position += transform.up;
                break;
            case TouchAxisCtrl.Direction.Down:
                transform.position -= transform.up;
                break;
            case TouchAxisCtrl.Direction.Right:
                transform.position += transform.right;
                break;
            default:
                transform.position -= transform.right;
                break;
        }
    }
}
