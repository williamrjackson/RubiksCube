using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    [Range(0f, 1f)]
    public float turnDuration = .25f;
    public int shuffleMoveCount = 25;
    public ParticleSystem celebrationParticles;
    public Transform flipAnchor;
    public RotateRow[] rotators;

    private bool checkForWin = false;
    private Stack<UndoElement> undoStack = new Stack<UndoElement>();
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
        checkForWin = true;
        RotateCubeRoutine = null;
    }

    public void RotateCube(bool left)
    {
        if (Blocked) return;
        float target = (left) ? 90f : -90f;
        RotateCubeRoutine = StartCoroutine(TurnCubeHoriz(target, turnDuration));
    }
    public void FlipCubeLeft(bool up)
    {
        if (Blocked) return;
        float target = (up) ? 90f : -90f;
        RotateCubeRoutine = StartCoroutine(TurnCubeLeftVert(target, turnDuration));
    }
    public void FlipCubeRight(bool up)
    {
        if (Blocked) return;
        float target = (up) ? 90f : -90f;
        RotateCubeRoutine = StartCoroutine(TurnCubeRightVert(target, turnDuration));
    }
    private IEnumerator TurnCubeHoriz(float targetRotation, float duration, bool addToStack = true)
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
            Vector3 updatedRot = Vector3.Lerp(Vector3.zero, Vector3.zero.With(y: targetRotation), Mathf.InverseLerp(0, duration, elapsedTime));
            flipAnchor.localEulerAngles = updatedRot;
            yield return new WaitForEndOfFrame();
        }
        flipAnchor.localEulerAngles = Vector3.zero.With(y: targetRotation);
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            subCube.Reparent();
        }
        flipAnchor.localEulerAngles = Vector3.zero;
        if (addToStack)
        {
            AddToUndoStack(new UndoElement(TurnCubeHoriz, targetRotation * -1f));
        }
        RotateCubeRoutine = null;
    }

    private IEnumerator TurnCubeLeftVert(float targetRotation, float duration, bool addToStack = true)
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
            Vector3 updatedRot = Vector3.Lerp(Vector3.zero, Vector3.zero.With(x: targetRotation), Mathf.InverseLerp(0, duration, elapsedTime));
            flipAnchor.localEulerAngles = updatedRot;
            yield return new WaitForEndOfFrame();
        }
        flipAnchor.localEulerAngles = Vector3.zero.With(x: targetRotation);
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            subCube.Reparent();
        }
        flipAnchor.localEulerAngles = Vector3.zero;
        if (addToStack)
        {
            AddToUndoStack(new UndoElement(TurnCubeLeftVert, targetRotation * -1f));
        }
        RotateCubeRoutine = null;
    }

    private IEnumerator TurnCubeRightVert(float targetRotation, float duration, bool addToStack = true)
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
            Vector3 updatedRot = Vector3.Lerp(Vector3.zero, Vector3.zero.With(z: targetRotation), Mathf.InverseLerp(0, duration, elapsedTime));
            flipAnchor.localEulerAngles = updatedRot;
            yield return new WaitForEndOfFrame();
        }
        flipAnchor.localEulerAngles = Vector3.zero.With(z: targetRotation);
        foreach (SubCube subCube in SubCube.AllSubCubes)
        {
            subCube.Reparent();
        }
        flipAnchor.localEulerAngles = Vector3.zero;
        if (addToStack)
        {
            AddToUndoStack(new UndoElement(TurnCubeRightVert, targetRotation * -1f));
        }
        RotateCubeRoutine = null;
    }

    public void AddToUndoStack(UndoElement undoElement)
    {
        undoStack.Push(undoElement);
        if (checkForWin)
        {
            if (Face.IsWinState)
            {
                Debug.Log("Win!");
                celebrationParticles.Play();
                checkForWin = false;
            }
        }
    }

    int UndoClickCount = 0;
    Coroutine undoRoutine = null;
    public void Undo()
    {
        UndoClickCount++;
        if (undoRoutine == null)
        {
            undoRoutine = StartCoroutine(ExecuteUndo());
        }
    }

    private IEnumerator ExecuteUndo()
    {
        if (Blocked) yield break;
        while ((undoStack.Count > 0) && (UndoClickCount > 0))
        {
            UndoElement undo = undoStack.Pop();
            UndoClickCount--;
            yield return StartCoroutine(undo.undoCommand(undo.angle, turnDuration, false));
        }
        if (undoStack.Count == 0)
        {
            checkForWin = false;
        }
        undoRoutine = null;
    }

    public void Reset()
    {
        if (Blocked) return;
        RotateCubeRoutine = StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        int stackCount = undoStack.Count;
        for (int i = 0; i < stackCount; i++)
        {
            UndoElement undo = undoStack.Pop();
            yield return RotateCubeRoutine = StartCoroutine(undo.undoCommand(undo.angle, .1f, false));
        }
        RotateCubeRoutine = null;
        checkForWin = false;
    }

    public class UndoElement
    {
        public delegate IEnumerator UndoCommand(float angle, float duration, bool addToStack);
        public UndoCommand undoCommand;
        public float angle;

        public UndoElement(UndoCommand undoCommand, float angle)
        {
            this.undoCommand = undoCommand;
            this.angle = angle;
        }
    }
}
