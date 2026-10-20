using UnityEngine;
using System;

public class InputEvents
{
    public InputEventContext inputEventContext { get; private set; } = InputEventContext.DEFAULT;

    public void ChangeInputEventContext(InputEventContext newContxt)
    {
        this.inputEventContext = newContxt;
    }

    public event Action<InputEventContext> onSubmitPressed;
    public void OnSubmitPressed()
    {
        if (onSubmitPressed != null)
        {
            //OnSubmitPressed(this.inputEventContext);
        }
    }


}
