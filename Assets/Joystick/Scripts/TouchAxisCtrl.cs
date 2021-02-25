using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAxisCtrl : MonoBehaviour
{
    public bool spawnOnTouch;
    public Transform node;
    [Range (0f, 1f)]
    public float touchArea = .25f;
    [Range(0f, 1f)]
    public float nodeRange = .25f;
    public float chase = 100;

    public enum Direction { Up, Down, Left, Right };
    public delegate void SwipeDelegate(Direction direction);
    public delegate void SwipeAngleDelegate(float angle);
    public SwipeDelegate OnSwipe;
    public SwipeAngleDelegate OnSwipeAngle;

    int m_CapturedTouch = -1;
    Vector2 m_Axis;
    Vector3 m_InitialScale;
    Vector2 velRef = Vector2.zero;
    private bool locked;
    private bool swipeRegistered;
    private SwipableObject swipable = null;


    private static List<TouchAxisCtrl> touchControllerList;

    // Use this for initialization
    void Start()
    {
        if (node.parent != transform)
        {
            node.parent = transform;
        }
        node.position = transform.position;
        m_InitialScale = transform.localScale;

        Reset();
    }

    void OnEnable()
    {
        if (touchControllerList == null)
        {
            touchControllerList = new List<TouchAxisCtrl>();
        }
        touchControllerList.Add(this);
    }

    void OnDisable()
    {
        touchControllerList.Remove(this);
    }

    void Update()
    {
        // Get the mouse position - this is for testing. 
        // If not in the unity editor, get the touch position. Either only care about a touch in the right zone, 
        // or use the spawn option and only care about the first touch.
        if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (m_CapturedTouch < 0)
            {
                for (int i = 0; i < Input.touchCount; ++i)
                {
                    if ((!spawnOnTouch && (Input.GetTouch(i).phase == TouchPhase.Began) && 
                        (GetPointDistance(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position)) < GetScaledParimeter(touchArea))) 
                        || (spawnOnTouch && Input.GetTouch(i).phase == TouchPhase.Began))
                    {
                        CaptureTouch(i, Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position));
                    }
                }
            }
            else
            {
                if (Input.GetTouch(m_CapturedTouch).phase == TouchPhase.Ended)
                {
                    Reset();
                }
                else
                {
                    HandleValidTouch(Camera.main.ScreenToWorldPoint(Input.GetTouch(m_CapturedTouch).position));
                }
            }
        }
        else
        {
            if (m_CapturedTouch < 0)
            {
                if ((!spawnOnTouch && Input.GetMouseButtonDown(0) && GetPointDistance(Input.mousePosition) < GetScaledParimeter(touchArea))
                    || (spawnOnTouch && Input.GetMouseButtonDown(0)))
                {
                    CaptureTouch(0, Input.mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Reset();
            }
            else
            {
                HandleValidTouch(Input.mousePosition);
            }
        }
    }

    void HandleValidTouch(Vector2 currentPos)
    {
        if (locked)
            return;

        if (GetPointDistance(currentPos) > GetScaledParimeter(nodeRange))
        {
            node.position = transform.position + (ToVector3(currentPos) - transform.position).normalized * GetScaledParimeter(nodeRange);
        }
        else
        {
            node.position = currentPos;
        }
        m_Axis = node.localPosition.normalized * Remap(GetPointDistance(currentPos), 0, GetScaledParimeter(touchArea), 0, 1);
        transform.position = Vector2.SmoothDamp(transform.position, currentPos, ref velRef, chase, 1000, Time.deltaTime);

        if ((OnSwipe != null || swipable != null) && m_Axis.sqrMagnitude > .6f && !swipeRegistered)
        {
            Direction swipeDir = Direction.Up;
            swipeRegistered = true;
            if (m_Axis.x < -.6f)
            {
                swipeDir = (Direction.Left);
            }
            else if (m_Axis.x > .6f)
            {
                swipeDir = (Direction.Right);
            }
            else if (m_Axis.y < -.6f)
            {
                swipeDir = (Direction.Down);
            }

            if (swipable != null)
            {
                swipable.Swiped(swipeDir);
                swipable.Swiped(m_Axis.Angle());
            }
            else 
            {
                if (OnSwipe != null)
                    OnSwipe(swipeDir);
                if (OnSwipeAngle != null)
                    OnSwipeAngle(m_Axis.Angle());
            }
        }
    }

    void CaptureTouch(int touchIndex, Vector2 touchPos)
    {
        if (locked)
            return;

        if (spawnOnTouch)
        {
            transform.position = touchPos;
            transform.localScale = m_InitialScale;
            RaycastHit hit;
            RaycastHit2D hit2d;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (hit2d = Physics2D.Raycast(ray.origin, ray.direction))
            {
                swipable = hit2d.collider.GetComponent<SwipableObject>();
            }
            else if (Physics.Raycast(ray, out hit))
            {
                swipable = hit.collider.GetComponent<SwipableObject>();
            }
        }
        else
        {
            foreach (TouchAxisCtrl touchAxis in touchControllerList)
            {
                if (touchAxis.spawnOnTouch && touchAxis != this)
                {
                    touchAxis.Lock();
                }
            }
        }
        m_CapturedTouch = touchIndex;
    }

    protected void Lock()
    {
        Reset();
        locked = true;
    }
    protected void Unlock()
    {
        locked = false;
    }

    void Reset()
    {
        if (spawnOnTouch)
        {
            transform.localScale = Vector3.zero;
        }
        else
        {
            foreach (TouchAxisCtrl touchAxis in touchControllerList)
            {
                if (touchAxis.spawnOnTouch && touchAxis != this)
                {
                    touchAxis.Unlock();
                }
            }
        }
        ZeroNode();
        m_CapturedTouch = -1;
    }

    float GetPointDistance(Vector2 point)
    {
        return Vector2.Distance(point, transform.position);
    }

    // __ HELPER FUNCTIONS __ //
    Vector3 ToVector3(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0f);
    }
    float GetScaledParimeter(float parimeter)
    {
        return parimeter * Mathf.Max(Screen.width, Screen.height) * (transform.localScale.magnitude / 2);
    }
    float Remap(float val, float srcMin, float srcMax, float destMin, float destMax)
    {
        return Mathf.Lerp(destMin, destMax, Mathf.InverseLerp(srcMin, srcMax, val));
    }

    // __ PUBLIC __ //
    public bool IsTouching()
    {
        return (m_CapturedTouch > -1);
    }
    public void ZeroNode()
    {
        node.position = transform.position;
        m_Axis = Vector2.zero;
        swipeRegistered = false;
        swipable = null;
    }
    public Vector2 GetAxis()
    {
        return m_Axis;
    }
    public float GetAxis(string vertOrHorizontal)
    {
        if (vertOrHorizontal == "Horizontal")
            return m_Axis.x;
        else if (vertOrHorizontal == "Vertical")
            return m_Axis.y;
        else if (vertOrHorizontal == "InverseHorizontal")
            return Remap(m_Axis.x, -1, 1, 1, -1);
        else if (vertOrHorizontal == "InverseVertical")
            return Remap(m_Axis.y, -1, 1, 1, -1);
        else
            return 0f;
    }

    // __ EDITOR GIZMOS __ //
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetScaledParimeter(touchArea));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, GetScaledParimeter(nodeRange));
    }
}
