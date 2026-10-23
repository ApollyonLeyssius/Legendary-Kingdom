using UnityEngine;
using System;

public class InputEvents
{
    public InputEventContext inputEventContext { get; private set; } = InputEventContext.DEFAULT;

    public void ChangeInputEventContext(InputEventContext newContxt)
    {
        this.inputEventContext = newContxt;
    }

    public event Action<Vector2> OnMovePressed;
    public void MovePressed(Vector2 moveDir)
    {
        OnMovePressed?.Invoke(moveDir);
    }

    public event Action<InputEventContext> OnSubmitPressed;
    public void SubmitPressed()
    {
        OnSubmitPressed?.Invoke(this.inputEventContext);
    }

    public event Action onQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        onQuestLogTogglePressed?.Invoke();
    }


}
