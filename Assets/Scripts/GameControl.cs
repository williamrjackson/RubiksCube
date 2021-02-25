using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    [Range(0f, 1f)]
    public float turnDuration = .25f;
    public int shuffleMoveCount = 25;
    public Transform flipAnchor;
    public RotateRow[] rotators;

    private Coroutine RotateCubeRoutine;

    public bool Blocked
    {
        get
        {
            return (RotateRow.IsMoving || RotateCubeRoutine != null);
        }
    }
    public static GameControl Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Shuffle()
    {
        if (Blocked) return;
        RotateCubeRoutine = StartCoroutine(ShuffleRoutine());
    }

    private IEnumerator ShuffleRoutine()
    {
        for (int i = 0; i < shuffleMoveCount; i++)
        {
            yield return StartCoroutine(rotators.GetRandom().MoveRandom(.1f));
        }
        RotateCubeRoutine = null;
    }

    public void RotateCube(bool left)
    {
        if (Blocked) return;
        Vector3 target = (left) ? Vector3.zero.With(y: 90f) : Vector3.zero.With(y: -90f);
        RotateCubeRoutine = StartCoroutine(TurnCube(target, turnDuration));
    }
    public void FlipCubeLeft(bool up)
    {
        if (Blocked) return;
        Vector3 target = (up) ? Vector3.zero.With(x: 90f) : Vector3.zero.With(x: -90f);
        RotateCubeRoutine = StartCoroutine(TurnCube(target, turnDuration));
    }
    public void FlipCubeRight(bool up)
    {
        if (Blocked) return;
        Vector3 target = (up) ? Vector3.zero.With(z: 90f) : Vector3.zero.With(z: -90f);
        RotateCubeRoutine = StartCoroutine(TurnCube(target, turnDuration));
    }
    private IEnumerator TurnCube(Vector3 targetRotation, float duration)
    {
        flipAnchor.localEulerAngles = Vector3.zero;
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            subCube.transform.parent = flipAnchor;
        }
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 updatedRot = Vector3.Lerp(Vector3.zero, targetRotation, Mathf.InverseLerp(0, duration, elapsedTime));
            flipAnchor.localEulerAngles = updatedRot;
            yield return new WaitForEndOfFrame();
        }
        flipAnchor.localEulerAngles = targetRotation;
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            subCube.Reparent();
        }
        flipAnchor.localEulerAngles = Vector3.zero;
        RotateCubeRoutine = null;
    }
}
