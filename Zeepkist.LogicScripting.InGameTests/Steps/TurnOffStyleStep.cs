using System.Collections;
using System.Linq;

namespace TNRD.Zeepkist.LogicScripting.InGameTests.Steps;

/// <summary>
/// Turns a style of a Logic Block type off, as the startup check does when it doesn't match this game version, so
/// Compile and the window's check report a Script that places one or refers to one. The type's first style when none is
/// given. The run turns it back on before the next scenario. The window checks a Script a moment after an edit, so turn
/// it off before the <c>set-text</c> whose check should see it.
/// </summary>
internal sealed class TurnOffStyleStep : ScenarioStep
{
    public TurnOffStyleStep(string function, string style = null)
    {
        Function = function;
        Style = style;
    }

    public string Function { get; }

    /// <summary>
    /// Null for the type's first style.
    /// </summary>
    public string Style { get; }

    public override IEnumerator Run(StepContext context)
    {
        // In the editor, the startup check has run, so it can't find the style off again
        if (context.RequireEditor() == null)
            yield break;

        var type = context.RequireBlockType(Function);
        if (type == null)
            yield break;

        var style = Style == null ? type.DefaultStyle : type.FindStyle(Style);
        if (style == null)
        {
            var styles = type.HasStyles ? string.Join(", ", type.Styles.Select(s => s.Name)) : "(none: it has one style)";
            context.Fail($"{Function} has no style called '{Style}'", styles, Style);
            yield break;
        }

        var turnedOff = context.Window.TurnOffStyle(type, style);
        context.Detail = $"{type.Describe(style)}, block {style.BlockId}" + (turnedOff ? string.Empty : ", which was already off");
        yield return null;
    }
}
