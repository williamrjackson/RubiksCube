using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRow : MonoBehaviour
{
    public enum FacingDirection { Left, Right, Top }
    public enum RowCondition { xNeg1, xZero, xPos1, yNeg1, yZero, yPos1, zNeg1, zZero, zPos1 }
    public FacingDirection facingDirection;
    public RowCondition rowCondition;
    public Wrj.RandomizedSoundEffect soundEffects;
    private static Coroutine CurrentMoveRoutine;
    public static bool IsMoving => CurrentMoveRoutine != null;

    public void RotateLeft()
    {
        if (GameControl.Instance.Blocked)
        {
            return;
        }
        CurrentMoveRoutine = StartCoroutine(DoMove(90f, GameControl.Instance.turnDuration));
    }
    public void RotateRight()
    {
        if (GameControl.Instance.Blocked)
        {
            return;
        }
        CurrentMoveRoutine = StartCoroutine(DoMove(-90f, GameControl.Instance.turnDuration));
    }

    public IEnumerator MoveRandom(float duration)
    {
        float moveDeg = (Wrj.Utils.CoinFlip) ? -90f : 90f;
        yield return CurrentMoveRoutine = StartCoroutine(DoMove(moveDeg, duration));
    }
    public void RotateCube(bool left, float duration)
    {
        float dir = (left) ? -90f : 90f;
        CurrentMoveRoutine = StartCoroutine(DoMove(dir, duration));
    }

    IEnumerator DoMove(float degrees, float duration, bool addToStack = true)
    {
        soundEffects.PlayRandom(.9f, 1.1f, .9f, 1.1f);
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            bool isApplicable = false;
            switch (rowCondition)
            {
                case RowCondition.xNeg1:
                    if (subCube.transform.localPosition.x < -.5)
                        isApplicable = true;
                    break;
                case RowCondition.xZero:
                    if (subCube.transform.localPosition.x > -.5 && subCube.transform.localPosition.x < .5f)
                        isApplicable = true;
                    break;
                case RowCondition.xPos1:
                    if (subCube.transform.localPosition.x > .5)
                        isApplicable = true;
                    break;
                case RowCondition.yNeg1:
                    if (subCube.transform.localPosition.y < -.5)
                        isApplicable = true;
                    break;
                case RowCondition.yZero:
                    if (subCube.transform.localPosition.y > -.5 && subCube.transform.localPosition.y < .5f)
                        isApplicable = true;
                    break;
                case RowCondition.yPos1:
                    if (subCube.transform.localPosition.y > .5)
                        isApplicable = true;
                    break;
                case RowCondition.zNeg1:
                    if (subCube.transform.localPosition.z < -.5)
                        isApplicable = true;
                    break;
                case RowCondition.zZero:
                    if (subCube.transform.localPosition.z > -.5 && subCube.transform.localPosition.z < .5f)
                        isApplicable = true;
                    break;
                case RowCondition.zPos1:
                    if (subCube.transform.localPosition.z > .5)
                        isApplicable = true;
                    break;
                default:
                    break;
            }
            if (isApplicable)
            {
                subCube.transform.parent = transform;
            }
        }

        float elapsedTime = 0f;
        float initialY = transform.localEulerAngles.y;
        float targetY = initialY + degrees;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            float updatedRot = Mathf.Lerp(initialY, targetY, Mathf.InverseLerp(0, duration, elapsedTime));
            transform.localEulerAngles = transform.localEulerAngles.With(y: updatedRot);
            yield return new WaitForEndOfFrame();
        }
        transform.localEulerAngles = transform.localEulerAngles.With(y: targetY);
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            subCube.Reparent();
        }
        if (addToStack)
        {
            var undo = new GameControl.UndoElement(DoMove, degrees * -1f);
            GameControl.Instance.AddToUndoStack(undo);
        }
        CurrentMoveRoutine = null;
    }
}
