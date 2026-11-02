using System.Collections;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Opens the completion list at the caret, as Ctrl+Space does, and waits for the editor to read its rows, so the steps
/// after it don't check the rows a list already open had before.
/// </summary>
internal sealed class CompleteStep : ScenarioStep
{
    private const float WaitSeconds = 1;

    public override IEnumerator Run(StepContext context)
    {
        if (!context.RequireText())
            yield break;

        context.Window.RequestCompletion();
        yield return context.WaitUntil(() => !context.Window.IsCompletionRequestPending, WaitSeconds);
        if (context.Window.IsCompletionRequestPending)
            context.Fail("the editor wasn't drawn to open the completion list");
    }
}
