using UnityEngine;

public class TouchAxisMove : MonoBehaviour {

    public TouchAxisCtrl touchAxis;
    public float speed = 7.0F;
    public float rotateSpeed = 10.0f;
    void Update()
    {
        if (touchAxis.GetAxis().sqrMagnitude != 0)
        {
            Quaternion targetRot = Quaternion.Euler(0, 0, -Mathf.Atan2(touchAxis.GetAxis().x, touchAxis.GetAxis().y) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            transform.position += transform.up * touchAxis.GetAxis().sqrMagnitude * speed * Time.deltaTime;
        }
    }
}
