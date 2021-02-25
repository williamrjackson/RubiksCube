using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public Transform ship;
    [Range(0.01f, .5f)]
    public float snapSpeed = .1f;
    public TouchAxisCtrl touchAxisControl;
    public float xRange = 4f;
    public float yRange = 4f;
    public Vector2 pitchAndRollRange = new Vector2(65, 100);

    private Vector2 m_InitialXY;
    Vector2 m_TargetXY;

    void Start()
    {
        float range = Mathf.Min(Screen.width, Screen.height) / 4;
        m_TargetXY = new Vector2(ship.localPosition.x, ship.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (touchAxisControl.IsTouching())
        {
            m_TargetXY = new Vector3(Mathf.Clamp(ship.localPosition.x + touchAxisControl.GetAxis("Horizontal"), -xRange, xRange),
                                     Mathf.Clamp(ship.localPosition.y + touchAxisControl.GetAxis("Vertical"), -yRange, yRange));
        }
        Vector2 lerpPos = Vector2.Lerp(new Vector2(ship.localPosition.x, ship.localPosition.y), m_TargetXY, snapSpeed);
        ship.localEulerAngles = new Vector3(Remap(ship.localPosition.y - m_TargetXY.y, -1, 1, -Mathf.Abs(pitchAndRollRange.x), Mathf.Abs(pitchAndRollRange.x)),
                                            ship.localEulerAngles.y,
                                            Remap(ship.localPosition.x - m_TargetXY.x, -1, 1, -Mathf.Abs(pitchAndRollRange.y), Mathf.Abs(pitchAndRollRange.y)));
        ship.localPosition = new Vector3(lerpPos.x, lerpPos.y, ship.localPosition.z);
    }

    float Remap(float val, float srcMin, float srcMax, float destMin, float destMax)
    {
        return Mathf.Lerp(destMin, destMax, Mathf.InverseLerp(srcMin, srcMax, val));
    }
}