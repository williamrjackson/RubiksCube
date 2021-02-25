using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class SwipableObject : MonoBehaviour
{
    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;
    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;
    public CustomSwipeAngle[] customSwipeAngles;

    public void Swiped(TouchAxisCtrl.Direction direction)
    {
        switch (direction)
        {
            case TouchAxisCtrl.Direction.Up:
                if (OnSwipeUp != null)
                {
                    OnSwipeUp.Invoke();
                }
                break;
            case TouchAxisCtrl.Direction.Down:
                if (OnSwipeDown != null)
                {
                    OnSwipeDown.Invoke();
                }
                break;
            case TouchAxisCtrl.Direction.Right:
                if (OnSwipeRight != null)
                {
                    OnSwipeRight.Invoke();
                }
                break;
            default:
                if (OnSwipeLeft != null)
                {
                    OnSwipeLeft.Invoke();
                }
                break;
        }
    }
    public void Swiped(float angle)
    {
        foreach (CustomSwipeAngle swipeAngle in customSwipeAngles)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(angle, swipeAngle.targetAngle)) < (swipeAngle.range * .5f) &&
                swipeAngle.swipeEvent != null)
            {
                swipeAngle.swipeEvent.Invoke();
            }
        }
    }

    [System.Serializable]
    public class CustomSwipeAngle
    {
        public float targetAngle;
        public float range;
        public UnityEvent swipeEvent;
    }
}
